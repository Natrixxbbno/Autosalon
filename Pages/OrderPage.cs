using System.Drawing;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using AutoSalon.Database.Models;
using AutoSalon.Database.Repositories;

namespace AutoSalon.Pages
{
    public class OrderPage : Form
    {
        private readonly TextBox lastName = new() { MaxLength = 100 };
        private readonly TextBox firstName = new() { MaxLength = 100 };
        private readonly TextBox phone = new() { MaxLength = 15 };
        private readonly TextBox email = new() { MaxLength = 255 };
        private readonly ComboBox category = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly ComboBox product = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly ComboBox paymentMethod = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly NumericUpDown quantity = new() { Minimum = 1, Maximum = 1000, Value = 1 };
        private readonly Button submit = new() { Text = "Transmite comanda", AutoSize = true };
        private List<Car> cars = new();
        private readonly bool tradeInMode;
        private readonly ComboBox offeredMake = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly TextBox offeredModel = new() { MaxLength = 100 };
        private readonly TextBox offeredColor = new() { MaxLength = 50 };
        private readonly TextBox offeredRegistration = new() { MaxLength = 20 };
        private readonly NumericUpDown offeredYear = new() { Minimum = 1900, Maximum = DateTime.Now.Year + 1, Value = DateTime.Now.Year };
        private readonly NumericUpDown offeredMileage = new() { Maximum = 10000000, ThousandsSeparator = false };
        private readonly NumericUpDown offeredValue = new() { Maximum = 99999999.99m, DecimalPlaces = 2 };
        private readonly Label tradeSummary = new() { Dock = DockStyle.Fill, ForeColor = Color.FromArgb(90, 45, 170), Padding = new Padding(6), Font = new Font("Segoe UI", 11, FontStyle.Bold) };

        public OrderPage(bool tradeIn = false)
        {
            tradeInMode = tradeIn;
            Text = "AutoSalon - Comandă nouă";
            ClientSize = new Size(540, 530);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Font = new Font("Segoe UI", 10);
            BackColor = Color.FromArgb(241, 244, 248);
            if (tradeInMode)
            {
                Text = "AutoSalon - Trade-in";
                ClientSize = new Size(720, Math.Min(860, Screen.PrimaryScreen!.WorkingArea.Height - 80));
                submit.Text = "Transmite comanda trade-in";
                quantity.Value = 1;
                quantity.Enabled = false;
            }

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(24), ColumnCount = 2, RowCount = tradeInMode ? 17 : 9
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62));
            void AddField(string label, Control input, int row)
            {
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 49));
                layout.Controls.Add(new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left }, 0, row);
                input.Dock = DockStyle.Fill;
                input.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                input.TabIndex = row;
                layout.Controls.Add(input, 1, row);
            }
            AddField("Nume", lastName, 0);
            AddField("Prenume", firstName, 1);
            AddField("Nr. de contact", phone, 2);
            AddField("Email", email, 3);
            AddField("Categorie (marcă)", category, 4);
            AddField("Produs (automobil)", product, 5);
            AddField("Cantitate", quantity, 6);
            paymentMethod.Items.AddRange(new object[] { "Cash", "Card" });
            paymentMethod.SelectedIndex = -1;
            AddField("Metodă de plată", paymentMethod, 7);
            int submitRow = 8;
            if (tradeInMode)
            {
                AddField("Marca mașinii oferite", offeredMake, 8);
                AddField("Modelul mașinii oferite", offeredModel, 9);
                AddField("Anul fabricației", offeredYear, 10);
                AddField("Kilometraj (km)", offeredMileage, 11);
                AddField("Culoare", offeredColor, 12);
                AddField("Nr. înmatriculare", offeredRegistration, 13);
                AddField("Evaluare manuală (€)", offeredValue, 14);
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 110));
                layout.Controls.Add(tradeSummary, 0, 15);
                layout.SetColumnSpan(tradeSummary, 2);
                submitRow = 16;
                offeredValue.ValueChanged += (s, e) => UpdateTradeSummary();
                product.SelectedIndexChanged += (s, e) => UpdateTradeSummary();
                UpdateTradeSummary();
            }
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 49));
            layout.Controls.Add(submit, 1, submitRow);
            submit.TabIndex = submitRow;
            var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            scroll.Controls.Add(layout);
            Controls.Add(scroll);
            AcceptButton = submit;

            phone.KeyPress += (s, e) =>
                e.Handled = !char.IsControl(e.KeyChar) && (e.KeyChar < '0' || e.KeyChar > '9');
            // Also reject non-digit content pasted from the clipboard.
            phone.TextChanged += (s, e) =>
            {
                string digits = Regex.Replace(phone.Text, "[^0-9]", "");
                if (digits != phone.Text)
                {
                    int position = phone.SelectionStart;
                    phone.Text = digits;
                    phone.SelectionStart = Math.Min(position, digits.Length);
                }
            };
            category.SelectedIndexChanged += (s, e) => LoadProducts();
            submit.Click += SubmitOrder;
            Load += (s, e) => LoadCategories();
        }

        private void LoadCategories()
        {
            try
            {
                cars = new CarRepository().GetAll().Where(c => c.Status?.Name == "Available").ToList();
                category.DisplayMember = "Name";
                category.DataSource = new ManufacturerRepository().GetAll().ToList();
                if (tradeInMode)
                {
                    offeredMake.DisplayMember = "Name";
                    offeredMake.DataSource = new ManufacturerRepository().GetAll().ToList();
                    offeredMake.SelectedIndex = -1;
                }
                category.SelectedIndex = -1;
                LoadProducts();
            }

            catch (Exception)
            {
                submit.Enabled = false;
                MessageBox.Show("Nu s-au putut încărca produsele. Închideți formularul și încercați din nou.",
                    "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProducts()
        {
            product.DataSource = null;
            product.Items.Clear();
            if (category.SelectedItem is Manufacturer manufacturer)
            {
                foreach (var car in cars.Where(c => c.ManufacturerId == manufacturer.Id))
                    product.Items.Add(new ProductChoice(car.Id, $"{car.Model} ({car.Year}) — {car.RegistrationNumber}"));
            }
            product.SelectedIndex = -1;
            product.Enabled = product.Items.Count > 0;
        }

        private void UpdateTradeSummary()
        {
            var selected = product.SelectedItem as ProductChoice;
            var car = cars.FirstOrDefault(c => c.Id == selected?.Id);
            if (car == null) { tradeSummary.Text = "Selectați mașina dorită pentru calculul diferenței."; return; }
            decimal balance = car.Price - offeredValue.Value;
            tradeSummary.Text = $"Preț mașină dorită: {car.Price:N2} €\nMașina oferită: {offeredValue.Value:N2} €\n" +
                (balance >= 0 ? $"De adăugat: {balance:N2} €" : $"Diferență de restituit clientului: {-balance:N2} €");
        }

        private void SubmitOrder(object? sender, EventArgs e)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(lastName.Text)) errors.Add("Completați numele.");
            if (string.IsNullOrWhiteSpace(firstName.Text)) errors.Add("Completați prenumele.");
            if (!Regex.IsMatch(phone.Text, "^[0-9]{9,15}$")) errors.Add("Numărul de contact trebuie să conțină între 9 și 15 cifre.");
            string address = email.Text.Trim();
            if (!MailAddress.TryCreate(address, out var parsed) || parsed.Address != address ||
                !Regex.IsMatch(address, @"^[^\s@]+@[^\s@]+\.[^\s@]+$"))
                errors.Add("Introduceți o adresă de email validă (ex.: nume@exemplu.ro).");
            if (category.SelectedItem is not Manufacturer) errors.Add("Selectați categoria.");
            if (product.SelectedItem is not ProductChoice) errors.Add("Selectați produsul.");
            if (paymentMethod.SelectedItem is not string) errors.Add("Selectați metoda de plată: Cash sau Card.");
            TradeInDetails? tradeDetails = null;
            if (tradeInMode)
            {
                if (offeredMake.SelectedItem is not Manufacturer) errors.Add("Selectați marca mașinii oferite.");
                if (string.IsNullOrWhiteSpace(offeredModel.Text)) errors.Add("Completați modelul mașinii oferite.");
                if (string.IsNullOrWhiteSpace(offeredColor.Text)) errors.Add("Completați culoarea mașinii oferite.");
                if (string.IsNullOrWhiteSpace(offeredRegistration.Text)) errors.Add("Completați numărul de înmatriculare.");
                if (offeredValue.Value <= 0) errors.Add("Introduceți o evaluare manuală mai mare decât zero.");
                var desired = cars.FirstOrDefault(c => c.Id == (product.SelectedItem as ProductChoice)?.Id);
                if (desired != null && offeredMake.SelectedItem is Manufacturer make)
                    tradeDetails = new TradeInDetails(make.Id, offeredModel.Text.Trim(), (int)offeredYear.Value,
                        (int)offeredMileage.Value, offeredColor.Text.Trim(), offeredRegistration.Text.Trim().ToUpperInvariant(),
                        offeredValue.Value, desired.Price);
            }
            if (!int.TryParse(quantity.Text, out int requestedQuantity) || requestedQuantity < 1 || requestedQuantity > 1000) errors.Add("Cantitatea trebuie să fie un număr întreg între 1 și 1000.");
            if (errors.Count > 0)
            {
                MessageBox.Show(string.Join(Environment.NewLine, errors), "Verificați datele", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            submit.Enabled = false;
            try
            {
                int id = new OrderRepository().Add(lastName.Text.Trim(), firstName.Text.Trim(), phone.Text,
                    address, ((ProductChoice)product.SelectedItem).Id, requestedQuantity, (string)paymentMethod.SelectedItem, tradeDetails);
                MessageBox.Show($"Comanda nr. {id} a fost transmisă cu succes!", "Comandă salvată", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Mașină indisponibilă", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LoadCategories();
            }
            catch (Exception)
            {
                MessageBox.Show("Comanda nu a putut fi salvată. Verificați conexiunea la baza de date și disponibilitatea produsului, apoi încercați din nou.",
                    "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { submit.Enabled = true; }
        }

        private sealed record ProductChoice(int Id, string Label)
        {
            public override string ToString() => Label;
        }
    }
}
