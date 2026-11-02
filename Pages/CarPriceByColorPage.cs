using System;
using System.Linq;
using System.Windows.Forms;
using AutoSalon.Database;
using Npgsql;
using System.Data;
using System.Collections.Generic;
using System.Drawing;

namespace AutoSalon.Pages
{
    public partial class CarPriceByColorPage : Form
    {
        private ComboBox cmbColor;
        private DataGridView dgvCars;
        private readonly Label minimumPrice = new Label();
        private readonly Label maximumPrice = new Label();

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
            this.Size = new Size(960, 620);
            this.MinimumSize = new Size(760, 540);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(243, 244, 249);
            this.Font = new Font("Segoe UI", 10F);

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
            dgvCars.EnableHeadersVisualStyles = false;
            dgvCars.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(28, 35, 55);
            dgvCars.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCars.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvCars.ColumnHeadersHeight = 46;
            dgvCars.RowTemplate.Height = 48;
            dgvCars.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvCars.GridColor = Color.FromArgb(233, 235, 243);
            dgvCars.DefaultCellStyle.ForeColor = Color.FromArgb(36, 43, 65);
            dgvCars.DefaultCellStyle.SelectionBackColor = Color.FromArgb(235, 226, 255);
            dgvCars.DefaultCellStyle.SelectionForeColor = Color.FromArgb(67, 36, 125);
            dgvCars.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 253);
            dgvCars.DefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
            dgvCars.Columns["Type"].FillWeight = 130;
            dgvCars.Columns["Price"].DefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

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
                                minimumPrice.Text = dt.Rows.Count == 0 ? "—" :
                                    dt.AsEnumerable().Min(r => Convert.ToDecimal(r["price"])).ToString("N0") + " €";
                                maximumPrice.Text = dt.Rows.Count == 0 ? "—" :
                                    dt.AsEnumerable().Max(r => Convert.ToDecimal(r["price"])).ToString("N0") + " €";
                                foreach (DataRow row in dt.Rows)
                                {
                                    dgvCars.Rows.Add(
                                        row["type"],
                                        row["manufacturer_name"],
                                        row["model"],
                                        row["color"],
                                        Convert.ToDecimal(row["price"]).ToString("N0") + " €"
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

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, Padding = new Padding(24), ColumnCount = 1, RowCount = 4
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 84));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 112));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            var header = new Panel { Dock = DockStyle.Fill };
            header.Controls.Add(new Label
            {
                Text = "Prețuri după culoare", Font = new Font("Segoe UI", 23F, FontStyle.Bold),
                ForeColor = Color.FromArgb(28, 35, 55), AutoSize = true, Location = new Point(0, 0)
            });
            header.Controls.Add(new Label
            {
                Text = "Compară cel mai mic și cel mai mare preț pentru culoarea aleasă.",
                ForeColor = Color.FromArgb(100, 107, 125), AutoSize = true, Location = new Point(2, 47)
            });
            var filter = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, Padding = new Padding(0, 8, 0, 0) };
            lblColor.Text = "Culoare";
            lblColor.Margin = new Padding(0, 9, 14, 0);
            cmbColor.Width = 220;
            cmbColor.Margin = new Padding(0, 4, 16, 0);
            btnShow.Text = "Compară prețurile";
            btnShow.Size = new Size(180, 38);
            btnShow.FlatStyle = FlatStyle.Flat;
            btnShow.FlatAppearance.BorderSize = 0;
            btnShow.BackColor = Color.FromArgb(126, 75, 235);
            btnShow.ForeColor = Color.White;
            btnShow.Cursor = Cursors.Hand;
            filter.Controls.AddRange(new Control[] { lblColor, cmbColor, btnShow });
            var cards = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Margin = new Padding(0, 0, 0, 18) };
            cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            Panel PriceCard(string title, Label value, Color accent)
            {
                var card = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(16, 10, 16, 10), Margin = new Padding(0, 0, 12, 0) };
                var caption = new Label { Text = title, Dock = DockStyle.Top, Height = 24, ForeColor = Color.FromArgb(100, 107, 125) };
                value.Text = "—";
                value.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
                value.ForeColor = accent;
                value.Dock = DockStyle.Fill;
                card.Controls.Add(value);
                card.Controls.Add(caption);
                return card;
            }
            cards.Controls.Add(PriceCard("PREȚ MINIM", minimumPrice, Color.FromArgb(22, 133, 105)), 0, 0);
            cards.Controls.Add(PriceCard("PREȚ MAXIM", maximumPrice, Color.FromArgb(126, 75, 235)), 1, 0);
            dgvCars.Dock = DockStyle.Fill;
            dgvCars.Margin = new Padding(0);
            layout.Controls.Add(header, 0, 0);
            layout.Controls.Add(filter, 0, 1);
            layout.Controls.Add(cards, 0, 2);
            layout.Controls.Add(dgvCars, 0, 3);
            Controls.Add(layout);
            cmbColor.SelectedIndexChanged += (s, e) => btnShow.PerformClick();
        }
    }
}
