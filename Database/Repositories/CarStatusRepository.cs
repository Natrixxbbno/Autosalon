using System;
using System.Collections.Generic;
using Npgsql;
using AutoSalon.Database.Models;

namespace AutoSalon.Database.Repositories
{
    public class CarStatusRepository
    {
        public IEnumerable<CarStatus> GetAll()
        {
            var statuses = new List<CarStatus>();
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = "SELECT * FROM car_statuses ORDER BY id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            statuses.Add(new CarStatus
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Name = reader["name"].ToString(),
                                Color = reader["color"]?.ToString() ?? "#cccccc",
                                CreatedAt = Convert.ToDateTime(reader["created_at"])
                            });
                        }
                    }
                }
            }
            return statuses;
        }

        public bool Add(CarStatus status)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = "INSERT INTO car_statuses (name, color, created_at) VALUES (@name, @color, @createdAt)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", status.Name);
                    cmd.Parameters.AddWithValue("@color", status.Color ?? "#cccccc");
                    cmd.Parameters.AddWithValue("@createdAt", status.CreatedAt);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Update(CarStatus status)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = "UPDATE car_statuses SET name = @name, color = @color WHERE id = @id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", status.Id);
                    cmd.Parameters.AddWithValue("@name", status.Name);
                    cmd.Parameters.AddWithValue("@color", status.Color ?? "#cccccc");
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Delete(int id)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = "DELETE FROM car_statuses WHERE id = @id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
} 