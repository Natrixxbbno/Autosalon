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