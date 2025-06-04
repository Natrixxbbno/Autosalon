using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using AutoSalon.Database.Models;

namespace AutoSalon.Database.Repositories
{
    public class ManufacturerRepository : IRepository<Manufacturer>
    {
        public Manufacturer GetById(int id)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = "SELECT * FROM manufacturers WHERE id = @id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapManufacturer(reader);
                        }
                    }
                }
            }
            return null;
        }

        public IEnumerable<Manufacturer> GetAll()
        {
            var manufacturers = new List<Manufacturer>();
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = "SELECT * FROM manufacturers ORDER BY name";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            manufacturers.Add(MapManufacturer(reader));
                        }
                    }
                }
            }
            return manufacturers;
        }

        public bool Add(Manufacturer manufacturer)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string sql = "INSERT INTO manufacturers (name, country) VALUES (@name, @country)";
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", manufacturer.Name);
                        cmd.Parameters.AddWithValue("@country", manufacturer.Country);
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

        public bool Update(Manufacturer manufacturer)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string sql = "UPDATE manufacturers SET name = @name, country = @country WHERE id = @id";
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", manufacturer.Id);
                        cmd.Parameters.AddWithValue("@name", manufacturer.Name);
                        cmd.Parameters.AddWithValue("@country", manufacturer.Country);
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

        public bool Delete(int id)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string sql = "DELETE FROM manufacturers WHERE id = @id";
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
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

        // Дополнительные методы

        public Manufacturer GetByName(string name)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = "SELECT * FROM manufacturers WHERE name = @name";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapManufacturer(reader);
                        }
                    }
                }
            }
            return null;
        }

        public IEnumerable<Manufacturer> GetByCountry(string country)
        {
            var manufacturers = new List<Manufacturer>();
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = "SELECT * FROM manufacturers WHERE country = @country ORDER BY name";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@country", country);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            manufacturers.Add(MapManufacturer(reader));
                        }
                    }
                }
            }
            return manufacturers;
        }

        private Manufacturer MapManufacturer(NpgsqlDataReader reader)
        {
            return new Manufacturer
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Country = reader.GetString(reader.GetOrdinal("country")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
            };
        }
    }
} 