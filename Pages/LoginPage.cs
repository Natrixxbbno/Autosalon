using AutoSalon.Pages;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using Npgsql;
using AutoSalon.Database;
using AutoSalon.Database.Repositories;
using AutoSalon.Pages;

namespace AutoSalon.Pages
{
    public class LoginPage : Form
    {
        private readonly UserRepository _userRepository;

        public LoginPage()
        {
            _userRepository = new UserRepository();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Autosalon Premium - Autentificare";
            this.Size = new Size(800, 550);
            this.BackColor = Color.FromArgb(235, 235, 235);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Panel pentru logo și titlu
            Panel headerPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(800, 120),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Adăugare logouri BMW și Audi
            PictureBox picBMW = new PictureBox
            {
                Image = Image.FromFile("bmw.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(20, 20),
                Size = new Size(80, 80),
                BackColor = Color.Transparent
            };
            PictureBox picAudi = new PictureBox
            {
                Image = Image.FromFile("audi.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(700, 20),
                Size = new Size(80, 80),
                BackColor = Color.Transparent
            };

            Label lblTitle = new Label
            {
                Text = "AUTOSALON PREMIUM",
                Location = new Point(150, 20),
                Size = new Size(500, 30),
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblSubtitle = new Label
            {
                Text = "BMW & AUDI",
                Location = new Point(150, 50),
                Size = new Size(500, 20),
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.FromArgb(100, 100, 100),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Panel pentru formular
            Panel formPanel = new Panel
            {
                Location = new Point(150, 150),
                Size = new Size(500, 350),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblUsername = new Label
            {
                Text = "Nume utilizator",
                Location = new Point(50, 30),
                Size = new Size(300, 20),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            TextBox txtUsername = new TextBox
            {
                Location = new Point(50, 60),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(245, 245, 245),
                ForeColor = Color.FromArgb(64, 64, 64),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblPassword = new Label
            {
                Text = "Parolă",
                Location = new Point(50, 120),
                Size = new Size(300, 20),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            TextBox txtPassword = new TextBox
            {
                Location = new Point(50, 150),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 10),
                PasswordChar = '•',
                BackColor = Color.FromArgb(245, 245, 245),
                ForeColor = Color.FromArgb(64, 64, 64),
                BorderStyle = BorderStyle.FixedSingle
            };

            Button btnLogin = new Button
            {
                Text = "AUTENTIFICARE",
                Location = new Point(125, 220),
                Size = new Size(250, 45),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            Button btnRegister = new Button
            {
                Text = "ÎNREGISTRARE",
                Location = new Point(125, 280),
                Size = new Size(250, 45),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(0, 120, 215),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            // Adăugarea evenimentului de click pentru butonul de autentificare
            btnLogin.Click += (sender, e) =>
            {
                if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text))
                {
                    MessageBox.Show("Vă rugăm să completați toate câmpurile!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (_userRepository.ValidateUser(txtUsername.Text, txtPassword.Text))
                {
                    CarListPage carListPage = new CarListPage();
                    this.Hide();
                    carListPage.ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверное имя пользователя или пароль!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            // Adăugarea evenimentului de click pentru butonul de înregistrare
            btnRegister.Click += (sender, e) =>
            {
                RegisterPage registerPage = new RegisterPage();
                registerPage.ShowDialog();
            };

            // Adăugarea efectelor de hover pentru butoane
            btnLogin.MouseEnter += (sender, e) => btnLogin.BackColor = Color.FromArgb(0, 100, 195);
            btnLogin.MouseLeave += (sender, e) => btnLogin.BackColor = Color.FromArgb(0, 120, 215);
            btnRegister.MouseEnter += (sender, e) => btnRegister.BackColor = Color.FromArgb(240, 240, 240);
            btnRegister.MouseLeave += (sender, e) => btnRegister.BackColor = Color.White;

            // Adăugarea controalelor în panouri
            headerPanel.Controls.AddRange(new Control[] { picBMW, lblTitle, lblSubtitle, picAudi });
            formPanel.Controls.AddRange(new Control[] { 
                lblUsername, txtUsername,
                lblPassword, txtPassword,
                btnLogin, btnRegister
            });
            this.Controls.AddRange(new Control[] { headerPanel, formPanel });
        }
    }
} 