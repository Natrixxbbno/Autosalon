using System;
using System.Linq;
using System.Windows.Forms;
using AutoSalon.Database;
using Npgsql;
using System.Data;
using System.Collections.Generic;

namespace AutoSalon.Pages
{
    public partial class CarPriceByColorPage : Form
    {
        private ComboBox cmbColor;
        private DataGridView dgvCars;

        public CarPriceByColorPage()
        {
            InitializeComponent();
            LoadColors();
        }

        private void LoadColors()
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string sql = @"
                        SELECT DISTINCT color 
                        FROM cars 
                        ORDER BY color";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            var colors = new List<string>();
                            while (reader.Read())
                            {
                                colors.Add(reader.GetString(0));
                            }
                            cmbColor.Items.AddRange(colors.ToArray());
                            if (colors.Count > 0)
                                cmbColor.SelectedIndex = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la încărcarea listei de culori: {ex.Message}", "Eroare", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Prețurile Automobilelor pe Culori";
            this.Size = new System.Drawing.Size(800, 500);

            Label lblColor = new Label
            {
                Text = "Selectați culoarea automobilului:",
                Location = new System.Drawing.Point(20, 20),
                AutoSize = true
            };

            cmbColor = new ComboBox
            {
                Location = new System.Drawing.Point(20, 50),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            Button btnShow = new Button
            {
                Text = "Afișează",
                Location = new System.Drawing.Point(240, 50),
                Width = 100
            };

            dgvCars = new DataGridView
            {
                Location = new System.Drawing.Point(20, 90),
                Size = new System.Drawing.Size(740, 340),
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

            dgvCars.Columns.Add("Type", "Tip");
            dgvCars.Columns.Add("Manufacturer", "Producător");
            dgvCars.Columns.Add("Model", "Model");
            dgvCars.Columns.Add("Color", "Culoare");
            dgvCars.Columns.Add("Price", "Preț");
            dgvCars.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            btnShow.Click += (sender, e) =>
            {
                try
                {
                    if (cmbColor.SelectedItem == null)
                    {
                        MessageBox.Show("Vă rugăm să selectați o culoare", "Avertisment", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    using (var conn = DatabaseConnection.GetConnection())
                    {
                        conn.Open();
                        var color = cmbColor.SelectedItem.ToString();
                        
                        string sql = @"
                            WITH car_prices AS (
                                SELECT 
                                    m.name as manufacturer_name,
                                    c.model,
                                    c.price,
                                    c.color,
                                    ROW_NUMBER() OVER (ORDER BY c.price DESC) as max_price_rank,
                                    ROW_NUMBER() OVER (ORDER BY c.price ASC) as min_price_rank
                                FROM cars c
                                JOIN manufacturers m ON c.manufacturer_id = m.id
                                WHERE LOWER(c.color) = LOWER(@color)
                            )
                            SELECT 
                                CASE 
                                    WHEN max_price_rank = 1 THEN 'Cel mai scump'
                                    WHEN min_price_rank = 1 THEN 'Cel mai ieftin'
                                END as type,
                                manufacturer_name,
                                model,
                                color,
                                price
                            FROM car_prices
                            WHERE max_price_rank = 1 OR min_price_rank = 1
                            ORDER BY price DESC";

                        using (var cmd = new NpgsqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@color", color);
                            using (var adapter = new NpgsqlDataAdapter(cmd))
                            {
                                var dt = new DataTable();
                                adapter.Fill(dt);

                                dgvCars.Rows.Clear();
                                foreach (DataRow row in dt.Rows)
                                {
                                    dgvCars.Rows.Add(
                                        row["type"],
                                        row["manufacturer_name"],
                                        row["model"],
                                        row["color"],
                                        Convert.ToDecimal(row["price"]).ToString("C")
                                    );
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Eroare la obținerea datelor: {ex.Message}", "Eroare", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            this.Controls.AddRange(new Control[] { lblColor, cmbColor, btnShow, dgvCars });
        }
    }
} 