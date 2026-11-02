using System.Drawing;
using System.Windows.Forms;
using System.Net.Mail;
using System.Text.RegularExpressions;
using AutoSalon.Database.Models;
using AutoSalon.Database.Repositories;

namespace AutoSalon.Pages
{
    public sealed class ContractPage : Form
    {
        public ContractPage(bool rental)
        {
            Text = rental ? "AutoSalon — Închiriere auto" : "AutoSalon — Credit auto";
            ClientSize = new Size(680, Math.Min(780, Screen.PrimaryScreen!.WorkingArea.Height - 80));
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 10); BackColor = Color.FromArgb(243, 244, 249);
            var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            var layout = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, Padding = new Padding(24) };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
            scroll.Controls.Add(layout); Controls.Add(scroll);
            int row = 0;
            void Field(string title, Control control, int height = 46)
            {
                layout.RowCount = row + 1;
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, height));
                layout.Controls.Add(new Label { Text = title, AutoSize = true, Anchor = AnchorStyles.Left }, 0, row);
                control.Anchor = AnchorStyles.Left | AnchorStyles.Right; control.TabIndex = row;
                layout.Controls.Add(control, 1, row++);
            }
            var last = new TextBox { MaxLength = 100 }; var first = new TextBox { MaxLength = 100 };
            var phone = new TextBox { MaxLength = 15 }; var email = new TextBox { MaxLength = 255 };
            var car = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, DropDownWidth = 550 };
            var payment = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            payment.Items.AddRange(new object[] { "Cash", "Card" });
            var down = new NumericUpDown { Maximum = 99999999.99m, DecimalPlaces = 2 };
            var rate = new NumericUpDown { Maximum = 100, DecimalPlaces = 2 };
            var months = new NumericUpDown { Minimum = 1, Maximum = 120, Value = 12 };
            var start = new DateTimePicker { Format = DateTimePickerFormat.Short, Value = DateTime.Today, MinDate = DateTime.Today };
            var end = new DateTimePicker { Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddDays(1), MinDate = DateTime.Today.AddDays(1) };
            var daily = new NumericUpDown { Maximum = 99999999.99m, DecimalPlaces = 2 };
            var summary = new Label { Dock = DockStyle.Fill, ForeColor = Color.FromArgb(100, 55, 190), Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            var save = new Button { Text = rental ? "Înregistrează închirierea" : "Înregistrează creditul", AutoSize = true };
            Field("Nume", last); Field("Prenume", first); Field("Nr. de contact", phone);
            Field("Email", email); Field("Automobil", car); Field("Metodă de plată", payment);
            if (rental)
            {
                Field("Data preluării", start); Field("Data returnării", end); Field("Tarif pe zi (€)", daily);
            }
            else
            {
                Field("Avans (€)", down); Field("Dobândă anuală (%)", rate); Field("Perioadă (luni)", months);
            }
            layout.RowCount = row + 1; layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 170));
            layout.Controls.Add(summary, 0, row++); layout.SetColumnSpan(summary, 2);
            Field("", save, 52); AcceptButton = save;
            phone.KeyPress += (s, e) => e.Handled = !char.IsControl(e.KeyChar) && (e.KeyChar < '0' || e.KeyChar > '9');
            phone.TextChanged += (s, e) =>
            {
                string digits = Regex.Replace(phone.Text, "[^0-9]", "");
                if (digits != phone.Text) { phone.Text = digits; phone.SelectionStart = digits.Length; }
            };
            void Recalculate()
            {
                if (car.SelectedItem is not CarChoice selected) { summary.Text = "Selectați un automobil disponibil."; return; }
                try
                {
                    if (rental)
                    {
                        int days = (end.Value.Date - start.Value.Date).Days;
                        summary.Text = days < 1 ? "Returnarea trebuie să fie după data preluării." :
                            $"Perioadă: {days} zile × {daily.Value:N2} €\nTotal închiriere: {days * daily.Value:N2} €\n\n" +
                            "Data returnării nu se taxează.\nMașina rămâne rezervată până la returnarea înregistrată.";
                    }
                    else
                    {
                        var calculation = CreditCalculation.Calculate(selected.Price, down.Value, rate.Value, (int)months.Value);
                        summary.Text = $"Preț auto: {selected.Price:N2} € • Finanțat: {calculation.Principal:N2} €\n" +
                            $"Rată lunară: {calculation.Monthly:N2} € • Ultima: {calculation.Last:N2} €\n" +
                            $"Dobândă totală: {calculation.Interest:N2} €\nTotal cu avans: {calculation.Total:N2} €\n" +
                            "Rate lunare fixe, fără comisioane suplimentare.";
                    }
                }
                catch (ArgumentException ex) { summary.Text = ex.Message; }
            }
            car.SelectedIndexChanged += (s, e) => Recalculate();
            foreach (var number in new[] { down, rate, months, daily }) number.ValueChanged += (s, e) => Recalculate();
            start.ValueChanged += (s, e) => Recalculate(); end.ValueChanged += (s, e) => Recalculate();
            Load += (s, e) =>
            {
                try
                {
                    foreach (var item in new CarRepository().GetAll().Where(c => c.Status?.Name == "Available"))
                        car.Items.Add(new CarChoice(item.Id, item.Price, $"{item.Manufacturer.Name} {item.Model} — {item.RegistrationNumber} — {item.Price:N2} €"));
                    if (car.Items.Count == 0) { save.Enabled = false; summary.Text = "Nu există automobile disponibile."; }
                    else Recalculate();
                }
                catch (Exception) { save.Enabled = false; MessageBox.Show("Automobilele nu au putut fi încărcate.", "Eroare"); }
            };
            save.Click += (s, e) =>
            {
                var errors = new List<string>();
                if (string.IsNullOrWhiteSpace(last.Text)) errors.Add("Completați numele.");
                if (string.IsNullOrWhiteSpace(first.Text)) errors.Add("Completați prenumele.");
                if (!Regex.IsMatch(phone.Text, "^[0-9]{9,15}$")) errors.Add("Telefonul trebuie să conțină 9–15 cifre.");
                if (!MailAddress.TryCreate(email.Text.Trim(), out var address) || address.Address != email.Text.Trim()
                    || !Regex.IsMatch(email.Text.Trim(), @"^[^\s@]+@[^\s@]+\.[^\s@]+$")) errors.Add("Introduceți un email valid.");
                if (car.SelectedItem is not CarChoice) errors.Add("Selectați automobilul.");
                if (payment.SelectedItem is not string) errors.Add("Selectați metoda de plată.");
                if (errors.Count > 0) { MessageBox.Show(string.Join("\n", errors), "Verificați datele"); return; }
                save.Enabled = false;
                try
                {
                    var selected = (CarChoice)car.SelectedItem;
                    int id = new ContractRepository().Add(new ContractRequest(rental, last.Text, first.Text, phone.Text,
                        email.Text.Trim(), selected.Id, selected.Price, (string)payment.SelectedItem,
                        down.Value, rate.Value, (int)months.Value, start.Value.Date, end.Value.Date, daily.Value));
                    MessageBox.Show($"Contractul nr. {id} a fost înregistrat.", "Succes"); DialogResult = DialogResult.OK; Close();
                }
                catch (ArgumentException ex) { MessageBox.Show(ex.Message, "Verificați datele"); }
                catch (InvalidOperationException ex) { MessageBox.Show(ex.Message, "Automobil indisponibil"); }
                catch (Exception) { MessageBox.Show("Contractul nu a putut fi salvat. Verificați conexiunea și încercați din nou.", "Eroare"); }
                finally { save.Enabled = true; }
            };
        }
        private sealed record CarChoice(int Id, decimal Price, string Label) { public override string ToString() => Label; }
    }
}
