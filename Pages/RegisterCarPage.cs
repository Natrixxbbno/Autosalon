using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using AutoSalon.Database.Models;
using AutoSalon.Database.Repositories;
using System.Collections.Generic;
using Npgsql;

namespace AutoSalon.Pages
{
    public class RegisterCarPage : Form
    {
        private ComboBox cmbManufacturer;
        private TextBox txtModel;
        private NumericUpDown numYear;
        private TextBox txtColor;
        private NumericUpDown numPrice;
        private TextBox txtRegistrationNumber;
        private DateTimePicker dtpPurchaseDate;
        private Button btnSave;
        private Button btnCancel;
        private Label lblManufacturer;
        private Label lblModel;
        private Label lblYear;
        private Label lblColor;
        private Label lblPrice;
        private Label lblRegistrationNumber;
        private Label lblPurchaseDate;

        public RegisterCarPage()
        {
            InitializeComponents();
            LoadManufacturers();
        }

        private void InitializeComponents()
        {
            this.Text = "Регистрация нового автомобиля";
            this.Size = new Size(500, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(235, 235, 235);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Создаем и настраиваем элементы управления
            lblManufacturer = new Label
            {
                Text = "Производитель:",
                Location = new Point(20, 20),
                Size = new Size(200, 20)
            };

            cmbManufacturer = new ComboBox
            {
                Location = new Point(20, 45),
                Size = new Size(440, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            lblModel = new Label
            {
                Text = "Модель:",
                Location = new Point(20, 80),
                Size = new Size(200, 20)
            };

            txtModel = new TextBox
            {
                Location = new Point(20, 105),
                Size = new Size(440, 25)
            };

            lblYear = new Label
            {
                Text = "Год выпуска:",
                Location = new Point(20, 140),
                Size = new Size(200, 20)
            };

            numYear = new NumericUpDown
            {
                Location = new Point(20, 165),
                Size = new Size(440, 25),
                Minimum = 1900,
                Maximum = DateTime.Now.Year,
                Value = DateTime.Now.Year
            };

            lblColor = new Label
            {
                Text = "Цвет:",
                Location = new Point(20, 200),
                Size = new Size(200, 20)
            };

            txtColor = new TextBox
            {
                Location = new Point(20, 225),
                Size = new Size(440, 25)
            };

            lblPrice = new Label
            {
                Text = "Цена:",
                Location = new Point(20, 260),
                Size = new Size(200, 20)
            };

            numPrice = new NumericUpDown
            {
                Location = new Point(20, 285),
                Size = new Size(440, 25),
                Minimum = 1000,
                Maximum = 1000000000,
                DecimalPlaces = 2,
                ThousandsSeparator = true,
                Value = 1000
            };

            lblRegistrationNumber = new Label
            {
                Text = "Регистрационный номер:",
                Location = new Point(20, 320),
                Size = new Size(200, 20)
            };

            txtRegistrationNumber = new TextBox
            {
                Location = new Point(20, 345),
                Size = new Size(440, 25)
            };

            lblPurchaseDate = new Label
            {
                Text = "Дата покупки:",
                Location = new Point(20, 380),
                Size = new Size(200, 20)
            };

            dtpPurchaseDate = new DateTimePicker
            {
                Location = new Point(20, 405),
                Size = new Size(440, 25),
                Format = DateTimePickerFormat.Short
            };

            btnSave = new Button
            {
                Text = "Сохранить",
                Location = new Point(20, 460),
                Size = new Size(200, 40),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btnCancel = new Button
            {
                Text = "Отмена",
                Location = new Point(260, 460),
                Size = new Size(200, 40),
                BackColor = Color.FromArgb(200, 200, 200),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            // Добавляем обработчики событий
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => this.Close();

            // Добавляем элементы управления на форму
            this.Controls.AddRange(new Control[] {
                lblManufacturer, cmbManufacturer,
                lblModel, txtModel,
                lblYear, numYear,
                lblColor, txtColor,
                lblPrice, numPrice,
                lblRegistrationNumber, txtRegistrationNumber,
                lblPurchaseDate, dtpPurchaseDate,
                btnSave, btnCancel
            });
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

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                var car = new Car
                {
                    ManufacturerId = ((Manufacturer)cmbManufacturer.SelectedItem).Id,
                    Model = txtModel.Text.Trim(),
                    Year = (int)numYear.Value,
                    Color = txtColor.Text.Trim(),
                    Price = numPrice.Value,
                    RegistrationNumber = txtRegistrationNumber.Text.Trim(),
                    PurchaseDate = dtpPurchaseDate.Value
                };

                var carRepository = new CarRepository();
                try
                {
                    if (carRepository.Add(car))
                    {
                        MessageBox.Show("Автомобиль успешно зарегистрирован!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    // Else case is now handled by the catch block as the repository throws
                }
                catch (Npgsql.PostgresException pgEx)
                {
                    // Обработка специфических ошибок PostgreSQL
                    MessageBox.Show($"Ошибка базы данных при регистрации автомобиля: {pgEx.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    // Общая обработка других ошибок
                    MessageBox.Show($"Ошибка при регистрации автомобиля: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidateInput()
        {
            if (cmbManufacturer.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите производителя.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtModel.Text))
            {
                MessageBox.Show("Пожалуйста, введите модель автомобиля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtColor.Text))
            {
                MessageBox.Show("Пожалуйста, введите цвет автомобиля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtRegistrationNumber.Text))
            {
                MessageBox.Show("Пожалуйста, введите регистрационный номер.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (numPrice.Value < 1000)
            {
                MessageBox.Show("Цена автомобиля должна быть не менее 1000.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
    }
} 