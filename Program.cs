using System;
using System.Windows.Forms;
using AutoSalon.Database;
using AutoSalon.Pages;

namespace AutoSalon
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Resolve image paths consistently when launched by double-click or a shortcut.
            System.IO.Directory.SetCurrentDirectory(AppContext.BaseDirectory);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // Inițializarea bazei de date
                DatabaseMigration.InitializeDatabase();

                Application.Run(new LoginPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la pornirea aplicației: {ex.Message}", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
