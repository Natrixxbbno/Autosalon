using System;
using System.Drawing;
using System.Windows.Forms;
using AutoSalon.Database.Models;
using AutoSalon.Database.Repositories;

namespace AutoSalon.Pages
{
    public class EditCarPage : Form
    {
        private ComboBox cmbManufacturer;
        private TextBox txtModel;
        private NumericUpDown numYear;
        private TextBox txtColor;
        private NumericUpDown numPrice;
        private TextBox txtRegistrationNumber;
        private NumericUpDown numMileage;
        private ComboBox cmbStatus;
        private DateTimePicker dtpPurchaseDate;
        private Button btnSave;
        private Button btnCancel;
        private Car car;

        public EditCarPage(Car car)
        {
            this.car = car;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.AutoScroll = true;
            InitializeComponents();
            LoadManufacturers();
            LoadStatuses();
            FillFields();
        }

        private void InitializeComponents()
        {
            this.Text = "Редактирование автомобиля";
            this.Size = new Size(500, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(235, 235, 235);
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.AutoScroll = true;

            Panel scrollPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(480, 700),
                AutoScroll = true,
                Dock = DockStyle.Fill
            };

            Label lblManufacturer = new Label { Text = "Производитель:", Location = new Point(20, 20), Size = new Size(200, 20) };
            cmbManufacturer = new ComboBox { Location = new Point(20, 45), Size = new Size(440, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            Label lblModel = new Label { Text = "Модель:", Location = new Point(20, 80), Size = new Size(200, 20) };
            txtModel = new TextBox { Location = new Point(20, 105), Size = new Size(440, 25) };
            Label lblYear = new Label { Text = "Год выпуска:", Location = new Point(20, 140), Size = new Size(200, 20) };
            numYear = new NumericUpDown { Location = new Point(20, 165), Size = new Size(440, 25), Minimum = 1900, Maximum = DateTime.Now.Year, Value = DateTime.Now.Year };
            Label lblColor = new Label { Text = "Цвет:", Location = new Point(20, 200), Size = new Size(200, 20) };
            txtColor = new TextBox { Location = new Point(20, 225), Size = new Size(440, 25) };
            Label lblPrice = new Label { Text = "Цена:", Location = new Point(20, 260), Size = new Size(200, 20) };
            numPrice = new NumericUpDown { Location = new Point(20, 285), Size = new Size(440, 25), Minimum = 1000, Maximum = 1000000000, DecimalPlaces = 2, ThousandsSeparator = true, Value = 1000 };
            Label lblRegistrationNumber = new Label { Text = "Регистрационный номер:", Location = new Point(20, 320), Size = new Size(200, 20) };
            txtRegistrationNumber = new TextBox { Location = new Point(20, 345), Size = new Size(440, 25) };
            Label lblMileage = new Label { Text = "Пробег:", Location = new Point(20, 380), Size = new Size(200, 20) };
            numMileage = new NumericUpDown { Location = new Point(20, 405), Size = new Size(440, 25), Minimum = 0, Maximum = 1000000, Value = 0, ThousandsSeparator = true };
            Label lblStatus = new Label { Text = "Статус:", Location = new Point(20, 440), Size = new Size(200, 20) };
            cmbStatus = new ComboBox { Location = new Point(20, 465), Size = new Size(440, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            Label lblPurchaseDate = new Label { Text = "Дата покупки:", Location = new Point(20, 500), Size = new Size(200, 20) };
            dtpPurchaseDate = new DateTimePicker { Location = new Point(20, 525), Size = new Size(440, 25), Format = DateTimePickerFormat.Short };
            btnSave = new Button { Text = "Сохранить", Location = new Point(20, 560), Size = new Size(200, 40), BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnCancel = new Button { Text = "Отмена", Location = new Point(260, 560), Size = new Size(200, 40), BackColor = Color.FromArgb(200, 200, 200), ForeColor = Color.Black, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => this.Close();
            scrollPanel.Controls.AddRange(new Control[] { lblManufacturer, cmbManufacturer, lblModel, txtModel, lblYear, numYear, lblColor, txtColor, lblPrice, numPrice, lblRegistrationNumber, txtRegistrationNumber, lblMileage, numMileage, lblStatus, cmbStatus, lblPurchaseDate, dtpPurchaseDate, btnSave, btnCancel });
            this.Controls.Add(scrollPanel);
        }

        private void LoadManufacturers()
        {
            var manufacturerRepository = new ManufacturerRepository();
            var manufacturers = manufacturerRepository.GetAll();
            cmbManufacturer.Items.Clear();
            foreach (var manufacturer in manufacturers)
            {
                cmbManufacturer.Items.Add(manufacturer);
            }
            cmbManufacturer.DisplayMember = "Name";
            cmbManufacturer.ValueMember = "Id";
        }

        private void LoadStatuses()
        {
            var statusRepository = new CarStatusRepository();
            var statuses = statusRepository.GetAll();
            cmbStatus.Items.Clear();
            foreach (var status in statuses)
            {
                cmbStatus.Items.Add(status);
            }
            cmbStatus.DisplayMember = "Name";
            cmbStatus.ValueMember = "Id";
        }

        private void FillFields()
        {
            if (car == null) return;
            cmbManufacturer.SelectedItem = cmbManufacturer.Items.Cast<Manufacturer>().FirstOrDefault(m => m.Id == car.ManufacturerId);
            txtModel.Text = car.Model;
            numYear.Value = car.Year;
            txtColor.Text = car.Color;
            numPrice.Value = car.Price;
            txtRegistrationNumber.Text = car.RegistrationNumber;
            numMileage.Value = car.Mileage;
            cmbStatus.SelectedItem = cmbStatus.Items.Cast<CarStatus>().FirstOrDefault(s => s.Id == car.StatusId);
            dtpPurchaseDate.Value = car.PurchaseDate;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;
            car.Manufacturer = cmbManufacturer.SelectedItem as Manufacturer;
            car.ManufacturerId = (cmbManufacturer.SelectedItem as Manufacturer)?.Id ?? 0;
            car.Model = txtModel.Text.Trim();
            car.Year = (int)numYear.Value;
            car.Color = txtColor.Text.Trim();
            car.Price = numPrice.Value;
            car.RegistrationNumber = txtRegistrationNumber.Text.Trim();
            car.Mileage = (int)numMileage.Value;
            car.Status = cmbStatus.SelectedItem as CarStatus;
            car.StatusId = (cmbStatus.SelectedItem as CarStatus)?.Id ?? 1;
            car.PurchaseDate = dtpPurchaseDate.Value;
            var carRepository = new CarRepository();
            if (carRepository.Update(car))
            {
                MessageBox.Show("Изменения успешно сохранены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Ошибка при сохранении изменений.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            // Можно добавить валидацию по аналогии с RegisterCarPage
            return true;
        }
    }
} 