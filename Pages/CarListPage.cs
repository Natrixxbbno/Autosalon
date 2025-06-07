using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using AutoSalon.Database.Models;
using AutoSalon.Database;
using Npgsql;
using System.Data;
using AutoSalon.Database.Repositories;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace AutoSalon.Pages
{
    public class CarListPage : Form
    {
        private DataGridView dgvCars;
        private Button btnBack;
        private TextBox txtSearch;
        private ComboBox cmbManufacturerFilter;
        private ComboBox cmbStatusFilter;
        private Panel topPanel;
        private Panel bottomPanel;
        private Label lblTitle;
        private string currentSortColumn = "manufacturer_name";
        private bool isAscending = true;
        private bool isPriceAscending = true;
        private Panel sidebarPanel;
        private Button btnDashboard;
        private Button btnCarInventory;
        private Button btnRegisterNewCar;
        private Button btnAnalytics;
        private Button btnReports;
        private Label logoIcon;
        private Label lblLogo;

        public CarListPage()
        {
            InitializeComponents();
            LoadCars();
            LoadManufacturerFilter();
            LoadStatusFilter();
        }

        private void InitializeComponents()
        {
            this.Text = "Автосалон - Управление автомобилями";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.Font = new Font("Segoe UI", 9F);

            // Верхняя панель
            topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.Transparent
            };

            lblTitle = new Label
            {
                Text = "Управление автомобилями",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Новый подзаголовок
            var lblSubtitle = new Label
            {
                Text = "Управляйте своим автопарком", // или "Manage your car collection"
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(22, 52), // чуть ниже основного заголовка
                AutoSize = true
            };

            // Панель поиска и фильтрации
            Panel searchPanel = new Panel
            {
                Location = new Point(20, 100),
                Size = new Size(1160, 50),
                BackColor = Color.White
            };

            // Label для поиска
            var lblSearch = new Label
            {
                Text = "Search",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(10, 0),
                AutoSize = true
            };
            // Label для производителя
            var lblMake = new Label
            {
                Text = "Make",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(200, 0),
                AutoSize = true
            };
            // Label для статуса
            var lblStatus = new Label
            {
                Text = "Status",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(420, 0),
                AutoSize = true
            };

            // --- Минималистичные панели для поиска и фильтров ---
            int panelHeight = 28;
            int panelWidth = 180;

            // Панель для поиска
            var searchBoxPanel = new Panel
            {
                Location = new Point(10, 22),
                Size = new Size(panelWidth, panelHeight),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtSearch = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Location = new Point(2, 4), // левее, чтобы текст помещался
                Size = new Size(340, 20),
                PlaceholderText = "Search by make or model...",
                Font = new Font("Segoe UI", 11F, FontStyle.Regular),
                ForeColor = Color.Gray
            };
            searchBoxPanel.Controls.Add(txtSearch);

            // Панель для фильтра производителя
            var makePanel = new Panel
            {
                Location = new Point(200, 18),
                Size = new Size(panelWidth, panelHeight + 18),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            var lblMakeInside = new Label
            {
                Text = "Make",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(8, 0),
                AutoSize = true
            };
            cmbManufacturerFilter = new ComboBox
            {
                Location = new Point(200, 14),
                Size = new Size(200, 32),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.Black
            };
            makePanel.Controls.Add(lblMakeInside);
            makePanel.Controls.Add(cmbManufacturerFilter);

            // Панель для фильтра статуса
            var statusPanel = new Panel
            {
                Location = new Point(420, 14),
                Size = new Size(panelWidth, panelHeight + 18),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            var lblStatusInside = new Label
            {
                Text = "Status",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(8, 0),
                AutoSize = true
            };
            cmbStatusFilter = new ComboBox
            {
                Location = new Point(420, 14),
                Size = new Size(200, 32),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.Black
            };
            statusPanel.Controls.Add(lblStatusInside);
            statusPanel.Controls.Add(cmbStatusFilter);

            // DataGridView
            dgvCars = new DataGridView
            {
                Location = new Point(20, 170),
                Size = new Size(1160, 500),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                Font = new Font("Segoe UI", 9F),
                RowTemplate = { Height = 40 },
                ColumnHeadersHeight = 45,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(0, 120, 215),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Padding = new Padding(5)
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(245, 245, 245)
                }
            };

            // Добавляем обработчики событий
            txtSearch.TextChanged += TxtSearch_TextChanged;
            cmbManufacturerFilter.SelectedIndexChanged += (s, e) => LoadCars();
            cmbStatusFilter.SelectedIndexChanged += (s, e) => LoadCars();

            // Настраиваем колонки DataGridView
            dgvCars.Columns.Add("Id", "ID");
            dgvCars.Columns["Id"].Visible = false;

            dgvCars.Columns.Add("Manufacturer", "Производитель");
            dgvCars.Columns.Add("Model", "Модель");
            dgvCars.Columns.Add("Year", "Год");
            dgvCars.Columns.Add("Color", "Цвет");
            dgvCars.Columns.Add("Mileage", "Пробег");
            dgvCars.Columns.Add("Price", "Цена");
            dgvCars.Columns.Add("RegistrationNumber", "Регистрационный номер");
            dgvCars.Columns.Add("Status", "Статус");

            // Добавляем обработчик сортировки
            dgvCars.ColumnHeaderMouseClick += DgvCars_ColumnHeaderMouseClick;

            // Кнопка редактирования
            var editButtonColumn = new DataGridViewButtonColumn
            {
                HeaderText = "Действие",
                Text = "✏️",
                UseColumnTextForButtonValue = true,
                Name = "EditColumn"
            };
            dgvCars.Columns.Add(editButtonColumn);

            // Кнопка удаления
            var deleteButtonColumn = new DataGridViewButtonColumn
            {
                HeaderText = "",
                Text = "🗑️",
                UseColumnTextForButtonValue = true,
                Name = "DeleteColumn"
            };
            dgvCars.Columns.Add(deleteButtonColumn);

            // Добавляем боковую панель
            sidebarPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = Color.FromArgb(245, 245, 245)
            };
            logoIcon = new Label
            {
                Text = "\uE804",
                Font = new Font("Segoe MDL2 Assets", 32F, FontStyle.Regular),
                ForeColor = Color.Black,
                Location = new Point(10, 18),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            lblLogo = new Label
            {
                Text = "CarAutosalon",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(68, 32),
                AutoSize = true
            };
            btnDashboard = new Button { Text = "\uD83D\uDCCA  Dashboard", Location = new Point(0, 80), Size = new Size(220, 50), FlatStyle = FlatStyle.Flat, BackColor = Color.Transparent, ForeColor = Color.Black, Font = new Font("Segoe UI", 12F, FontStyle.Regular), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(24, 0, 0, 0), Cursor = Cursors.Hand };
            btnCarInventory = new Button { Text = "\uD83D\uDE97  Car Inventory", Location = new Point(0, 130), Size = new Size(220, 50), FlatStyle = FlatStyle.Flat, BackColor = Color.Black, ForeColor = Color.White, Font = new Font("Segoe UI", 12F, FontStyle.Regular), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(24, 0, 0, 0), Cursor = Cursors.Hand };
            btnRegisterNewCar = new Button { Text = "\u2795  Register New Car", Location = new Point(0, 180), Size = new Size(220, 50), FlatStyle = FlatStyle.Flat, BackColor = Color.Transparent, ForeColor = Color.Black, Font = new Font("Segoe UI", 12F, FontStyle.Regular), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(24, 0, 0, 0), Cursor = Cursors.Hand };
            btnAnalytics = new Button { Text = "\u23F0  Analytics", Location = new Point(0, 230), Size = new Size(220, 50), FlatStyle = FlatStyle.Flat, BackColor = Color.Transparent, ForeColor = Color.Black, Font = new Font("Segoe UI", 12F, FontStyle.Regular), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(24, 0, 0, 0), Cursor = Cursors.Hand };
            btnReports = new Button { Text = "\uD83D\uDCC4  Reports", Location = new Point(0, 280), Size = new Size(220, 50), FlatStyle = FlatStyle.Flat, BackColor = Color.Transparent, ForeColor = Color.Black, Font = new Font("Segoe UI", 12F, FontStyle.Regular), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(24, 0, 0, 0), Cursor = Cursors.Hand };
            btnBack = new Button 
            { 
                Text = "⏏️  Выход",
                Dock = DockStyle.Bottom,
                Size = new Size(220, 50), 
                FlatStyle = FlatStyle.Flat, 
                BackColor = Color.Transparent, 
                Font = new Font("Segoe UI", 12F), 
                TextAlign = ContentAlignment.MiddleLeft, 
                Padding = new Padding(40, 0, 0, 0),
                Cursor = Cursors.Hand
            };

            // Добавляем обработчики для кнопок бокового меню
            btnDashboard.Click += (s, e) => { SetActiveSidebarButton(btnDashboard); MessageBox.Show("Раздел Dashboard в разработке", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information); };
            btnCarInventory.Click += (s, e) => { SetActiveSidebarButton(btnCarInventory); /* ничего не делаем, уже на этой странице */ };
            btnRegisterNewCar.Click += (s, e) => {
                SetActiveSidebarButton(btnRegisterNewCar);
                using (var registerForm = new RegisterCarPage())
                {
                    registerForm.ShowDialog();
                    LoadCars();
                }
            };
            btnAnalytics.Click += (s, e) => { SetActiveSidebarButton(btnAnalytics); using (var form = new AnalyticsPage()) form.ShowDialog(); };
            btnReports.Click += (s, e) => { SetActiveSidebarButton(btnReports); MessageBox.Show("Раздел Reports в разработке", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information); };
            btnBack.Click += (s, e) => this.Close();

            // После создания dgvCars:
            dgvCars.CellContentClick += dgvCars_CellContentClick;

            // Добавляем элементы управления на форму
            topPanel.Controls.Add(lblTitle);
            topPanel.Controls.Add(lblSubtitle);
            searchPanel.Controls.Add(lblSearch);
            searchPanel.Controls.Add(searchBoxPanel);
            searchPanel.Controls.Add(lblMake);
            searchPanel.Controls.Add(cmbManufacturerFilter);
            searchPanel.Controls.Add(lblStatus);
            searchPanel.Controls.Add(cmbStatusFilter);
            sidebarPanel.Controls.AddRange(new Control[] { logoIcon, lblLogo, btnDashboard, btnCarInventory, btnRegisterNewCar, btnAnalytics, btnReports, btnBack });
            
            this.Controls.AddRange(new Control[] { topPanel, searchPanel, dgvCars, sidebarPanel });

            // Сдвигаем остальные панели вправо
            topPanel.Location = new Point(220, 0);
            topPanel.Width -= 220;
            searchPanel.Location = new Point(240, 100);
            searchPanel.Width -= 220;
            dgvCars.Location = new Point(240, 170);
            dgvCars.Width -= 220;
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadCars();
        }

        private void LoadCars()
        {
            try
            {
                var carRepository = new CarRepository();
                var cars = carRepository.GetAll().ToList();

                // Применяем поиск
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    string searchText = txtSearch.Text;
                    cars = cars.Where(c => 
                        c.Manufacturer.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                        c.Model.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                        c.RegistrationNumber.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                if (cmbManufacturerFilter.SelectedIndex > 0)
                {
                    string selected = cmbManufacturerFilter.SelectedItem.ToString();
                    cars = cars.Where(c => c.Manufacturer.Name == selected).ToList();
                }
                if (cmbStatusFilter.SelectedIndex > 0)
                {
                    string selected = cmbStatusFilter.SelectedItem.ToString();
                    cars = cars.Where(c => c.Status != null && c.Status.Name == selected).ToList();
                }

                dgvCars.Rows.Clear();

                foreach (var car in cars)
                {
                    dgvCars.Rows.Add(
                        car.Id,
                        car.Manufacturer.Name,
                        car.Model,
                        car.Year,
                        car.Color,
                        car.Mileage.ToString("N0") + " км",
                        car.Price.ToString("N0") + " €",
                        car.RegistrationNumber,
                        car.Status?.Name ?? "",
                        "✏️",
                        "🗑️"
                    );
                }

                // Настраиваем стили для колонки с ценой
                if (dgvCars.Columns["Price"] != null)
                {
                    dgvCars.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvCars_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex == dgvCars.Columns["EditColumn"].Index)
            {
                var carId = Convert.ToInt32(dgvCars.Rows[e.RowIndex].Cells["Id"].Value);
                var carRepository = new CarRepository();
                var car = carRepository.GetById(carId);
                using (var editForm = new EditCarPage(car))
                {
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadStatusFilter();
                        LoadCars();
                    }
                }
                return;
            }
            if (e.ColumnIndex == dgvCars.Columns["DeleteColumn"].Index)
            {
                var carId = Convert.ToInt32(dgvCars.Rows[e.RowIndex].Cells["Id"].Value);
                var registrationNumber = dgvCars.Rows[e.RowIndex].Cells["RegistrationNumber"].Value.ToString();

                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить автомобиль с номером {registrationNumber}?",
                    "Подтверждение удаления",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    try
                {
                    var carRepository = new CarRepository();
                    if (carRepository.Delete(carId))
                    {
                        MessageBox.Show("Автомобиль успешно удален.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadCars();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении автомобиля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void LoadManufacturerFilter()
        {
            var repo = new ManufacturerRepository();
            var manufacturers = repo.GetAll().ToList();
            cmbManufacturerFilter.Items.Clear();
            cmbManufacturerFilter.Items.Add("Все марки");
            foreach (var m in manufacturers)
                cmbManufacturerFilter.Items.Add(m.Name);
            cmbManufacturerFilter.SelectedIndex = 0;
        }

        private void LoadStatusFilter()
        {
            var repo = new CarStatusRepository();
            var statuses = repo.GetAll().ToList();
            cmbStatusFilter.Items.Clear();
            cmbStatusFilter.Items.Add("Все статусы");
            foreach (var s in statuses)
                cmbStatusFilter.Items.Add(s.Name);
            cmbStatusFilter.SelectedIndex = 0;
        }

        private void DgvCars_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == dgvCars.Columns["Price"].Index)
            {
                var cars = new List<Car>();
                foreach (DataGridViewRow row in dgvCars.Rows)
                {
                    var car = new Car
                    {
                        Id = Convert.ToInt32(row.Cells["Id"].Value),
                        Manufacturer = new Manufacturer { Name = row.Cells["Manufacturer"].Value.ToString() },
                        Model = row.Cells["Model"].Value.ToString(),
                        Year = Convert.ToInt32(row.Cells["Year"].Value),
                        Color = row.Cells["Color"].Value.ToString(),
                        Price = decimal.Parse(row.Cells["Price"].Value.ToString().Replace(" €", "").Replace(" ", ""), NumberStyles.Any),
                        RegistrationNumber = row.Cells["RegistrationNumber"].Value.ToString(),
                        Status = new CarStatus { Name = row.Cells["Status"].Value.ToString() }
                    };
                    cars.Add(car);
                }

                // Сортируем список
                cars = isPriceAscending 
                    ? cars.OrderBy(c => c.Price).ToList()
                    : cars.OrderByDescending(c => c.Price).ToList();

                // Меняем направление сортировки
                isPriceAscending = !isPriceAscending;

                // Обновляем таблицу
                dgvCars.Rows.Clear();
                foreach (var car in cars)
                {
                    dgvCars.Rows.Add(
                        car.Id,
                        car.Manufacturer.Name,
                        car.Model,
                        car.Year,
                        car.Color,
                        car.Mileage.ToString("N0") + " км",
                        car.Price.ToString("N0") + " €",
                        car.RegistrationNumber,
                        car.Status?.Name ?? "",
                        "✏️",
                        "🗑️"
                    );
                }
            }
        }

        private void SetActiveSidebarButton(Button activeButton)
        {
            var buttons = new[] { btnDashboard, btnCarInventory, btnRegisterNewCar, btnAnalytics, btnReports };
            foreach (var btn in buttons)
            {
                btn.BackColor = Color.Transparent;
                btn.ForeColor = Color.Black;
                btn.Font = new Font("Segoe UI", 12F, FontStyle.Regular);
            }
            activeButton.BackColor = Color.Black;
            activeButton.ForeColor = Color.White;
            activeButton.Font = new Font("Segoe UI", 12F, FontStyle.Regular);
        }
    }
} 