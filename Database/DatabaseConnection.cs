using System;
using Npgsql;

namespace AutoSalon.Database
{
    public static class DatabaseConnection
    {
        private static readonly string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=autosalon";

        public static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(connectionString);
        }

        public static bool RegisterUser(string firstName, string lastName, string email, string password)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    string sql = "INSERT INTO users (first_name, last_name, email, password) VALUES (@firstName, @lastName, @email, @password)";
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@firstName", firstName);
                        cmd.Parameters.AddWithValue("@lastName", lastName);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
} 