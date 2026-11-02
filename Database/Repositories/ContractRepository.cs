using System.Data;
using Npgsql;
using NpgsqlTypes;
using AutoSalon.Database.Models;

namespace AutoSalon.Database.Repositories
{
    public sealed record ContractRequest(bool Rental, string LastName, string FirstName, string Phone,
        string Email, int CarId, decimal ExpectedPrice, string PaymentMethod, decimal DownPayment,
        decimal AnnualRate, int Months, DateTime Start, DateTime End, decimal DailyRate);

    public class ContractRepository
    {
        public int Add(ContractRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.LastName) || string.IsNullOrWhiteSpace(request.FirstName)
                || !System.Text.RegularExpressions.Regex.IsMatch(request.Phone, "^[0-9]{9,15}$")
                || !System.Net.Mail.MailAddress.TryCreate(request.Email, out _)
                || request.PaymentMethod is not ("Cash" or "Card"))
                throw new ArgumentException("Datele clientului nu sunt valide.");
            CreditCalculation? credit = null;
            int days = (request.End.Date - request.Start.Date).Days;
            if (request.Rental)
            {
                if (request.Start.Date < DateTime.Today || days < 1 || days > 3650 || request.DailyRate <= 0 || request.DailyRate > 99999999.99m)
                    throw new ArgumentException("Închirierea trebuie să dureze între 1 și 3650 zile, cu tarif zilnic pozitiv.");
            }
            else credit = CreditCalculation.Calculate(request.ExpectedPrice, request.DownPayment, request.AnnualRate, request.Months);
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();
            using var transaction = conn.BeginTransaction();
            using (var reserve = new NpgsqlCommand(@"UPDATE cars SET status_id =
                (SELECT id FROM car_statuses WHERE name = @status ORDER BY id LIMIT 1)
                WHERE id = @car AND price = @price AND status_id IN
                (SELECT id FROM car_statuses WHERE name = 'Available')
                AND NOT EXISTS (SELECT 1 FROM orders WHERE car_id = @car AND completed_at IS NULL)
                AND NOT EXISTS (SELECT 1 FROM auto_contracts WHERE car_id = @car AND closed_at IS NULL)
                RETURNING id", conn, transaction))
            {
                reserve.Parameters.AddWithValue("status", request.Rental ? "Reserved" : "Sold");
                reserve.Parameters.AddWithValue("car", request.CarId);
                reserve.Parameters.AddWithValue("price", request.ExpectedPrice);
                if (reserve.ExecuteScalar() == null)
                    throw new InvalidOperationException("Mașina nu mai este disponibilă sau prețul s-a schimbat. Redeschideți formularul.");
            }
            using var cmd = new NpgsqlCommand(@"INSERT INTO auto_contracts
                (kind, last_name, first_name, phone, email, car_id, payment_method, car_price,
                down_payment, annual_rate, months, monthly_payment, total_cost, start_date, end_date, daily_rate)
                VALUES (@kind, @last, @first, @phone, @email, @car, @payment, @price,
                @down, @rate, @months, @monthly, @total, @start, @end, @daily) RETURNING id", conn, transaction);
            cmd.Parameters.AddWithValue("kind", request.Rental ? "Rental" : "Credit");
            cmd.Parameters.AddWithValue("last", request.LastName.Trim());
            cmd.Parameters.AddWithValue("first", request.FirstName.Trim());
            cmd.Parameters.AddWithValue("phone", request.Phone);
            cmd.Parameters.AddWithValue("email", request.Email.Trim());
            cmd.Parameters.AddWithValue("car", request.CarId);
            cmd.Parameters.AddWithValue("payment", request.PaymentMethod);
            cmd.Parameters.AddWithValue("price", request.ExpectedPrice);
            void Optional(string name, NpgsqlDbType type, object? value) => cmd.Parameters.AddWithValue(name, type, value ?? DBNull.Value);
            Optional("down", NpgsqlDbType.Numeric, request.Rental ? null : request.DownPayment);
            Optional("rate", NpgsqlDbType.Numeric, request.Rental ? null : request.AnnualRate);
            Optional("months", NpgsqlDbType.Integer, request.Rental ? null : request.Months);
            Optional("monthly", NpgsqlDbType.Numeric, credit?.Monthly);
            cmd.Parameters.AddWithValue("total", request.Rental ? days * request.DailyRate : credit!.Total);
            cmd.Parameters.AddWithValue("start", NpgsqlDbType.Date, request.Start.Date);
            Optional("end", NpgsqlDbType.Date, request.Rental ? request.End.Date : null);
            Optional("daily", NpgsqlDbType.Numeric, request.Rental ? request.DailyRate : null);
            int id = Convert.ToInt32(cmd.ExecuteScalar());
            transaction.Commit();
            return id;
        }

        public DataTable GetAll()
        {
            using var conn = DatabaseConnection.GetConnection(); conn.Open();
            using var cmd = new NpgsqlCommand(@"SELECT a.*, m.name || ' ' || c.model || ' — ' || c.registration_number AS car,
                CASE WHEN a.kind = 'Credit' THEN 'Credit' ELSE 'Închiriere' END AS type,
                CASE WHEN a.closed_at IS NOT NULL THEN 'Închis' ELSE 'Activ' END AS state
                FROM auto_contracts a JOIN cars c ON c.id = a.car_id
                JOIN manufacturers m ON m.id = c.manufacturer_id ORDER BY a.created_at DESC, a.id DESC", conn);
            using var reader = cmd.ExecuteReader(); var table = new DataTable(); table.Load(reader); return table;
        }

        public void RegisterPaymentOrReturn(int id, int expectedPaid)
        {
            using var conn = DatabaseConnection.GetConnection(); conn.Open();
            using var transaction = conn.BeginTransaction();
            int car; bool rental; int paid; int months;
            using (var cmd = new NpgsqlCommand("SELECT car_id, kind, installments_paid, months, closed_at FROM auto_contracts WHERE id = @id FOR UPDATE", conn, transaction))
            {
                cmd.Parameters.AddWithValue("id", id);
                using var reader = cmd.ExecuteReader();
                if (!reader.Read() || !reader.IsDBNull(4)) throw new InvalidOperationException("Contractul nu mai este activ.");
                car = reader.GetInt32(0); rental = reader.GetString(1) == "Rental";
                paid = reader.GetInt32(2); months = rental ? 0 : reader.GetInt32(3);
                if (!rental && paid != expectedPaid) throw new InvalidOperationException("Lista s-a modificat. Actualizați înainte de a înregistra plata.");
            }
            if (rental)
            {
                using var release = new NpgsqlCommand(@"UPDATE cars SET status_id =
                    (SELECT id FROM car_statuses WHERE name = 'Available' ORDER BY id LIMIT 1)
                    WHERE id = @car AND status_id IN (SELECT id FROM car_statuses WHERE name = 'Reserved')", conn, transaction);
                release.Parameters.AddWithValue("car", car);
                if (release.ExecuteNonQuery() != 1) throw new InvalidOperationException("Statutul mașinii a fost modificat. Verificați inventarul.");
            }
            using var update = new NpgsqlCommand(rental
                ? "UPDATE auto_contracts SET closed_at = CURRENT_TIMESTAMP WHERE id = @id"
                : "UPDATE auto_contracts SET installments_paid = installments_paid + 1, closed_at = CASE WHEN installments_paid + 1 = months THEN CURRENT_TIMESTAMP ELSE NULL END WHERE id = @id", conn, transaction);
            update.Parameters.AddWithValue("id", id); update.ExecuteNonQuery(); transaction.Commit();
        }
    }
}
