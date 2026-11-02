using System.Data;
using System.Drawing;
using System.Windows.Forms;
using AutoSalon.Database.Repositories;

namespace AutoSalon.Pages
{
    public sealed class ContractReportsPage : Form
    {
        private readonly DataGridView grid = new() { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false,
            AutoGenerateColumns = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells, RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, BackgroundColor = Color.White };
        public ContractReportsPage()
        {
            Text = "AutoSalon — Credite și închirieri"; WindowState = FormWindowState.Maximized;
            Size = new Size(1200, 650); StartPosition = FormStartPosition.CenterParent; Font = new Font("Segoe UI", 10);
            grid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            foreach (var column in new[] { ("id", "Nr."), ("type", "Tip"), ("last_name", "Nume"), ("first_name", "Prenume"),
                ("phone", "Telefon"), ("car", "Automobil"), ("payment_method", "Plată"), ("total_cost", "Total (€)"), ("state", "Stare") })
                grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = column.Item1, Name = column.Item1, HeaderText = column.Item2 });
            grid.Columns["car"].FillWeight = 200; grid.Columns["total_cost"].DefaultCellStyle.Format = "N2";
            var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(10) };
            var refresh = new Button { Text = "Actualizează", AutoSize = true };
            var details = new Button { Text = "Detalii contract", AutoSize = true };
            var action = new Button { Text = "Înregistrează rată / returnare", AutoSize = true };
            toolbar.Controls.AddRange(new Control[] { refresh, details, action });
            Controls.Add(grid); Controls.Add(toolbar);
            refresh.Click += (s, e) => Reload(); Load += (s, e) => Reload();
            details.Click += (s, e) =>
            {
                if (grid.CurrentRow?.DataBoundItem is not DataRowView row) return;
                bool rental = row["kind"].ToString() == "Rental";
                string text = $"Client: {row["last_name"]} {row["first_name"]}\nTelefon: {row["phone"]}\nEmail: {row["email"]}\nAutomobil: {row["car"]}\n";
                if (rental) text += $"Preluare: {Convert.ToDateTime(row["start_date"]):dd.MM.yyyy}\nReturnare planificată: {Convert.ToDateTime(row["end_date"]):dd.MM.yyyy}\nTarif/zi: {row["daily_rate"]} €\n";
                else
                {
                    int months = Convert.ToInt32(row["months"]); int paid = Convert.ToInt32(row["installments_paid"]);
                    decimal totalLoan = Convert.ToDecimal(row["total_cost"]) - Convert.ToDecimal(row["down_payment"]);
                    decimal monthly = Convert.ToDecimal(row["monthly_payment"]);
                    decimal remaining = paid == months ? 0 : totalLoan - paid * monthly;
                    text += $"Preț: {row["car_price"]} €\nAvans: {row["down_payment"]} €\nDobândă anuală: {row["annual_rate"]}%\n" +
                        $"Perioadă: {months} luni\nRată: {monthly:N2} €\nUltima rată: {totalLoan - monthly * (months - 1):N2} €\n" +
                        $"Rate achitate: {paid}/{months}\nRest de achitat: {remaining:N2} €\n" +
                        (paid < months ? $"Următoarea scadență: {Convert.ToDateTime(row["start_date"]).AddMonths(paid + 1):dd.MM.yyyy}\n" : "");
                }
                MessageBox.Show(text + $"Total: {row["total_cost"]} €\nStare: {row["state"]}", "Detalii contract");
            };
            action.Click += (s, e) =>
            {
                if (grid.CurrentRow?.DataBoundItem is not DataRowView row) return;
                if (row["closed_at"] != DBNull.Value) { MessageBox.Show("Contractul este deja închis."); return; }
                bool rental = row["kind"].ToString() == "Rental";
                int paid = Convert.ToInt32(row["installments_paid"]);
                decimal next = 0;
                if (!rental)
                    next = paid + 1 == Convert.ToInt32(row["months"])
                        ? Convert.ToDecimal(row["total_cost"]) - Convert.ToDecimal(row["down_payment"]) - paid * Convert.ToDecimal(row["monthly_payment"])
                        : Convert.ToDecimal(row["monthly_payment"]);
                if (MessageBox.Show(rental ? "Confirmați returnarea mașinii? Va reveni la Available." : $"Confirmați încasarea ratei {paid + 1}: {next:N2} €?",
                    "Confirmare", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                try { new ContractRepository().RegisterPaymentOrReturn(Convert.ToInt32(row["id"]), paid); }
                catch (InvalidOperationException ex) { MessageBox.Show(ex.Message, "Contract modificat"); }
                catch (Exception) { MessageBox.Show("Operațiunea nu a putut fi salvată.", "Eroare"); }
                Reload();
            };
        }
        private void Reload()
        {
            try { grid.DataSource = new ContractRepository().GetAll(); }
            catch (Exception) { MessageBox.Show("Contractele nu au putut fi încărcate. Verificați conexiunea la baza de date.", "Eroare"); }
        }
    }
}
