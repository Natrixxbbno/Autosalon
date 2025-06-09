using System;
using Npgsql;
using AutoSalon.Database;
using BCrypt.Net;
using System.Windows.Forms;

namespace AutoSalon.Database.Repositories
{
    public class UserRepository
    {
        public bool IsEmailExists(string email)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string sql = "SELECT COUNT(*) FROM users WHERE email = @email";
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la verificarea email-ului: {ex.Message}", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool ValidateUser(string email, string password)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string sql = "SELECT password FROM users WHERE email = @email";
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string hashedPassword = reader.GetString(0);
                                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
                            }
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la validarea utilizatorului: {ex.Message}", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool RegisterUser(string firstName, string lastName, string email, string password)
        {
            try
            {
                if (IsEmailExists(email))
                {
                    MessageBox.Show("Un utilizator cu acest email există deja!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string sql = @"INSERT INTO users (first_name, last_name, email, password, created_at) 
                                 VALUES (@firstName, @lastName, @email, @password, @createdAt)";
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@firstName", firstName);
                        cmd.Parameters.AddWithValue("@lastName", lastName);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@password", hashedPassword);
                        cmd.Parameters.AddWithValue("@createdAt", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la înregistrarea utilizatorului: {ex.Message}", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
} 