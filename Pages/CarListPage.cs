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
                MultiSelect = false
            };

            // Подписываемся на событие клика по содержимому ячейки (для кнопок)
            dgvCars.CellContentClick += dgvCars_CellContentClick;

            // Настраиваем колонки
            // Добавляем скрытый столбец для ID
            dgvCars.Columns.Add("Id", "ID");
            dgvCars.Columns["Id"].Visible = false; // Скрываем столбец ID

            dgvCars.Columns.Add("Manufacturer", "Производитель");
            dgvCars.Columns.Add("Model", "Модель");
            dgvCars.Columns.Add("Year", "Год");
            dgvCars.Columns.Add("Color", "Цвет");
            dgvCars.Columns.Add("Price", "Цена");
            dgvCars.Columns.Add("RegistrationNumber", "Регистрационный номер");

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

            // Добавляем все элементы управления на форму
            this.Controls.AddRange(new Control[] { dgvCars, btnRegisterNew, btnBack });
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
                    ORDER BY m.name";

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
                                row["id"], // Добавляем ID
                                row["manufacturer_name"],
                                row["model"],
                                row["year"],
                                row["color"],
                                Convert.ToDecimal(row["price"]).ToString("C"),
                                row["registration_number"]
                                // Столбец с кнопкой удаления будет добавлен автоматически
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