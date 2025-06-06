using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using AutoSalon.Database.Models;
using AutoSalon.Database;
using Npgsql;
using System.Data;
using AutoSalon.Database.Repositories;

namespace AutoSalon.Pages
{
    public class CarListPage : Form
    {
        private DataGridView dgvCars;
        private Button btnBack;
        private Button btnRegisterNew;
        private TextBox txtDeleteRegistrationNumber;
        private Button btnDeleteCar;
        private Label lblDeleteRegistrationNumber;
        private string currentSortColumn = "manufacturer_name";
        private bool isAscending = true;

        public CarListPage()
        {
            InitializeComponents();
            LoadCars();
        }

        private void InitializeComponents()
        {
            this.Text = "Список автомобилей";
            this.Size = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(235, 235, 235);

            // Создаем DataGridView
            dgvCars = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(960, 500),
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
                RowTemplate = { Height = 35 },
                ColumnHeadersHeight = 40,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(0, 120, 215),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
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

            // Подписываемся на событие клика по содержимому ячейки (для кнопок)
            dgvCars.CellContentClick += dgvCars_CellContentClick;

            // Подписываемся на событие клика по заголовку колонки
            dgvCars.ColumnHeaderMouseClick += DgvCars_ColumnHeaderMouseClick;

            // Настраиваем колонки
            // Добавляем скрытый столбец для ID
            dgvCars.Columns.Add("Id", "ID");
            dgvCars.Columns["Id"].Visible = false; // Скрываем столбец ID

            dgvCars.Columns.Add("Manufacturer", "Производитель");
            dgvCars.Columns["Manufacturer"].Width = 150;

            dgvCars.Columns.Add("Model", "Модель");
            dgvCars.Columns["Model"].Width = 150;

            dgvCars.Columns.Add("Year", "Год");
            dgvCars.Columns["Year"].Width = 80;

            dgvCars.Columns.Add("Color", "Цвет");
            dgvCars.Columns["Color"].Width = 100;

            dgvCars.Columns.Add("Price", "Цена");
            dgvCars.Columns["Price"].Width = 120;
            dgvCars.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvCars.Columns.Add("RegistrationNumber", "Регистрационный номер");
            dgvCars.Columns["RegistrationNumber"].Width = 150;

            // Добавляем столбец с кнопкой удаления
            var deleteButtonColumn = new DataGridViewButtonColumn
            {
                HeaderText = "Действие",
                Text = "Удалить",
                UseColumnTextForButtonValue = true,
                Name = "DeleteColumn"
            };
            dgvCars.Columns.Add(deleteButtonColumn);

            // Создаем кнопку "Зарегистрировать новый автомобиль"
            btnRegisterNew = new Button
            {
                Text = "Зарегистрировать новый автомобиль",
                Location = new Point(20, 540),
                Size = new Size(250, 30),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            // Создаем кнопку "Назад"
            btnBack = new Button
            {
                Text = "Назад",
                Location = new Point(870, 540),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btnRegisterNew.Click += BtnRegisterNew_Click;
            btnBack.Click += (s, e) => this.Close();

            // Добавляем кнопку для просмотра средней цены
            Button btnAveragePrice = new Button
            {
                Text = "Средняя цена по стране",
                Location = new Point(290, 540),
                Size = new Size(200, 30),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btnAveragePrice.Click += (s, e) =>
            {
                using (var averagePriceForm = new AveragePricePage())
                {
                    averagePriceForm.ShowDialog();
                }
            };

            // Добавляем все элементы управления на форму
            this.Controls.AddRange(new Control[] { dgvCars, btnRegisterNew, btnAveragePrice, btnBack });
        }

        private void BtnRegisterNew_Click(object sender, EventArgs e)
        {
            using (var registerForm = new RegisterCarPage())
            {
                if (registerForm.ShowDialog() == DialogResult.OK)
                {
                    LoadCars(); // Обновляем список после добавления нового автомобиля
                }
            }
        }

        private void DgvCars_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0) return;

            string columnName = dgvCars.Columns[e.ColumnIndex].Name;
            
            // Пропускаем колонки, которые не нужно сортировать
            if (columnName == "Id" || columnName == "DeleteColumn") return;

            // Определяем имя колонки в базе данных
            string dbColumnName = columnName switch
            {
                "Manufacturer" => "manufacturer_name",
                "Model" => "model",
                "Year" => "year",
                "Color" => "color",
                "Price" => "price",
                "RegistrationNumber" => "registration_number",
                _ => columnName
            };

            // Если кликнули по той же колонке, меняем направление сортировки
            if (currentSortColumn == dbColumnName)
            {
                isAscending = !isAscending;
            }
            else
            {
                currentSortColumn = dbColumnName;
                isAscending = true;
            }

            // Обновляем отображение
            LoadCars();
        }

        private void BtnDeleteCar_Click(object sender, EventArgs e)
        {
            string registrationNumberToDelete = txtDeleteRegistrationNumber.Text.Trim();

            if (string.IsNullOrWhiteSpace(registrationNumberToDelete))
            {
                MessageBox.Show("Пожалуйста, введите регистрационный номер для удаления.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var carRepository = new CarRepository();
            var carToDelete = carRepository.GetCarByRegistrationNumber(registrationNumberToDelete);

            if (carToDelete == null)
            {
                MessageBox.Show("Автомобиль с таким регистрационным номером не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var result = MessageBox.Show($"Вы уверены, что хотите удалить автомобиль с номером {registrationNumberToDelete}?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (carRepository.Delete(carToDelete.Id))
                {
                    MessageBox.Show("Автомобиль успешно удален.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCars(); // Обновляем список после удаления
                    txtDeleteRegistrationNumber.Clear(); // Очищаем поле ввода
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении автомобиля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadCars()
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = @"
                    SELECT c.id, c.*, m.name as manufacturer_name 
                    FROM cars c 
                    JOIN manufacturers m ON c.manufacturer_id = m.id 
                    ORDER BY " + currentSortColumn + (isAscending ? " ASC" : " DESC");

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    using (var adapter = new NpgsqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);

                        dgvCars.Rows.Clear();
                        foreach (DataRow row in dt.Rows)
                        {
                            dgvCars.Rows.Add(
                                row["id"],
                                row["manufacturer_name"],
                                row["model"],
                                row["year"],
                                row["color"],
                                Convert.ToDecimal(row["price"]).ToString("C"),
                                row["registration_number"]
                            );
                        }
                    }
                }
            }
        }

        private void dgvCars_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Проверяем, что клик был по кнопке в столбце "Действие"
            if (dgvCars.Columns[e.ColumnIndex].Name == "DeleteColumn" && e.RowIndex >= 0)
            {
                // Получаем ID автомобиля из скрытого столбца
                int carId = Convert.ToInt32(dgvCars.Rows[e.RowIndex].Cells["Id"].Value);
                string registrationNumber = dgvCars.Rows[e.RowIndex].Cells["RegistrationNumber"].Value.ToString();

                var result = MessageBox.Show($"Вы уверены, что хотите удалить автомобиль с регистрационным номером {registrationNumber}?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var carRepository = new CarRepository();
                    if (carRepository.Delete(carId))
                    {
                        MessageBox.Show("Автомобиль успешно удален.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadCars(); // Обновляем список
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении автомобиля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
} 