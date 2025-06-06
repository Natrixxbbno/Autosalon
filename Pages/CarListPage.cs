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

namespace AutoSalon.Pages
{
    public class CarListPage : Form
    {
        private DataGridView dgvCars;
        private Button btnBack;
        private TextBox txtSearch;
        private ComboBox cmbFilter;
        private ComboBox cmbManufacturerFilter;
        private ComboBox cmbStatusFilter;
        private Panel topPanel;
        private Panel bottomPanel;
        private Label lblTitle;
        private string currentSortColumn = "manufacturer_name";
        private bool isAscending = true;
        private Panel sidebarPanel;
        private Button btnDashboard;
        private Button btnCarInventory;
        private Button btnRegisterNewCar;
        private Button btnAnalytics;
        private Button btnReports;
        private PictureBox logoPictureBox;
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
                BackColor = Color.FromArgb(0, 120, 215)
            };

            lblTitle = new Label
            {
                Text = "Управление автомобилями",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Панель поиска и фильтрации
            Panel searchPanel = new Panel
            {
                Location = new Point(20, 100),
                Size = new Size(1160, 50),
                BackColor = Color.White
            };

            txtSearch = new TextBox
            {
                Location = new Point(10, 10),
                Size = new Size(300, 30),
                PlaceholderText = "Поиск по модели или производителю...",
                Font = new Font("Segoe UI", 10F)
            };

            cmbFilter = new ComboBox
            {
                Location = new Point(320, 10),
                Size = new Size(200, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F)
            };
            cmbFilter.Items.AddRange(new string[] { "Все", "По цене (возр.)", "По цене (убыв.)", "По году (новые)", "По году (старые)" });
            cmbFilter.SelectedIndex = 0;

            cmbManufacturerFilter = new ComboBox
            {
                Location = new Point(540, 10),
                Size = new Size(200, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F)
            };
            cmbStatusFilter = new ComboBox
            {
                Location = new Point(760, 10),
                Size = new Size(200, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F)
            };

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

            // Нижняя панель с кнопками
            bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                BackColor = Color.White
            };

            Button btnAveragePrice = new Button
            {
                Text = "Средняя цена",
                Location = new Point(20, 20),
                Size = new Size(180, 40),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F),
                Cursor = Cursors.Hand
            };

            Button btnPriceByColor = new Button
            {
                Text = "Цены по цвету",
                Location = new Point(220, 20),
                Size = new Size(180, 40),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F),
                Cursor = Cursors.Hand
            };

            Button btnMostFrequent = new Button
            {
                Text = "Популярный производитель",
                Location = new Point(420, 20),
                Size = new Size(220, 40),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F),
                Cursor = Cursors.Hand
            };

            Button btnManageStatuses = new Button
            {
                Text = "Статусы автомобилей",
                Location = new Point(660, 20),
                Size = new Size(180, 40),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F),
                Cursor = Cursors.Hand
            };

            btnBack = new Button
            {
                Text = "Назад",
                Location = new Point(860, 20),
                Size = new Size(100, 40),
                BackColor = Color.FromArgb(220, 220, 220),
                ForeColor = Color.FromArgb(64, 64, 64),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F),
                Cursor = Cursors.Hand
            };

            // Добавляем обработчики событий
            btnBack.Click += (s, e) => this.Close();
            btnAveragePrice.Click += (s, e) => { using (var form = new AveragePricePage()) form.ShowDialog(); };
            btnPriceByColor.Click += (s, e) => { using (var form = new CarPriceByColorPage()) form.ShowDialog(); };
            btnMostFrequent.Click += (s, e) => { using (var form = new MostFrequentManufacturerPage()) form.ShowDialog(); };
            btnManageStatuses.Click += (s, e) =>
            {
                using (var form = new ManageCarStatusesPage())
                {
                    form.ShowDialog();
                    LoadCars();
                }
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;
            cmbFilter.SelectedIndexChanged += CmbFilter_SelectedIndexChanged;
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

            // Добавляем элементы управления на форму
            topPanel.Controls.Add(lblTitle);
            searchPanel.Controls.AddRange(new Control[] { txtSearch, cmbFilter, cmbManufacturerFilter, cmbStatusFilter });
            bottomPanel.Controls.AddRange(new Control[] { btnAveragePrice, btnPriceByColor, btnMostFrequent, btnManageStatuses, btnBack });
            
            this.Controls.AddRange(new Control[] { topPanel, searchPanel, dgvCars, bottomPanel });

            // Добавляем боковую панель
            sidebarPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = Color.FromArgb(245, 245, 245)
            };
            logoPictureBox = new PictureBox
            {
                // Image = Properties.Resources.CarIcon, // Нет ресурса, оставляем пустым
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(20, 20),
                Size = new Size(40, 40),
                BackColor = Color.LightGray // Просто фон для визуального разделения
            };
            lblLogo = new Label
            {
                Text = "CarAutosalon",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(70, 28),
                AutoSize = true
            };
            btnDashboard = new Button { Text = "Dashboard", Location = new Point(0, 80), Size = new Size(220, 50), FlatStyle = FlatStyle.Flat, BackColor = Color.Transparent, Font = new Font("Segoe UI", 12F), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(40, 0, 0, 0) };
            btnCarInventory = new Button { Text = "Car Inventory", Location = new Point(0, 130), Size = new Size(220, 50), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(220, 220, 220), Font = new Font("Segoe UI", 12F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(40, 0, 0, 0) };
            btnRegisterNewCar = new Button { Text = "Register New Car", Location = new Point(0, 180), Size = new Size(220, 50), FlatStyle = FlatStyle.Flat, BackColor = Color.Transparent, Font = new Font("Segoe UI", 12F), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(40, 0, 0, 0) };
            btnAnalytics = new Button { Text = "Analytics", Location = new Point(0, 230), Size = new Size(220, 50), FlatStyle = FlatStyle.Flat, BackColor = Color.Transparent, Font = new Font("Segoe UI", 12F), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(40, 0, 0, 0) };
            btnReports = new Button { Text = "Reports", Location = new Point(0, 280), Size = new Size(220, 50), FlatStyle = FlatStyle.Flat, BackColor = Color.Transparent, Font = new Font("Segoe UI", 12F), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(40, 0, 0, 0) };
            sidebarPanel.Controls.AddRange(new Control[] { logoPictureBox, lblLogo, btnDashboard, btnCarInventory, btnRegisterNewCar, btnAnalytics, btnReports });
            this.Controls.Add(sidebarPanel);

            // Сдвигаем остальные панели вправо (topPanel, searchPanel, dgvCars, bottomPanel):
            topPanel.Location = new Point(220, 0);
            topPanel.Width -= 220;
            searchPanel.Location = new Point(240, 100);
            searchPanel.Width -= 220;
            dgvCars.Location = new Point(240, 170);
            dgvCars.Width -= 220;
            bottomPanel.Location = new Point(220, bottomPanel.Location.Y);
            bottomPanel.Width -= 220;

            // Добавляем обработчики для кнопок бокового меню
            btnDashboard.Click += (s, e) => MessageBox.Show("Раздел Dashboard в разработке", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnCarInventory.Click += (s, e) => { /* ничего не делаем, уже на этой странице */ };
            btnRegisterNewCar.Click += (s, e) => {
                using (var registerForm = new RegisterCarPage())
                {
                    registerForm.ShowDialog();
                    LoadCars();
                }
            };
            btnAnalytics.Click += (s, e) => MessageBox.Show("Раздел Analytics в разработке", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnReports.Click += (s, e) => MessageBox.Show("Раздел Reports в разработке", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // После создания dgvCars:
            dgvCars.CellContentClick += dgvCars_CellContentClick;
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadCars();
        }

        private void CmbFilter_SelectedIndexChanged(object sender, EventArgs e)
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

                // Применяем фильтрацию
                switch (cmbFilter.SelectedIndex)
                {
                    case 1: // По цене (возр.)
                        cars = cars.OrderBy(c => c.Price).ToList();
                        break;
                    case 2: // По цене (убыв.)
                        cars = cars.OrderByDescending(c => c.Price).ToList();
                        break;
                    case 3: // По году (новые)
                        cars = cars.OrderByDescending(c => c.Year).ToList();
                        break;
                    case 4: // По году (старые)
                        cars = cars.OrderBy(c => c.Year).ToList();
                        break;
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
                        car.Price.ToString("N0") + " ₽",
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
                    editForm.ShowDialog();
                    LoadCars();
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
    }
} 