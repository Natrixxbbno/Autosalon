using System.Drawing;
using System.Windows.Forms;
using AutoSalon.Database.Repositories;

namespace AutoSalon.Pages
{
    public class OrderReportsPage : Form
    {
        private bool loading;
        private readonly DataGridView orders = new()
        {
            Dock = DockStyle.Fill, ReadOnly = false, AutoGenerateColumns = false,
            AllowUserToAddRows = false, AllowUserToDeleteRows = false,
            RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
            BackgroundColor = Color.White, BorderStyle = BorderStyle.None
        };
        private readonly Label status = new() { AutoSize = true, Margin = new Padding(15, 10, 0, 0) };

        public OrderReportsPage()
        {
            Text = "AutoSalon - Reports - Toate comenzile";
            Size = new Size(1200, 650);
            MinimumSize = new Size(750, 400);
            StartPosition = FormStartPosition.CenterParent;
            WindowState = FormWindowState.Maximized;
            Font = new Font("Segoe UI", 10);
            orders.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            orders.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;

            void AddColumn(string field, string title)
            {
                orders.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = field, DataPropertyName = field, HeaderText = title, ReadOnly = true
                });
            }
            AddColumn("id", "Nr. comandă");
            AddColumn("order_type", "Tip comandă");
            AddColumn("last_name", "Nume");
            AddColumn("first_name", "Prenume");
            AddColumn("contact_number", "Nr. de contact");
            AddColumn("email", "Email");
            AddColumn("category", "Categorie (marcă)");
            AddColumn("product", "Produs (automobil)");
            AddColumn("registration_number", "Nr. de înmatriculare");
            AddColumn("quantity", "Cantitate");
            AddColumn("payment_method", "Metodă de plată");
            AddColumn("created_at", "Data comenzii");
            AddColumn("car_status", "Statut mașină");
            orders.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "completed", DataPropertyName = "completed", HeaderText = "Finalizată (Sold)"
            });
            // Give longer fields more room while keeping every column visible.
            foreach (DataGridViewColumn column in orders.Columns)
            {
                column.MinimumWidth = 35;
                column.FillWeight = column.Name switch
                {
                    "id" => 50,
                    "quantity" => 55,
                    "email" => 185,
                    "registration_number" => 120,
                    "created_at" => 125,
                    "product" => 110,
                    "completed" => 80,
                    _ => 95
                };
            }
            orders.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (orders.CurrentCell is DataGridViewCheckBoxCell && orders.IsCurrentCellDirty)
                    orders.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };
            orders.CellValueChanged += (s, e) =>
            {
                if (loading || e.RowIndex < 0 || e.ColumnIndex != orders.Columns["completed"].Index) return;
                var row = orders.Rows[e.RowIndex];
                if (!Equals(row.Cells["completed"].Value, true)) return;
                try
                {
                    new OrderRepository().Complete(Convert.ToInt32(row.Cells["id"].Value));
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "Comanda nu a fost finalizată", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception)
                {
                    MessageBox.Show("Comanda nu a putut fi finalizată. Încercați din nou.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                BeginInvoke(new Action(LoadOrders));
            };
            orders.Columns["created_at"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

            var toolbar = new FlowLayoutPanel
            {
                Dock = DockStyle.Top, Height = 58, Padding = new Padding(10),
                BackColor = Color.FromArgb(241, 244, 248)
            };
            var refresh = new Button { Text = "Actualizează", AutoSize = true };
            refresh.Click += (s, e) => LoadOrders();
            toolbar.Controls.Add(refresh);
            var details = new Button { Text = "Detalii trade-in", AutoSize = true };
            details.Click += (s, e) =>
            {
                if (orders.CurrentRow?.DataBoundItem is not System.Data.DataRowView selected || selected["trade_in_model"] == DBNull.Value)
                {
                    MessageBox.Show("Selectați o comandă Trade-in din listă.", "Trade-in");
                    return;
                }
                using var form = new TradeInDetailsPage(selected);
                form.ShowDialog(this);
                LoadOrders();
            };
            toolbar.Controls.Add(details);
            var contracts = new Button { Text = "Credite și închirieri", AutoSize = true };
            contracts.Click += (s, e) => { using var form = new ContractReportsPage(); form.ShowDialog(this); LoadOrders(); };
            toolbar.Controls.Add(contracts);
            toolbar.Controls.Add(status);
            Controls.Add(orders);
            Controls.Add(toolbar);
            Load += (s, e) => LoadOrders();
        }

        private void LoadOrders()
        {
            loading = true;
            try
            {
                var table = new OrderRepository().GetAll();
                // PostgreSQL marks the calculated boolean as read-only in the result schema.
                // Allow local checkbox edits; Complete() persists the actual status change.
                table.Columns["completed"]!.ReadOnly = false;
                orders.DataSource = table;
                foreach (DataGridViewRow row in orders.Rows)
                    row.Cells["completed"].ReadOnly = Equals(row.Cells["completed"].Value, true);
                status.Text = table.Rows.Count == 0 ? "Nu există comenzi înregistrate." : $"Total comenzi: {table.Rows.Count}";
            }
            catch (Exception)
            {
                status.Text = "Lista nu a putut fi actualizată.";
                MessageBox.Show("Comenzile nu au putut fi încărcate. Verificați conexiunea la baza de date și încercați din nou.",
                    "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { loading = false; }
        }
    }
}
