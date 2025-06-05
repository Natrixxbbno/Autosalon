using System;
using System.Drawing;
using System.Windows.Forms;
using AutoSalon.Database.Repositories;
using System.Text.RegularExpressions;

namespace AutoSalon.Pages
{
    public class RegisterPage : Form
    {
        public RegisterPage()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Autosalon Premium - Înregistrare";
            this.Size = new Size(850, 800);
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
                Text = "CREARE CONT NOU",
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

            // Buton ÎNAPOI (doar în header, lângă logo BMW)
            Button btnInapoi = new Button
            {
                Text = "ÎNAPOI",
                Size = new Size(90, 35),
                Location = new Point(110, 40), // lângă logo BMW
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(0, 120, 215),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnInapoi.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215);
            btnInapoi.FlatAppearance.BorderSize = 1;
            btnInapoi.Click += (sender, e) => this.Close();
            headerPanel.Controls.Add(btnInapoi);

            // Panel pentru formular
            Panel formPanel = new Panel
            {
                Location = new Point(150, 170),
                Size = new Size(550, 600),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Câmpuri pentru înregistrare
            Label lblNume = new Label
            {
                Text = "Nume",
                Location = new Point(50, 30),
                Size = new Size(300, 20),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            TextBox txtNume = new TextBox
            {
                Location = new Point(50, 60),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(245, 245, 245),
                ForeColor = Color.FromArgb(64, 64, 64),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblPrenume = new Label
            {
                Text = "Prenume",
                Location = new Point(50, 120),
                Size = new Size(300, 20),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            TextBox txtPrenume = new TextBox
            {
                Location = new Point(50, 150),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(245, 245, 245),
                ForeColor = Color.FromArgb(64, 64, 64),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblEmail = new Label
            {
                Text = "Email",
                Location = new Point(50, 210),
                Size = new Size(300, 20),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            TextBox txtEmail = new TextBox
            {
                Location = new Point(50, 240),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(245, 245, 245),
                ForeColor = Color.FromArgb(64, 64, 64),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblParola = new Label
            {
                Text = "Parolă",
                Location = new Point(50, 300),
                Size = new Size(300, 20),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            TextBox txtParola = new TextBox
            {
                Location = new Point(50, 330),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 10),
                PasswordChar = '•',
                BackColor = Color.FromArgb(245, 245, 245),
                ForeColor = Color.FromArgb(64, 64, 64),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblConfirmareParola = new Label
            {
                Text = "Confirmare parolă",
                Location = new Point(50, 390),
                Size = new Size(300, 20),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            TextBox txtConfirmareParola = new TextBox
            {
                Location = new Point(50, 420),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 10),
                PasswordChar = '•',
                BackColor = Color.FromArgb(245, 245, 245),
                ForeColor = Color.FromArgb(64, 64, 64),
                BorderStyle = BorderStyle.FixedSingle
            };

            Button btnInregistrare = new Button
            {
                Text = "CREARE CONT",
                Size = new Size(320, 50),
                Location = new Point((550 - 320) / 2, 500),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            // Adăugarea evenimentului de click pentru butonul de înregistrare
            btnInregistrare.Click += (sender, e) =>
            {
                if (string.IsNullOrEmpty(txtNume.Text) || string.IsNullOrEmpty(txtPrenume.Text) ||
                    string.IsNullOrEmpty(txtEmail.Text) || string.IsNullOrEmpty(txtParola.Text) ||
                    string.IsNullOrEmpty(txtConfirmareParola.Text))
                {
                    MessageBox.Show("Vă rugăm să completați toate câmpurile!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidateName(txtNume.Text))
                {
                    MessageBox.Show("Numele trebuie să conțină doar litere și să aibă cel puțin 2 caractere!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidateName(txtPrenume.Text))
                {
                    MessageBox.Show("Prenumele trebuie să conțină doar litere și să aibă cel puțin 2 caractere!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidateEmail(txtEmail.Text))
                {
                    MessageBox.Show("Vă rugăm să introduceți o adresă de email validă!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidatePassword(txtParola.Text))
                {
                    MessageBox.Show("Parola trebuie să aibă cel puțin 8 caractere, să conțină cel puțin o literă și o cifră!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtParola.Text != txtConfirmareParola.Text)
                {
                    MessageBox.Show("Parolele nu coincid!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    var userRepository = new UserRepository();
                    if (userRepository.RegisterUser(txtNume.Text, txtPrenume.Text, txtEmail.Text, txtParola.Text))
                    {
                        MessageBox.Show("Cont creat cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Eroare la crearea contului!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Eroare la salvare: {ex.Message}", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            // Adăugarea efectului de hover pentru buton
            btnInregistrare.MouseEnter += (sender, e) => btnInregistrare.BackColor = Color.FromArgb(0, 100, 195);
            btnInregistrare.MouseLeave += (sender, e) => btnInregistrare.BackColor = Color.FromArgb(0, 120, 215);

            // Adăugarea controalelor în panouri
            headerPanel.Controls.AddRange(new Control[] { picBMW, lblTitle, lblSubtitle, picAudi });
            formPanel.Controls.AddRange(new Control[] { 
                lblNume, txtNume,
                lblPrenume, txtPrenume,
                lblEmail, txtEmail,
                lblParola, txtParola,
                lblConfirmareParola, txtConfirmareParola,
                btnInregistrare 
            });
            this.Controls.AddRange(new Control[] { headerPanel, formPanel });
        }

        private bool ValidateEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";
            return Regex.IsMatch(email, pattern);
        }

        private bool ValidatePassword(string password)
        {
            // Минимум 8 символов, хотя бы одна цифра и одна буква
            string pattern = @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$";
            return Regex.IsMatch(password, pattern);
        }

        private bool ValidateName(string name)
        {
            // Только буквы, минимум 2 символа
            string pattern = @"^[A-Za-zА-Яа-я]{2,}$";
            return Regex.IsMatch(name, pattern);
        }
    }
} 