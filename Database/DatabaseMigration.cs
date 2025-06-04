using System;
using System.IO;
using System.Linq;
using Npgsql;

namespace AutoSalon.Database
{
    public static class DatabaseMigration
    {
        private static readonly string DefaultConnectionString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=postgres";

        public static void InitializeDatabase()
        {
            // First connect to default postgres database to create our database
            using (var conn = new NpgsqlConnection(DefaultConnectionString))
            {
                conn.Open();
                CreateDatabaseIfNotExists(conn);
            }

            // Then connect to our database to create tables
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                ExecuteMigrations(conn);
            }
        }

        private static void CreateDatabaseIfNotExists(NpgsqlConnection conn)
        {
            string checkDbSql = "SELECT 1 FROM pg_database WHERE datname = 'autosalon'";
            using (var cmd = new NpgsqlCommand(checkDbSql, conn))
            {
                var result = cmd.ExecuteScalar();
                if (result == null)
                {
                    string createDbSql = "CREATE DATABASE autosalon";
                    using (var createCmd = new NpgsqlCommand(createDbSql, conn))
                    {
                        createCmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void ExecuteMigrations(NpgsqlConnection conn)
        {
            string migrationsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "migrations");
            var migrationFiles = Directory.GetFiles(migrationsPath, "*.sql")
                                        .OrderBy(f => f);

            foreach (var migrationFile in migrationFiles)
            {
                string migrationScript = File.ReadAllText(migrationFile);
                using (var cmd = new NpgsqlCommand(migrationScript, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
} 