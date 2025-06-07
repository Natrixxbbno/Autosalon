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
            this.Text = "AutoSalon Premium - Register";
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
                Text = "Create your account and discover your dream car",
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

            var txtFirstName = createTextBoxWithIcon("First Name", "user", 20);
            var txtLastName = createTextBoxWithIcon("Last Name", "user", 74);
            var txtEmail = createTextBoxWithIcon("Email Address", "email", 128);
            var txtPassword = createTextBoxWithIcon("Enter your password", "password", 182);
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.PlaceholderText = "Enter your password";
            var txtConfirmPassword = createTextBoxWithIcon("Confirm your password", "password", 236);
            txtConfirmPassword.UseSystemPasswordChar = true;
            txtConfirmPassword.PlaceholderText = "Confirm your password";

            // Кнопка регистрации
            Button btnRegister = new Button
            {
                Text = "Create Account",
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
                Text = "Already have an account? Sign in here",
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
                Text = "🚗 Discover Premium Vehicles • Reliable Service • Competitive Financing",
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
                if (txtFirstName.Text == "First Name" || txtLastName.Text == "Last Name" ||
                    txtEmail.Text == "Email Address" || txtPassword.Text == "Enter your password" ||
                    txtConfirmPassword.Text == "Confirm your password" ||
                    string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text) ||
                    string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPassword.Text) ||
                    string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
                {
                    MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!ValidateName(txtFirstName.Text))
                {
                    MessageBox.Show("Имя должно содержать только буквы и быть не менее 2 символов!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!ValidateName(txtLastName.Text))
                {
                    MessageBox.Show("Фамилия должна содержать только буквы и быть не менее 2 символов!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!ValidateEmail(txtEmail.Text))
                {
                    MessageBox.Show("Пожалуйста, введите корректный email адрес!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!ValidatePassword(txtPassword.Text))
                {
                    MessageBox.Show("Пароль должен быть не менее 8 символов, содержать хотя бы одну букву и одну цифру!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (txtPassword.Text != txtConfirmPassword.Text)
                {
                    MessageBox.Show("Пароли не совпадают!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    var userRepository = new UserRepository();
                    if (userRepository.RegisterUser(txtFirstName.Text, txtLastName.Text, txtEmail.Text, txtPassword.Text))
                    {
                        MessageBox.Show("Аккаунт успешно создан!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            linkSignIn.LinkClicked += (s, e) =>
            {
                this.Hide();
                var loginForm = new LoginPage();
                loginForm.ShowDialog();
                this.Show();
            };

            // Добавление панелей на форму
            this.Controls.Add(headerPanel);
            this.Controls.Add(formPanel);

            this.FormClosed += (s, e) => Application.Exit();
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