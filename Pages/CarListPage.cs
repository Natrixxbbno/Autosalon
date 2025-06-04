using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using AutoSalon.Database.Models;
using AutoSalon.Database;
using Npgsql;
using System.Data;

namespace AutoSalon.Pages
{
    public class CarListPage : Form
    {
        private DataGridView dgvCars;
        private Button btnBack;

        public CarListPage()
        {
            InitializeComponents();
            LoadCars();
        }

        private void InitializeComponents()
        {
            this.Text = "Список автомобилей";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(235, 235, 235);

            // Создаем DataGridView
            dgvCars = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(940, 480),
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

            // Настраиваем колонки
            dgvCars.Columns.Add("Manufacturer", "Производитель");
            dgvCars.Columns.Add("Model", "Модель");
            dgvCars.Columns.Add("Year", "Год");
            dgvCars.Columns.Add("Color", "Цвет");
            dgvCars.Columns.Add("Price", "Цена");
            dgvCars.Columns.Add("RegistrationNumber", "Регистрационный номер");

            // Создаем кнопку "Назад"
            btnBack = new Button
            {
                Text = "Назад",
                Location = new Point(20, 520),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btnBack.Click += (s, e) => this.Close();

            // Добавляем элементы управления на форму
            this.Controls.AddRange(new Control[] { dgvCars, btnBack });
        }

        private void LoadCars()
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = @"
                    SELECT c.*, m.name as manufacturer_name 
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
    }
} 