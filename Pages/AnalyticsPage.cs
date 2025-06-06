using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AutoSalon.Database.Repositories;
using AutoSalon.Database.Models;
using System.Collections.Generic;

namespace AutoSalon.Pages
{
    public class AnalyticsPage : Form
    {
        private CarRepository carRepository = new CarRepository();

        public AnalyticsPage()
        {
            InitializeComponents();
            LoadAnalyticsData();
        }

        private void InitializeComponents()
        {
            this.Text = "Автосалон - Аналитика";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.Font = new Font("Segoe UI", 10F);

            // Верхняя панель
            Panel topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.White,
                Padding = new Padding(30, 20, 30, 0)
            };
            Label lblTitle = new Label
            {
                Text = "Analytics",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(0,0)
            };
             Label lblSubtitle = new Label
            {
                Text = "Insights and statistics about your inventory",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(0, 45)
            };
            topPanel.Controls.Add(lblTitle);
             topPanel.Controls.Add(lblSubtitle);
            this.Controls.Add(topPanel);

            // Главная панель аналитики (контейнер для карточек)
            Panel cardsContainerPanel = new Panel
            {
                Location = new Point(30, 110),
                Size = new Size(1140, 650),
                BackColor = Color.Transparent,
                AutoScroll = true // Если карточек станет много
            };

            this.Controls.Add(cardsContainerPanel);
        }

        private void LoadAnalyticsData()
        {
             // Получаем данные
            var cars = carRepository.GetAll().ToList();

            // --- Расчеты аналитики ---
            decimal avgPrice = cars.Count > 0 ? cars.Average(c => c.Price) : 0;
            decimal minPrice = cars.Count > 0 ? cars.Min(c => c.Price) : 0;
            decimal maxPrice = cars.Count > 0 ? cars.Max(c => c.Price) : 0;

            // Цвета
            var colorGroups = cars.Where(c => !string.IsNullOrWhiteSpace(c.Color))
                .GroupBy(c => c.Color.Trim())
                .Select(g => new { Color = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(5)
                .ToList();

            // Производители
            var brandGroups = cars.Where(c => c.Manufacturer != null && !string.IsNullOrWhiteSpace(c.Manufacturer.Name))
                .GroupBy(c => c.Manufacturer.Name.Trim())
                .Select(g => new { Brand = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(5)
                .ToList();

            // Статусы
            var statusGroups = cars
                .GroupBy(c => string.IsNullOrWhiteSpace(c.Status?.Name) ? "В наличии" : c.Status.Name.Trim())
                .Select(g => new { Status = g.Key, Count = g.Count() })
                 .OrderByDescending(g => g.Count)
                .ToList();

            // --- Создание и заполнение карточек ---
            Panel cardsContainerPanel = this.Controls.OfType<Panel>().FirstOrDefault(p => p.Location == new Point(30, 110));
            if (cardsContainerPanel == null) return; // Проверка на случай, если панель не найдена

            // Карточка "Average Price"
            Panel cardAveragePrice = CreateCard(new Point(0, 0), new Size(360, 150));
            Label lblAvgTitle = CreateCardTitle("Average Price", new Point(20, 20));
            Label lblAvgSubtitle = CreateCardSubtitle("Overall inventory average", new Point(20, 45));
            Label lblAvgValue = new Label
            {
                Text = avgPrice > 0 ? $"${avgPrice:N0}" : "-",
                Font = new Font("Segoe UI", 30F, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(20, 75),
                AutoSize = true
            };
            cardAveragePrice.Controls.Add(lblAvgTitle);
            cardAveragePrice.Controls.Add(lblAvgSubtitle);
            cardAveragePrice.Controls.Add(lblAvgValue);
            cardsContainerPanel.Controls.Add(cardAveragePrice);

            // Карточка "Price Range"
            Panel cardPriceRange = CreateCard(new Point(380, 0), new Size(360, 150));
            Label lblRangeTitle = CreateCardTitle("Price Range", new Point(20, 20));
            Label lblRangeSubtitle = CreateCardSubtitle("Min and max prices", new Point(20, 45));

            Label lblMin = new Label { Text = "Minimum:", Font = new Font("Segoe UI", 12F), ForeColor = Color.Black, Location = new Point(20, 80), AutoSize = true };
            Label lblMinValue = new Label { Text = minPrice > 0 ? $"${minPrice:N0}" : "-", Font = new Font("Segoe UI", 12F, FontStyle.Bold), ForeColor = Color.Black, Location = new Point(220, 80), AutoSize = true };

            Label lblMax = new Label { Text = "Maximum:", Font = new Font("Segoe UI", 12F), ForeColor = Color.Black, Location = new Point(20, 105), AutoSize = true };
             Label lblMaxValue = new Label { Text = maxPrice > 0 ? $"${maxPrice:N0}" : "-", Font = new Font("Segoe UI", 12F, FontStyle.Bold), ForeColor = Color.Black, Location = new Point(220, 105), AutoSize = true };

            cardPriceRange.Controls.Add(lblRangeTitle);
            cardPriceRange.Controls.Add(lblRangeSubtitle);
            cardPriceRange.Controls.Add(lblMin);
            cardPriceRange.Controls.Add(lblMinValue);
            cardPriceRange.Controls.Add(lblMax);
            cardPriceRange.Controls.Add(lblMaxValue);
            cardsContainerPanel.Controls.Add(cardPriceRange);

            // Карточка "Inventory Status"
            Panel cardInventory = CreateCard(new Point(760, 0), new Size(360, 150));
            Label lblInvTitle = CreateCardTitle("Inventory Status", new Point(20, 20));
            Label lblInvSubtitle = CreateCardSubtitle("Current stock status", new Point(20, 45));
            cardInventory.Controls.Add(lblInvTitle);
            cardInventory.Controls.Add(lblInvSubtitle);

            int statusY = 80;
            foreach (var s in statusGroups)
            {
                Label lblStatus = new Label
                {
                    Text = s.Status + ":",
                    Font = new Font("Segoe UI", 12F),
                    ForeColor = Color.Black,
                    Location = new Point(20, statusY),
                    AutoSize = true
                };
                 Label lblCount = new Label
                {
                    Text = s.Count.ToString(),
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                    ForeColor = Color.Black,
                    Location = new Point(280, statusY),
                    AutoSize = true
                };
                cardInventory.Controls.Add(lblStatus);
                 cardInventory.Controls.Add(lblCount);
                statusY += 25;
            }
            cardsContainerPanel.Controls.Add(cardInventory);

            // Карточка "Price by Color"
            Panel cardColor = CreateCard(new Point(0, 170), new Size(550, 300));
            Label lblColorTitle = CreateCardTitle("Price by Color", new Point(20, 20));
            Label lblColorSubtitle = CreateCardSubtitle("Distribution of car prices by color", new Point(20, 45));
            cardColor.Controls.Add(lblColorTitle);
            cardColor.Controls.Add(lblColorSubtitle);

            int maxColorCount = colorGroups.Count > 0 ? colorGroups.Max(g => g.Count) : 1;
            int colorY = 85;
            foreach (var g in colorGroups)
            {
                // Цветной кружок (иконка)
                Panel colorCircle = new Panel
                {
                    Location = new Point(20, colorY + 4),
                    Size = new Size(15, 15),
                    BackColor = GetColorByName(g.Color), // Преобразование названия цвета в Color
                    BorderStyle = BorderStyle.FixedSingle
                };

                Label lbl = new Label
                {
                    Text = g.Color,
                    Font = new Font("Segoe UI", 11F),
                    ForeColor = Color.Black,
                    Location = new Point(45, colorY),
                    AutoSize = true
                };

                // Контейнер для прогресс-бара (светлый фон)
                Panel barContainer = new Panel
                {
                    Location = new Point(200, colorY + 4),
                    Size = new Size(250, 15),
                    BackColor = Color.FromArgb(230, 230, 230), // Светлый фон
                };
                 // Сама полоса (темная)
                Panel bar = new Panel
                {
                    Location = new Point(0, 0),
                    Size = new Size((int)(250.0 * g.Count / maxColorCount), 15),
                    BackColor = Color.Black, // Темная полоса
                };
                 barContainer.Controls.Add(bar);

                Label lblCount = new Label
                {
                    Text = g.Count.ToString(),
                    Font = new Font("Segoe UI", 11F),
                    ForeColor = Color.Black,
                    Location = new Point(470, colorY),
                    AutoSize = true
                };

                // Добавляем элементы на карточку
                cardColor.Controls.Add(colorCircle);
                cardColor.Controls.Add(lbl);
                cardColor.Controls.Add(barContainer);
                cardColor.Controls.Add(lblCount);

                // Делаем кружок круглым после добавления на панель
                colorCircle.CreateControl(); // Принудительное создание хэндла
                using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                     path.AddEllipse(0, 0, colorCircle.Width, colorCircle.Height);
                    colorCircle.Region = new Region(path);
                }

                colorY += 35;
            }
             cardsContainerPanel.Controls.Add(cardColor);

            // Карточка "Popular Manufacturers"
            Panel cardManufacturers = CreateCard(new Point(570, 170), new Size(550, 300));
            Label lblManTitle = CreateCardTitle("Popular Manufacturers", new Point(20, 20));
            Label lblManSubtitle = CreateCardSubtitle("Most popular car brands in inventory", new Point(20, 45));
            cardManufacturers.Controls.Add(lblManTitle);
            cardManufacturers.Controls.Add(lblManSubtitle);

            int maxBrandCount = brandGroups.Count > 0 ? brandGroups.Max(g => g.Count) : 1;
            int brandY = 85;
            foreach (var g in brandGroups)
            {
                Label lbl = new Label
                {
                    Text = g.Brand,
                    Font = new Font("Segoe UI", 11F),
                    ForeColor = Color.Black,
                    Location = new Point(20, brandY),
                    Size = new Size(160, 25),
                    AutoSize = true
                };
                // Контейнер для прогресс-бара (светлый фон)
                Panel barContainer = new Panel
                {
                    Location = new Point(200, brandY + 4),
                    Size = new Size(250, 15),
                     BackColor = Color.FromArgb(230, 230, 230), // Светлый фон
                };
                 // Сама полоса (темная)
                 Panel bar = new Panel
                {
                    Location = new Point(0, 0),
                    Size = new Size((int)(250.0 * g.Count / maxBrandCount), 15),
                    BackColor = Color.Black, // Темная полоса
                };
                 barContainer.Controls.Add(bar);

                Label lblCount = new Label
                {
                    Text = g.Count.ToString(),
                    Font = new Font("Segoe UI", 11F),
                    ForeColor = Color.Black,
                     Location = new Point(470, brandY),
                    AutoSize = true
                };
                cardManufacturers.Controls.Add(lbl);
                 cardManufacturers.Controls.Add(barContainer);
                cardManufacturers.Controls.Add(lblCount);
                brandY += 35;
            }
            cardsContainerPanel.Controls.Add(cardManufacturers);

             // Кнопка назад (перемещена вниз)
            Button btnBack = new Button
            {
                Text = "Назад",
                Location = new Point(980, 600),
                Size = new Size(100, 40),
                BackColor = Color.FromArgb(220, 220, 220),
                ForeColor = Color.FromArgb(64, 64, 64),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F),
                Cursor = Cursors.Hand
            };
            btnBack.Click += (s, e) => this.Close();
            cardsContainerPanel.Controls.Add(btnBack);
        }

        private Panel CreateCard(Point location, Size size)
        {
            Panel card = new Panel
            {
                Location = location,
                Size = size,
                BackColor = Color.White,
                BorderStyle = BorderStyle.None,
                Margin = new Padding(10),
                Padding = new Padding(28, 24, 28, 24)
            };
            card.Paint += (s, e) => {
                var g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                int radius = 32; // Более округлённые углы
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    path.AddArc(0, 0, radius, radius, 180, 90);
                    path.AddArc(card.Width - radius - 1, 0, radius, radius, 270, 90);
                    path.AddArc(card.Width - radius - 1, card.Height - radius - 1, radius, radius, 0, 90);
                    path.AddArc(0, card.Height - radius - 1, radius, radius, 90, 90);
                    path.CloseAllFigures();
                    card.Region = new Region(path);
                    using (var pen = new Pen(Color.LightGray, 1))
                        g.DrawPath(pen, path);
                }
            };
            return card;
        }

        private Label CreateCardTitle(string text, Point location)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = location,
                AutoSize = true
            };
        }

        private Label CreateCardSubtitle(string text, Point location)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.Gray,
                Location = location,
                AutoSize = true
            };
        }

         // Хелпер для преобразования названия цвета в Color
         private Color GetColorByName(string colorName)
        {
            switch (colorName.ToLower())
            {
                case "black": return Color.Black;
                case "white": return Color.White;
                case "silver": return Color.Silver;
                case "gray": return Color.Gray;
                case "blue": return Color.FromArgb(60, 120, 216);
                case "red": return Color.Red;
                case "green": return Color.Green;
                case "yellow": return Color.Yellow;
                case "orange": return Color.Orange;
                case "brown": return Color.Brown;
                case "beige": return Color.Beige;
                case "purple": return Color.Purple;
                case "pink": return Color.Pink;
                // Добавьте другие стандартные цвета по необходимости
                default:
                    // Генерируем цвет на основе хеша названия (для новых цветов)
                    int hash = colorName.GetHashCode();
                    int r = (hash & 0xFF0000) >> 16;
                    int g = (hash & 0x00FF00) >> 8;
                    int b = (hash & 0x0000FF);
                    return Color.FromArgb(255, r, g, b);
            }
        }
    }
} 