using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;
using AutoSalon.Database.Repositories;

namespace AutoSalon.Pages
{
    public class LoginPage : Form
    {
        private readonly UserRepository _userRepository;

        public LoginPage()
        {
            _userRepository = new UserRepository();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "AutoSalon Premium - Autentificare";
            this.Size = new Size(500, 700);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            // Градиентный фон
            this.Paint += (s, e) =>
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    this.ClientRectangle,
                    Color.FromArgb(61, 102, 255),
                    Color.FromArgb(0, 204, 255),
                    90F))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
            };

            // Просто иконка авто без круга
            PictureBox iconBox = new PictureBox
            {
                Location = new Point((500 - 64) / 2, 40),
                Size = new Size(64, 64),
                BackColor = Color.Transparent,
                Image = Image.FromFile(Path.Combine("Resources", "car.png")),
                SizeMode = PictureBoxSizeMode.Zoom
            };

            // Заголовок
            Label lblTitle = new Label
            {
                Text = "AutoSalon Premium",
                Size = new Size(500, 32),
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            // Подзаголовок
            Label lblSubtitle = new Label
            {
                Text = "Conectați-vă pentru a accesa experiența dvs. premium auto",
                Size = new Size(500, 20),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            // Белая панель с тенью и скруглёнными углами
            Panel formPanel = new Panel
            {
                Location = new Point(30, 200),
                Size = new Size(340, 220),
                BackColor = Color.White,
            };
            formPanel.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, formPanel.Width - 1, formPanel.Height - 1);
                using (GraphicsPath path = RoundedRect(rect, 18))
                using (Pen pen = new Pen(Color.FromArgb(230, 230, 230), 1))
                using (SolidBrush shadow = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                {
                    g.FillPath(shadow, RoundedRect(new Rectangle(4, 6, rect.Width, rect.Height), 18));
                    g.FillPath(new SolidBrush(Color.White), path);
                    g.DrawPath(pen, path);
                }
            };

            // Email field с иконкой
            Panel emailPanel = new Panel
            {
                Location = new Point(25, 20),
                Size = new Size(290, 44),
                BackColor = Color.FromArgb(245, 245, 245),
            };
            PictureBox emailIcon = new PictureBox
            {
                Location = new Point(10, 10),
                Size = new Size(24, 24),
                Image = Image.FromFile(Path.Combine("Resources", "Email.png")),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };
            TextBox txtEmail = new TextBox
            {
                Location = new Point(44, 10),
                Size = new Size(230, 24),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(245, 245, 245),
                ForeColor = Color.Gray,
                Text = "Adresa de email"
            };
            txtEmail.GotFocus += (s, e) => { if (txtEmail.Text == "Adresa de email") { txtEmail.Text = ""; txtEmail.ForeColor = Color.Black; } };
            txtEmail.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(txtEmail.Text)) { txtEmail.Text = "Adresa de email"; txtEmail.ForeColor = Color.Gray; } };
            emailPanel.Controls.AddRange(new Control[] { emailIcon, txtEmail });

            // Password field с иконкой
            Panel passPanel = new Panel
            {
                Location = new Point(25, 74),
                Size = new Size(290, 44),
                BackColor = Color.FromArgb(245, 245, 245),
            };
            PictureBox passIcon = new PictureBox
            {
                Location = new Point(10, 10),
                Size = new Size(24, 24),
                Image = Image.FromFile(Path.Combine("Resources", "Password.png")),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };
            TextBox txtPassword = new TextBox
            {
                Location = new Point(44, 10),
                Size = new Size(230, 24),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(245, 245, 245),
                ForeColor = Color.Black,
                UseSystemPasswordChar = true,
                PlaceholderText = "Parolă"
            };
            passPanel.Controls.AddRange(new Control[] { passIcon, txtPassword });

            // Sign In button
            Button btnLogin = new Button
            {
                Text = "Autentificare",
                Location = new Point(25, 128),
                Size = new Size(290, 44),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(61, 102, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(0, 0, 0, 0)
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 82, 215);
            btnLogin.FlatAppearance.MouseDownBackColor = Color.FromArgb(41, 82, 215);
            btnLogin.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, btnLogin.Width, btnLogin.Height, 10, 10));

            // Ссылка регистрации и подпись на одной строке
            Panel registerPanel = new Panel
            {
                Location = new Point(25, 182),
                Size = new Size(290, 20),
                BackColor = Color.Transparent
            };
            Label lblNoAccount = new Label
            {
                Text = "Nu aveți un cont?",
                Location = new Point(0, 0),
                Size = new Size(140, 20),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(100, 100, 100),
                TextAlign = ContentAlignment.MiddleRight
            };
            LinkLabel linkRegister = new LinkLabel
            {
                Text = "Creați un cont aici",
                Location = new Point(145, 0),
                Size = new Size(120, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                LinkColor = Color.FromArgb(61, 102, 255),
                ActiveLinkColor = Color.FromArgb(0, 120, 215),
                VisitedLinkColor = Color.FromArgb(61, 102, 255),
                TextAlign = ContentAlignment.MiddleLeft
            };
            registerPanel.Controls.AddRange(new Control[] { lblNoAccount, linkRegister });

            // Нижняя строка с преимуществами
            Label lblFooter = new Label
            {
                Text = "🚗 Descoperiți Vehicule Premium • Servicii De Încredere • Finanțare Competitivă",
                Size = new Size(500, 30),
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            // События
            btnLogin.Click += (sender, e) =>
            {
                if (txtEmail.Text == "Adresa de email" || string.IsNullOrEmpty(txtEmail.Text) || txtPassword.Text == "Parolă" || string.IsNullOrEmpty(txtPassword.Text))
                {
                    MessageBox.Show("Vă rugăm să completați toate câmpurile!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (_userRepository.ValidateUser(txtEmail.Text, txtPassword.Text))
                {
                    CarListPage carListPage = new CarListPage();
                    this.Hide();
                    carListPage.ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Email sau parolă incorectă!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            linkRegister.Click += (sender, e) =>
            {
                this.Hide();
                RegisterPage registerPage = new RegisterPage();
                registerPage.ShowDialog();
                this.Show();
            };

            // Центрирование и аккуратные отступы
            int totalHeight = 64 + 24 + 20 + 30 + formPanel.Height + 30 + lblFooter.Height; // иконка + отступ + заголовок + подзаголовок + отступ + форма + футер
            int startY = (this.ClientSize.Height - totalHeight) / 2;
            iconBox.Location = new Point((this.ClientSize.Width - iconBox.Width) / 2, startY);
            lblTitle.Location = new Point(0, iconBox.Bottom + 24);
            lblSubtitle.Location = new Point(0, lblTitle.Bottom + 8);
            formPanel.Location = new Point((this.ClientSize.Width - formPanel.Width) / 2, lblSubtitle.Bottom + 30);
            lblFooter.Location = new Point((this.ClientSize.Width - lblFooter.Width) / 2, formPanel.Bottom + 30);

            // Добавление элементов
            formPanel.Controls.AddRange(new Control[] {
                emailPanel, passPanel, btnLogin, registerPanel
            });
            this.Controls.AddRange(new Control[] { iconBox, lblTitle, lblSubtitle, formPanel, lblFooter });

            this.FormClosed += (s, e) => Application.Exit();
        }

        // Вспомогательная функция для скругления углов
        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            // Верхний левый угол
            path.AddArc(arc, 180, 90);
            // Верхний правый угол
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            // Нижний правый угол
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            // Нижний левый угол
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);
    }
} 