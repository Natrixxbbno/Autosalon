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
using System.Runtime.InteropServices;

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
            this.Paint += MainForm_Paint;
        }

        private void InitializeComponents()
        {
            this.Text = "Автосалон - Управление автомобилями";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(241, 244, 248); // светло-серый фон, как на скриншоте
            this.Font = new Font("Segoe UI", 9F);

            // Верхняя панель
            int panelHeight = 90;
            int panelWidth = this.ClientSize.Width;
            int panelX = 0;
            int panelY = 16;
            topPanel = new Panel
            {
                Location = new Point(panelX, panelY),
                Size = new Size(panelWidth, panelHeight),
                BackColor = Color.FromArgb(48, 58, 76),
                BorderStyle = BorderStyle.None,
                Padding = new Padding(20, 16, 20, 16)
            };
            // Скругляем только верхние углы
            topPanel.Region = System.Drawing.Region.FromHrgn(
                NativeMethods.CreateRoundRectRgn(0, 0, topPanel.Width, topPanel.Height + 1, 24, 24));

            lblTitle = new Label
            {
                Text = "Управление автомобилями",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Новый подзаголовок
            var lblSubtitle = new Label
            {
                Text = "Управляйте своим автопарком", // или "Manage your car collection"
                Font = new Font("Segoe UI", 11F, FontStyle.Regular),
                ForeColor = Color.White,
                Location = new Point(22, 54), // чуть ниже основного заголовка
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
            int filterPanelHeight = 28;
            int filterPanelWidth = 180;

            // Панель для поиска
            var searchBoxPanel = new Panel
            {
                Location = new Point(10, 22),
                Size = new Size(filterPanelWidth, filterPanelHeight),
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
                Size = new Size(filterPanelWidth, filterPanelHeight + 18),
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
                Size = new Size(filterPanelWidth, filterPanelHeight + 18),
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
                    BackColor = Color.FromArgb(43, 47, 74),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Padding = new Padding(5),
                    BackColor = Color.White,
                    ForeColor = Color.Black
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(244, 247, 250)
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

            // Вместо этого добавляю одну колонку:
            var actionColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Действие",
                Name = "ActionColumn",
                Width = 80
            };
            dgvCars.Columns.Add(actionColumn);

            // Добавляем боковую панель
            sidebarPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = Color.FromArgb(23, 30, 50), // глубокий тёмно-синий
            };
            logoIcon = new Label
            {
                Text = "\uE804",
                Font = new Font("Segoe MDL2 Assets", 32F, FontStyle.Regular),
                ForeColor = Color.White,
                Location = new Point(10, 18),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            lblLogo = new Label
            {
                Text = "CarAutosalon",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(68, 32),
                AutoSize = true
            };
            btnDashboard = new Button { Text = "\uD83D\uDCCA  Dashboard", Location = new Point(0, 80), Size = new Size(220, 48), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(140, 82, 255), ForeColor = Color.White, Font = new Font("Segoe UI", 12F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(24, 0, 0, 0), Cursor = Cursors.Hand, TabStop = false };
            btnCarInventory = new Button { Text = "\uD83D\uDE97  Car Inventory", Location = new Point(0, 128), Size = new Size(220, 48), FlatStyle = FlatStyle.Flat, BackColor = Color.Transparent, ForeColor = Color.White, Font = new Font("Segoe UI", 12F, FontStyle.Regular), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(24, 0, 0, 0), Cursor = Cursors.Hand, TabStop = false };
            btnRegisterNewCar = new Button { Text = "\u2795  Register New Car", Location = new Point(0, 176), Size = new Size(220, 48), FlatStyle = FlatStyle.Flat, BackColor = Color.Transparent, ForeColor = Color.White, Font = new Font("Segoe UI", 12F, FontStyle.Regular), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(24, 0, 0, 0), Cursor = Cursors.Hand, TabStop = false };
            btnAnalytics = new Button { Text = "\u23F0  Analytics", Location = new Point(0, 224), Size = new Size(220, 48), FlatStyle = FlatStyle.Flat, BackColor = Color.Transparent, ForeColor = Color.White, Font = new Font("Segoe UI", 12F, FontStyle.Regular), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(24, 0, 0, 0), Cursor = Cursors.Hand, TabStop = false };
            btnReports = new Button { Text = "\uD83D\uDCC4  Reports", Location = new Point(0, 272), Size = new Size(220, 48), FlatStyle = FlatStyle.Flat, BackColor = Color.Transparent, ForeColor = Color.White, Font = new Font("Segoe UI", 12F, FontStyle.Regular), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(24, 0, 0, 0), Cursor = Cursors.Hand, TabStop = false };
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
                Cursor = Cursors.Hand,
                ForeColor = Color.White
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
            dgvCars.EnableHeadersVisualStyles = false;
            dgvCars.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(48, 58, 76);
            dgvCars.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCars.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvCars.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

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

            // Добавить методы в класс CarListPage:
            dgvCars.CellPainting += DgvCars_CellPainting_ActionIcons;
            dgvCars.CellClick += DgvCars_CellClick_ActionIcons;
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
                        ""
                    );
                    // Цветные метки статусов
                    var statusCell = dgvCars.Rows[dgvCars.Rows.Count - 1].Cells["Status"];
                    if (car.Status?.Name == "Available")
                    {
                        statusCell.Style.BackColor = Color.FromArgb(209, 250, 229); // светло-зелёный
                        statusCell.Style.ForeColor = Color.FromArgb(27, 94, 32); // тёмно-зелёный
                    }
                    else if (car.Status?.Name == "Reserved")
                    {
                        statusCell.Style.BackColor = Color.FromArgb(255, 236, 179); // светло-оранжевый
                        statusCell.Style.ForeColor = Color.FromArgb(255, 143, 0); // оранжевый
                    }
                    else if (car.Status?.Name == "Sold")
                    {
                        statusCell.Style.BackColor = Color.FromArgb(255, 205, 210); // светло-красный
                        statusCell.Style.ForeColor = Color.FromArgb(183, 28, 28); // тёмно-красный
                    }
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
            if (e.ColumnIndex == dgvCars.Columns["EditImageColumn"].Index)
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
            if (e.ColumnIndex == dgvCars.Columns["DeleteImageColumn"].Index)
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
                        null, // edit image
                        null  // delete image
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

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            // Градиентный фон сверху вниз
            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(this.ClientRectangle,
                Color.White, Color.FromArgb(241, 244, 248), 90F))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }

        private void DgvCars_CellPainting_ActionIcons(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex == dgvCars.Columns["ActionColumn"].Index && e.RowIndex >= 0)
            {
                e.PaintBackground(e.ClipBounds, true);
                var editImg = Image.FromFile("Resources/edit.png");
                var deleteImg = Image.FromFile("Resources/delete.png");
                int iconSize = 20;
                int padding = 6;
                int totalWidth = iconSize * 2 + padding;
                int x = e.CellBounds.Left + (e.CellBounds.Width - totalWidth) / 2;
                int y = e.CellBounds.Top + (e.CellBounds.Height - iconSize) / 2;
                e.Graphics.DrawImage(editImg, new Rectangle(x, y, iconSize, iconSize));
                e.Graphics.DrawImage(deleteImg, new Rectangle(x + iconSize + padding, y, iconSize, iconSize));
                e.Handled = true;
            }
        }

        private void DgvCars_CellClick_ActionIcons(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvCars.Columns["ActionColumn"].Index && e.RowIndex >= 0)
            {
                var cell = dgvCars[e.ColumnIndex, e.RowIndex];
                var cellRect = dgvCars.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                var mousePos = dgvCars.PointToClient(Cursor.Position);
                int iconSize = 20;
                int padding = 6;
                int totalWidth = iconSize * 2 + padding;
                int x = cellRect.Left + (cellRect.Width - totalWidth) / 2;
                int y = cellRect.Top + (cellRect.Height - iconSize) / 2;
                var editRect = new Rectangle(x, y, iconSize, iconSize);
                var deleteRect = new Rectangle(x + iconSize + padding, y, iconSize, iconSize);
                if (editRect.Contains(mousePos))
                {
                    // Действие редактирования
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
                }
                else if (deleteRect.Contains(mousePos))
                {
                    // Действие удаления
                    var carId = Convert.ToInt32(dgvCars.Rows[e.RowIndex].Cells["Id"].Value);
                    var registrationNumber = dgvCars.Rows[e.RowIndex].Cells["RegistrationNumber"].Value.ToString();
                    var result = MessageBox.Show($"Вы уверены, что хотите удалить автомобиль с номером {registrationNumber}?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
        }
    }

    internal class NativeMethods
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        public static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );
    }
} 