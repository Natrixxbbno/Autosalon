using System;
using System.Drawing;
using System.Windows.Forms;
using AutoSalon.Database.Repositories;
using System.Text.RegularExpressions;

namespace AutoSalon.Pages
{
    public class RegisterPage : Form
    {
        private Form _loginForm;

        public RegisterPage(Form loginForm = null)
        {
            _loginForm = loginForm;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "AutoSalon Premium - Înregistrare";
            this.Size = new Size(420, 680);
            this.BackColor = Color.FromArgb(245, 247, 251);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Верхняя панель с иконкой и заголовком
            Panel headerPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(420, 180),
                BackColor = Color.Transparent
            };

            PictureBox picCar = new PictureBox
            {
                Image = Image.FromFile("Resources/car.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point((420 - 64) / 2, 24),
                Size = new Size(64, 64),
                BackColor = Color.Transparent
            };

            Label lblTitle = new Label
            {
                Text = "AutoSalon Premium",
                Location = new Point(0, 100),
                Size = new Size(420, 32),
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            Label lblSubtitle = new Label
            {
                Text = "Creați-vă contul și descoperiți mașina visurilor dvs.",
                Location = new Point(0, 135),
                Size = new Size(420, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            // Градиентный фон для header
            headerPanel.Paint += (s, e) =>
            {
                if (headerPanel.ClientRectangle.Width <= 0 || headerPanel.ClientRectangle.Height <= 0) return;
                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(headerPanel.ClientRectangle,
                    Color.FromArgb(63, 94, 251), Color.FromArgb(70, 204, 252), 90F))
                {
                    e.Graphics.FillRectangle(brush, headerPanel.ClientRectangle);
                }
            };
            headerPanel.Controls.AddRange(new Control[] { picCar, lblTitle, lblSubtitle });

            // Панель формы
            Panel formPanel = new Panel
            {
                Location = new Point(0, 180),
                Size = new Size(420, 420),
                BackColor = Color.White
            };

            // Функция для создания поля с иконкой (кастомные PNG)
            Func<string, string, int, TextBox> createTextBoxWithIcon = (placeholder, iconType, y) =>
            {
                Panel panel = new Panel
                {
                    Location = new Point(40, y),
                    Size = new Size(340, 44),
                    BackColor = Color.White
                };
                PictureBox icon = new PictureBox
                {
                    Size = new Size(24, 24),
                    Location = new Point(8, 10),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = Color.Transparent
                };
                if (iconType == "user")
                    icon.Image = Image.FromFile("Resources/login.png");
                else if (iconType == "email")
                    icon.Image = Image.FromFile("Resources/email.png");
                else if (iconType == "password")
                    icon.Image = Image.FromFile("Resources/password.png");
                TextBox tb = new TextBox
                {
                    Location = new Point(40, 10),
                    Size = new Size(280, 24),
                    Font = new Font("Segoe UI", 10),
                    BorderStyle = BorderStyle.None,
                    ForeColor = Color.FromArgb(40, 40, 40),
                    BackColor = Color.White
                };
                // Только для паролей используем PlaceholderText и скрытый ввод
                if (iconType == "password")
                {
                    tb.PlaceholderText = placeholder;
                    tb.UseSystemPasswordChar = true;
                }
                else
                {
                    tb.Tag = placeholder;
                    tb.GotFocus += (s, e) => { if (tb.Text == placeholder) { tb.Text = ""; tb.ForeColor = Color.Black; } };
                    tb.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(tb.Text)) { tb.Text = placeholder; tb.ForeColor = Color.Gray; } };
                    tb.Text = placeholder;
                    tb.ForeColor = Color.Gray;
                }
                panel.Controls.Add(icon);
                panel.Controls.Add(tb);
                formPanel.Controls.Add(panel);
                return tb;
            };

            var txtFirstName = createTextBoxWithIcon("Prenume", "user", 20);
            var txtLastName = createTextBoxWithIcon("Nume", "user", 74);
            var txtEmail = createTextBoxWithIcon("Adresa de email", "email", 128);
            var txtPassword = createTextBoxWithIcon("Introduceți parola", "password", 182);
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.PlaceholderText = "Introduceți parola";
            var txtConfirmPassword = createTextBoxWithIcon("Confirmați parola", "password", 236);
            txtConfirmPassword.UseSystemPasswordChar = true;
            txtConfirmPassword.PlaceholderText = "Confirmați parola";

            // Кнопка регистрации
            Button btnRegister = new Button
            {
                Text = "Creează cont",
                Size = new Size(340, 44),
                Location = new Point(40, 300),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(63, 94, 251),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.MouseEnter += (s, e) => btnRegister.BackColor = Color.FromArgb(50, 80, 200);
            btnRegister.MouseLeave += (s, e) => btnRegister.BackColor = Color.FromArgb(63, 94, 251);
            formPanel.Controls.Add(btnRegister);

            // Ссылка на вход
            LinkLabel linkSignIn = new LinkLabel
            {
                Text = "Aveți deja un cont? Autentificați-vă aici",
                Location = new Point(90, 355),
                Size = new Size(250, 20),
                Font = new Font("Segoe UI", 9),
                LinkColor = Color.FromArgb(63, 94, 251),
                ActiveLinkColor = Color.FromArgb(50, 80, 200),
                TextAlign = ContentAlignment.MiddleCenter
            };
            formPanel.Controls.Add(linkSignIn);

            // Нижняя подпись
            Label lblFooter = new Label
            {
                Text = "🚗 Descoperiți Vehicule Premium • Servicii De Încredere • Finanțare Competitivă",
                Location = new Point(0, 620),
                Size = new Size(420, 20),
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblFooter);

            // Логика регистрации
            btnRegister.Click += (sender, e) =>
            {
                if (txtFirstName.Text == "Prenume" || txtLastName.Text == "Nume" ||
                    txtEmail.Text == "Adresa de email" || txtPassword.Text == "Introduceți parola" ||
                    txtConfirmPassword.Text == "Confirmați parola" ||
                    string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text) ||
                    string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPassword.Text) ||
                    string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
                {
                    MessageBox.Show("Vă rugăm să completați toate câmpurile!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!ValidateName(txtFirstName.Text))
                {
                    MessageBox.Show("Prenumele trebuie să conțină doar litere și să aibă cel puțin 2 caractere!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!ValidateName(txtLastName.Text))
                {
                    MessageBox.Show("Numele trebuie să conțină doar litere și să aibă cel puțin 2 caractere!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!ValidateEmail(txtEmail.Text))
                {
                    MessageBox.Show("Vă rugăm să introduceți o adresă de email validă!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!ValidatePassword(txtPassword.Text))
                {
                    MessageBox.Show("Parola trebuie să aibă cel puțin 8 caractere, să conțină cel puțin o literă și o cifră!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (txtPassword.Text != txtConfirmPassword.Text)
                {
                    MessageBox.Show("Parolele nu coincid!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    var userRepository = new UserRepository();
                    if (userRepository.RegisterUser(txtFirstName.Text, txtLastName.Text, txtEmail.Text, txtPassword.Text))
                    {
                        MessageBox.Show("Cont creat cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Eroare la salvare: {ex.Message}", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            linkSignIn.LinkClicked += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            // Добавление панелей на форму
            this.Controls.Add(headerPanel);
            this.Controls.Add(formPanel);

        }

        private bool ValidateEmail(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        private bool ValidatePassword(string password)
        {
            string pattern = @"^(?=.*[A-Za-z])(?=.*\d).{8,}$";
            return Regex.IsMatch(password, pattern);
        }

        private bool ValidateName(string name)
        {
            name = name.Trim();
            string pattern = @"^[A-Za-zА-Яа-яЁё\-']{2,}$";
            return Regex.IsMatch(name, pattern);
        }
    }
}
