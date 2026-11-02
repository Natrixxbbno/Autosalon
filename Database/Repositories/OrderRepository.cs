using Npgsql;

namespace AutoSalon.Database.Repositories
{
    public sealed record TradeInDetails(int ManufacturerId, string Model, int Year, int Mileage,
        string Color, string Registration, decimal Value, decimal DesiredPrice);

    public class OrderRepository
    {
        public System.Data.DataTable GetAll()
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(@"SELECT o.id, o.last_name, o.first_name,
                o.contact_number, o.email, m.name AS category, c.model AS product,
                c.registration_number, o.quantity, o.created_at,
                COALESCE(o.payment_method, 'Nespecificată') AS payment_method,
                o.trade_in_model, o.trade_in_year, o.trade_in_mileage, o.trade_in_value,
                o.desired_car_price, o.trade_in_balance,
                o.trade_in_registration,
                CASE WHEN o.trade_in_model IS NULL THEN 'Standard' ELSE 'Trade-in' END AS order_type,
                s.name AS car_status, (o.completed_at IS NOT NULL) AS completed
                FROM orders o
                LEFT JOIN cars c ON c.id = o.car_id
                LEFT JOIN manufacturers m ON m.id = c.manufacturer_id
                LEFT JOIN car_statuses s ON s.id = c.status_id
                ORDER BY o.created_at DESC, o.id DESC", conn);
            using var reader = cmd.ExecuteReader();
            var table = new System.Data.DataTable();
            table.Load(reader);
            return table;
        }

        public int Add(string lastName, string firstName, string contactNumber,
            string email, int carId, int quantity, string paymentMethod, TradeInDetails? tradeIn = null)
        {
            if (paymentMethod != "Cash" && paymentMethod != "Card")
                throw new ArgumentException("Metoda de plată trebuie să fie Cash sau Card.", nameof(paymentMethod));
            if (tradeIn != null && (quantity != 1 || string.IsNullOrWhiteSpace(tradeIn.Model)
                || tradeIn.Model.Length > 100 || tradeIn.Year < 1900 || tradeIn.Year > DateTime.Now.Year + 1
                || tradeIn.Mileage < 0 || tradeIn.Value <= 0 || tradeIn.DesiredPrice <= 0
                || tradeIn.Value > 99999999.99m || tradeIn.ManufacturerId <= 0 || string.IsNullOrWhiteSpace(tradeIn.Color)
                || tradeIn.Color.Length > 50 || string.IsNullOrWhiteSpace(tradeIn.Registration)
                || tradeIn.Registration.Length > 20))
                throw new ArgumentException("Datele pentru trade-in nu sunt valide.", nameof(tradeIn));
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();
            using var transaction = conn.BeginTransaction();
            if (tradeIn != null)
            {
                using var duplicate = new NpgsqlCommand(@"SELECT EXISTS (
                    SELECT 1 FROM cars WHERE upper(trim(registration_number)) = upper(trim(@registration))
                    UNION ALL SELECT 1 FROM orders WHERE completed_at IS NULL
                    AND upper(trim(trade_in_registration)) = upper(trim(@registration)))", conn, transaction);
                duplicate.Parameters.AddWithValue("registration", tradeIn.Registration);
                if (Equals(duplicate.ExecuteScalar(), true))
                    throw new InvalidOperationException("Mașina oferită există deja în inventar sau într-o comandă trade-in activă.");
            }
            using (var reserve = new NpgsqlCommand(@"UPDATE cars SET status_id =
                (SELECT id FROM car_statuses WHERE name = 'Reserved' ORDER BY id LIMIT 1)
                WHERE id = @carId AND status_id IN
                (SELECT id FROM car_statuses WHERE name = 'Available')
                AND NOT EXISTS (SELECT 1 FROM orders WHERE car_id = @carId AND completed_at IS NULL)
                RETURNING price", conn, transaction))
            {
                reserve.Parameters.AddWithValue("carId", carId);
                var price = reserve.ExecuteScalar();
                if (price == null)
                    throw new InvalidOperationException("Mașina nu mai este disponibilă pentru comandă.");
                if (tradeIn != null && Convert.ToDecimal(price) != tradeIn.DesiredPrice)
                    throw new InvalidOperationException("Prețul automobilului s-a schimbat. Selectați din nou automobilul pentru recalculare.");
            }
            using var cmd = new NpgsqlCommand(@"INSERT INTO orders
                (last_name, first_name, contact_number, email, car_id, quantity, payment_method,
                    trade_in_model, trade_in_year, trade_in_mileage, trade_in_value, desired_car_price,
                    trade_in_manufacturer_id, trade_in_color, trade_in_registration)
                VALUES (@lastName, @firstName, @phone, @email, @carId, @quantity, @paymentMethod,
                    @tradeModel, @tradeYear, @tradeMileage, @tradeValue, @desiredPrice,
                    @tradeManufacturer, @tradeColor, @tradeRegistration)
                RETURNING id", conn, transaction);
            cmd.Parameters.AddWithValue("lastName", lastName);
            cmd.Parameters.AddWithValue("firstName", firstName);
            cmd.Parameters.AddWithValue("phone", contactNumber);
            cmd.Parameters.AddWithValue("email", email);
            cmd.Parameters.AddWithValue("carId", carId);
            cmd.Parameters.AddWithValue("quantity", quantity);
            cmd.Parameters.AddWithValue("paymentMethod", paymentMethod);
            cmd.Parameters.AddWithValue("tradeModel", NpgsqlTypes.NpgsqlDbType.Varchar, (object?)tradeIn?.Model ?? DBNull.Value);
            cmd.Parameters.AddWithValue("tradeYear", NpgsqlTypes.NpgsqlDbType.Integer, (object?)tradeIn?.Year ?? DBNull.Value);
            cmd.Parameters.AddWithValue("tradeMileage", NpgsqlTypes.NpgsqlDbType.Integer, (object?)tradeIn?.Mileage ?? DBNull.Value);
            cmd.Parameters.AddWithValue("tradeValue", NpgsqlTypes.NpgsqlDbType.Numeric, (object?)tradeIn?.Value ?? DBNull.Value);
            cmd.Parameters.AddWithValue("desiredPrice", NpgsqlTypes.NpgsqlDbType.Numeric, (object?)tradeIn?.DesiredPrice ?? DBNull.Value);
            cmd.Parameters.AddWithValue("tradeManufacturer", NpgsqlTypes.NpgsqlDbType.Integer, (object?)tradeIn?.ManufacturerId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("tradeColor", NpgsqlTypes.NpgsqlDbType.Varchar, (object?)tradeIn?.Color ?? DBNull.Value);
            cmd.Parameters.AddWithValue("tradeRegistration", NpgsqlTypes.NpgsqlDbType.Varchar, (object?)tradeIn?.Registration ?? DBNull.Value);
            int id = Convert.ToInt32(cmd.ExecuteScalar());
            transaction.Commit();
            return id;
        }

        public void Complete(int orderId)
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();
            using var transaction = conn.BeginTransaction();
            int carId;
            using (var find = new NpgsqlCommand("SELECT car_id, completed_at FROM orders WHERE id = @id FOR UPDATE", conn, transaction))
            {
                find.Parameters.AddWithValue("id", orderId);
                using var reader = find.ExecuteReader();
                if (!reader.Read()) throw new InvalidOperationException("Comanda nu mai există.");
                if (!reader.IsDBNull(1)) return;
                carId = reader.GetInt32(0);
            }
            // Available is accepted for orders created before reservation was introduced.
            using (var sell = new NpgsqlCommand(@"UPDATE cars SET status_id =
                (SELECT id FROM car_statuses WHERE name = 'Sold' ORDER BY id LIMIT 1)
                WHERE id = @carId AND status_id IN
                (SELECT id FROM car_statuses WHERE name IN ('Reserved', 'Available'))", conn, transaction))
            {
                sell.Parameters.AddWithValue("carId", carId);
                if (sell.ExecuteNonQuery() != 1)
                    throw new InvalidOperationException("Mașina nu poate fi vândută: verificați statutul ei.");
            }
            using (var complete = new NpgsqlCommand("UPDATE orders SET completed_at = CURRENT_TIMESTAMP WHERE id = @id", conn, transaction))
            {
                complete.Parameters.AddWithValue("id", orderId);
                complete.ExecuteNonQuery();
            }
            // Add the exchanged car only when the transaction is finalized, once per order.
            using (var receive = new NpgsqlCommand(@"INSERT INTO cars
                (manufacturer_id, model, year, color, price, registration_number, purchase_date, mileage, status_id)
                SELECT trade_in_manufacturer_id, trade_in_model, trade_in_year, trade_in_color,
                    trade_in_value, trade_in_registration, CURRENT_DATE, trade_in_mileage,
                    (SELECT id FROM car_statuses WHERE name = 'Available' ORDER BY id LIMIT 1)
                FROM orders WHERE id = @id AND trade_in_model IS NOT NULL", conn, transaction))
            {
                receive.Parameters.AddWithValue("id", orderId);
                receive.ExecuteNonQuery();
            }
            transaction.Commit();
        }
    }
}
