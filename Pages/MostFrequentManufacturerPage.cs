using System;
using System.Linq;
using System.Windows.Forms;
using AutoSalon.Database;
using Npgsql;
using System.Data;

namespace AutoSalon.Pages
{
    public partial class MostFrequentManufacturerPage : Form
    {
        private DataGridView dgvResults;
        private Label lblResult;

        public MostFrequentManufacturerPage()
        {
            InitializeComponent();
            LoadMostFrequentManufacturer();
        }

        private void InitializeComponent()
        {
            this.Text = "Producătorul cu Cel Mai Mare Număr de Automobile";
            this.Size = new System.Drawing.Size(800, 500);

            lblResult = new Label
            {
                Text = "Se încarcă datele...",
                Location = new System.Drawing.Point(20, 20),
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold)
            };

            dgvResults = new DataGridView
            {
                Location = new System.Drawing.Point(20, 60),
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

            dgvResults.Columns.Add("Manufacturer", "Producător");
            dgvResults.Columns.Add("Country", "Țară");
            dgvResults.Columns.Add("CarCount", "Număr de automobile");
            dgvResults.Columns["CarCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            Button btnBack = new Button
            {
                Text = "Înapoi",
                Location = new System.Drawing.Point(660, 410),
                Size = new System.Drawing.Size(100, 30),
                BackColor = System.Drawing.Color.FromArgb(0, 120, 215),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btnBack.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { lblResult, dgvResults, btnBack });
        }

        private void LoadMostFrequentManufacturer()
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string sql = @"
                        WITH manufacturer_counts AS (
                            SELECT 
                                m.name as manufacturer_name,
                                m.country,
                                COUNT(c.id) as car_count,
                                RANK() OVER (ORDER BY COUNT(c.id) DESC) as rank
                            FROM manufacturers m
                            JOIN cars c ON m.id = c.manufacturer_id
                            GROUP BY m.id, m.name, m.country
                        )
                        SELECT 
                            manufacturer_name,
                            country,
                            car_count
                        FROM manufacturer_counts
                        WHERE rank = 1
                        ORDER BY car_count DESC";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        using (var adapter = new NpgsqlDataAdapter(cmd))
                        {
                            var dt = new DataTable();
                            adapter.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                var manufacturerName = dt.Rows[0]["manufacturer_name"].ToString();
                                var carCount = Convert.ToInt32(dt.Rows[0]["car_count"]);
                                lblResult.Text = $"Producătorul cu cel mai mare număr de automobile: {manufacturerName} ({carCount} automobile)";

                                dgvResults.Rows.Clear();
                                foreach (DataRow row in dt.Rows)
                                {
                                    dgvResults.Rows.Add(
                                        row["manufacturer_name"],
                                        row["country"],
                                        row["car_count"]
                                    );
                                }
                            }
                            else
                            {
                                lblResult.Text = "Nu s-au găsit date";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la încărcarea datelor: {ex.Message}", "Eroare", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblResult.Text = "Eroare la încărcarea datelor";
            }
        }
    }
} 