using System;
using System.Drawing;
using System.Windows.Forms;
using AutoSalon.Database.Models;
using AutoSalon.Database.Repositories;
using System.Linq;

namespace AutoSalon.Pages
{
    public class ManageCarStatusesPage : Form
    {
        private DataGridView dgvStatuses;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;

        public ManageCarStatusesPage()
        {
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.AutoScroll = true;
            InitializeComponents();
            LoadStatuses();
        }

        private void InitializeComponents()
        {
            this.Text = "Gestionarea Statusurilor Automobilelor";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.AutoScroll = true;

            dgvStatuses = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(540, 250),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvStatuses.Columns.Add("Id", "ID");
            dgvStatuses.Columns["Id"].Visible = false;
            dgvStatuses.Columns.Add("Name", "Nume");
            dgvStatuses.Columns.Add("Color", "Culoare badge");

            btnAdd = new Button { Text = "Adaugă", Location = new Point(20, 290), Size = new Size(120, 40), BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnEdit = new Button { Text = "Editează", Location = new Point(160, 290), Size = new Size(150, 40), BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnDelete = new Button { Text = "Șterge", Location = new Point(330, 290), Size = new Size(120, 40), BackColor = Color.FromArgb(220, 53, 69), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;

            Panel scrollPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(580, 600),
                AutoScroll = true,
                Dock = DockStyle.Fill
            };
            scrollPanel.Controls.AddRange(new Control[] { dgvStatuses, btnAdd, btnEdit, btnDelete });
            this.Controls.Add(scrollPanel);
        }

        private void LoadStatuses()
        {
            var repo = new CarStatusRepository();
            var statuses = repo.GetAll().ToList();
            dgvStatuses.Rows.Clear();
            foreach (var status in statuses)
            {
                dgvStatuses.Rows.Add(status.Id, status.Name, status.Color);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var dialog = new StatusEditDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var repo = new CarStatusRepository();
                repo.Add(dialog.Status);
                LoadStatuses();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvStatuses.SelectedRows.Count == 0) return;
            int id = Convert.ToInt32(dgvStatuses.SelectedRows[0].Cells["Id"].Value);
            string name = dgvStatuses.SelectedRows[0].Cells["Name"].Value.ToString();
            string color = dgvStatuses.SelectedRows[0].Cells["Color"].Value.ToString();
            var dialog = new StatusEditDialog(new CarStatus { Id = id, Name = name, Color = color });
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var repo = new CarStatusRepository();
                repo.Update(dialog.Status);
                LoadStatuses();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvStatuses.SelectedRows.Count == 0) return;
            int id = Convert.ToInt32(dgvStatuses.SelectedRows[0].Cells["Id"].Value);
            if (MessageBox.Show("Ștergeți statusul selectat?", "Confirmare", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var repo = new CarStatusRepository();
                repo.Delete(id);
                LoadStatuses();
            }
        }
    }

    // Диалог для добавления/редактирования статуса
    public class StatusEditDialog : Form
    {
        public CarStatus Status { get; private set; }
        private TextBox txtName;
        private Button btnColor;
        private Button btnSave;
        private Button btnCancel;
        private ColorDialog colorDialog;
        private string colorHex;

        public StatusEditDialog(CarStatus status = null)
        {
            Status = status ?? new CarStatus();
            colorHex = Status.Color ?? "#cccccc";
            InitializeComponents();
            if (status != null)
            {
                txtName.Text = status.Name;
                btnColor.BackColor = ColorTranslator.FromHtml(status.Color);
            }
        }

        private void InitializeComponents()
        {
            this.Text = "Status";
            this.Size = new Size(350, 220);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label lblName = new Label { Text = "Nume:", Location = new Point(20, 20), Size = new Size(100, 20) };
            txtName = new TextBox { Location = new Point(20, 45), Size = new Size(290, 25) };
            Label lblColor = new Label { Text = "Culoare badge:", Location = new Point(20, 80), Size = new Size(100, 20) };
            btnColor = new Button { Location = new Point(20, 105), Size = new Size(80, 30), BackColor = ColorTranslator.FromHtml(colorHex) };
            btnColor.Click += (s, e) => {
                colorDialog = new ColorDialog();
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    btnColor.BackColor = colorDialog.Color;
                    colorHex = "#" + colorDialog.Color.R.ToString("X2") + colorDialog.Color.G.ToString("X2") + colorDialog.Color.B.ToString("X2");
                }
            };
            btnSave = new Button { Text = "Salvează", Location = new Point(20, 150), Size = new Size(120, 35), BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCancel = new Button { Text = "Anulează", Location = new Point(190, 150), Size = new Size(120, 35), BackColor = Color.FromArgb(200, 200, 200), ForeColor = Color.Black, FlatStyle = FlatStyle.Flat };
            btnSave.Click += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Introduceți numele statusului!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                Status.Name = txtName.Text.Trim();
                Status.Color = colorHex;
                Status.CreatedAt = Status.CreatedAt == default ? DateTime.Now : Status.CreatedAt;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.AddRange(new Control[] { lblName, txtName, lblColor, btnColor, btnSave, btnCancel });
        }
    }
} 