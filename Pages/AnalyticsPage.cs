using System;
using System.Drawing;
using System.Windows.Forms;

namespace AutoSalon.Pages
{
    public class AnalyticsPage : Form
    {
        private Panel topPanel;
        private Label lblTitle;
        private Button btnBack;

        public AnalyticsPage()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Автосалон - Аналитика";
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
                Text = "Аналитика",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Панель с кнопками аналитики
            Panel analyticsPanel = new Panel
            {
                Location = new Point(20, 100),
                Size = new Size(1160, 600),
                BackColor = Color.White
            };

            Button btnAveragePrice = new Button
            {
                Text = "Средняя цена",
                Location = new Point(20, 20),
                Size = new Size(300, 100),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12F),
                Cursor = Cursors.Hand
            };

            Button btnPriceByColor = new Button
            {
                Text = "Цены по цвету",
                Location = new Point(340, 20),
                Size = new Size(300, 100),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12F),
                Cursor = Cursors.Hand
            };

            Button btnMostFrequent = new Button
            {
                Text = "Популярный производитель",
                Location = new Point(660, 20),
                Size = new Size(300, 100),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12F),
                Cursor = Cursors.Hand
            };

            // Нижняя панель с кнопкой назад
            Panel bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                BackColor = Color.White
            };

            btnBack = new Button
            {
                Text = "Назад",
                Location = new Point(1080, 20),
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

            // Добавляем элементы управления на форму
            topPanel.Controls.Add(lblTitle);
            analyticsPanel.Controls.AddRange(new Control[] { btnAveragePrice, btnPriceByColor, btnMostFrequent });
            bottomPanel.Controls.Add(btnBack);
            
            this.Controls.AddRange(new Control[] { topPanel, analyticsPanel, bottomPanel });
        }
    }
} 