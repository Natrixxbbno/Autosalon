using System;
using System.Linq;
using System.Windows.Forms;
using AutoSalon.Database;
using Npgsql;
using System.Data;
using System.Collections.Generic;

namespace AutoSalon.Pages
{
    public partial class AveragePricePage : Form
    {
        private ComboBox cmbCountry;
        private DataGridView dgvCars;

        public AveragePricePage()
        {
            InitializeComponent();
            LoadCountries();
        }

        private void LoadCountries()
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string sql = @"
                        SELECT DISTINCT country 
                        FROM manufacturers 
                        ORDER BY country";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            var countries = new List<string>();
                            while (reader.Read())
                            {
                                countries.Add(reader.GetString(0));
                            }
                            cmbCountry.Items.AddRange(countries.ToArray());
                            if (countries.Count > 0)
                                cmbCountry.SelectedIndex = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке списка стран: {ex.Message}", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Средняя цена по стране";
            this.Size = new System.Drawing.Size(800, 500);

            Label lblCountry = new Label
            {
                Text = "Выберите страну производителя:",
                Location = new System.Drawing.Point(20, 20),
                AutoSize = true
            };

            cmbCountry = new ComboBox
            {
                Location = new System.Drawing.Point(20, 50),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            Button btnCalculate = new Button
            {
                Text = "Рассчитать",
                Location = new System.Drawing.Point(240, 50),
                Width = 100
            };

            Label lblResult = new Label
            {
                Location = new System.Drawing.Point(20, 90),
                AutoSize = true
            };

            dgvCars = new DataGridView
            {
                Location = new System.Drawing.Point(20, 130),
                Size = new System.Drawing.Size(740, 300),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            dgvCars.Columns.Add("Manufacturer", "Производитель");
            dgvCars.Columns.Add("Model", "Модель");
            dgvCars.Columns.Add("Price", "Цена");
            dgvCars.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            btnCalculate.Click += (sender, e) =>
            {
                try
                {
                    if (cmbCountry.SelectedItem == null)
                    {
                        MessageBox.Show("Пожалуйста, выберите страну", "Предупреждение", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    using (var conn = DatabaseConnection.GetConnection())
                    {
                        conn.Open();
                        var country = cmbCountry.SelectedItem.ToString();
                        
                        // Получаем среднюю цену
                        string avgSql = @"
                            SELECT AVG(c.price) as average_price
                            FROM cars c
                            JOIN manufacturers m ON c.manufacturer_id = m.id
                            WHERE LOWER(m.country) = LOWER(@country)";

                        using (var cmd = new NpgsqlCommand(avgSql, conn))
                        {
                            cmd.Parameters.AddWithValue("@country", country);
                            var result = cmd.ExecuteScalar();

                            if (result != null && result != DBNull.Value)
                            {
                                decimal averagePrice = Convert.ToDecimal(result);
                                lblResult.Text = $"Средняя цена автомобилей из {country}: {averagePrice:C}";
                            }
                            else
                            {
                                lblResult.Text = $"Автомобили из страны {country} не найдены";
                            }
                        }

                        // Получаем список автомобилей
                        string carsSql = @"
                            SELECT m.name as manufacturer_name, c.model, c.price
                            FROM cars c
                            JOIN manufacturers m ON c.manufacturer_id = m.id
                            WHERE LOWER(m.country) = LOWER(@country)
                            ORDER BY c.price DESC";

                        using (var cmd = new NpgsqlCommand(carsSql, conn))
                        {
                            cmd.Parameters.AddWithValue("@country", country);
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
                                        Convert.ToDecimal(row["price"]).ToString("C")
                                    );
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при расчете: {ex.Message}", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            this.Controls.AddRange(new Control[] { lblCountry, cmbCountry, btnCalculate, lblResult, dgvCars });
        }
    }
} 