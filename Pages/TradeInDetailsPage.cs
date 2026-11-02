using System.Data;
using System.Drawing;
using System.Windows.Forms;
using AutoSalon.Database.Repositories;

namespace AutoSalon.Pages
{
    public sealed class TradeInDetailsPage : Form
    {
        public TradeInDetailsPage(DataRowView order)
        {
            Text = $"Trade-in — Comanda nr. {order["id"]}";
            ClientSize = new Size(580, 460);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Font = new Font("Segoe UI", 10);
            BackColor = Color.FromArgb(243, 244, 249);
            decimal originalPrice = Convert.ToDecimal(order["desired_car_price"]);
            decimal valuation = Convert.ToDecimal(order["trade_in_value"]);
            bool completed = Convert.ToBoolean(order["completed"]);
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(24), ColumnCount = 1, RowCount = 6 };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            foreach (int height in new[] { 140, 32, 44, 86, 36, 52 })
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, height));
            layout.Controls.Add(new Label
            {
                Dock = DockStyle.Fill,
                Text = $"Mașină oferită: {order["trade_in_model"]} ({order["trade_in_year"]})\n" +
                    $"Înmatriculare: {order["trade_in_registration"]} • {order["trade_in_mileage"]} km\n" +
                    $"Evaluare manuală: {valuation:N2} €\n\n" +
                    $"Mașină dorită: {order["category"]} {order["product"]} — {order["registration_number"]}"
            }, 0, 0);
            layout.Controls.Add(new Label { Text = "Preț negociat pentru mașina dorită (€)", AutoSize = true }, 0, 1);
            var price = new NumericUpDown
            {
                Maximum = 9999999999.99m, Minimum = 0.01m, DecimalPlaces = 2,
                Value = originalPrice, Width = 260, Enabled = !completed
            };
            layout.Controls.Add(price, 0, 2);
            var balance = new Label { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = Color.FromArgb(100, 55, 190) };
            void Recalculate()
            {
                decimal difference = price.Value - valuation;
                balance.Text = $"Mașina oferită: {valuation:N2} €\n" +
                    (difference >= 0 ? $"Clientul adaugă: {difference:N2} €" : $"De restituit clientului: {-difference:N2} €");
            }
            price.ValueChanged += (s, e) => Recalculate();
            Recalculate();
            layout.Controls.Add(balance, 0, 3);
            layout.Controls.Add(new Label { Text = completed ? "Comandă finalizată — prețul nu mai poate fi modificat." : "Prețul negociat se salvează pentru această comandă.", Dock = DockStyle.Fill }, 0, 4);
            var save = new Button { Text = "Salvează prețul", AutoSize = true, Enabled = !completed };
            save.Click += (s, e) =>
            {
                save.Enabled = false;
                try
                {
                    new OrderRepository().UpdateTradeInPrice(Convert.ToInt32(order["id"]), originalPrice, price.Value);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "Comandă modificată", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception)
                {
                    MessageBox.Show("Prețul nu a putut fi salvat. Verificați conexiunea și încercați din nou.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally { save.Enabled = true; }
            };
            layout.Controls.Add(save, 0, 5);
            Controls.Add(layout);
            AcceptButton = save;
        }
    }
}
