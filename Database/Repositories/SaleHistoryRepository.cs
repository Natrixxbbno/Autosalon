using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using AutoSalon.Database.Models;

namespace AutoSalon.Database.Repositories
{
    public class SaleHistoryRepository : IRepository<SaleHistory>
    {
        public SaleHistory GetById(int id)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT sh.*, c.model, c.registration_number, m.name as manufacturer_name 
                             FROM sales_history sh 
                             JOIN cars c ON sh.car_id = c.id 
                             JOIN manufacturers m ON c.manufacturer_id = m.id 
                             WHERE sh.id = @id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapSaleHistory(reader);
                        }
                    }
                }
            }
            return null;
        }

        public IEnumerable<SaleHistory> GetAll()
        {
            var sales = new List<SaleHistory>();
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT sh.*, c.model, c.registration_number, m.name as manufacturer_name 
                             FROM sales_history sh 
                             JOIN cars c ON sh.car_id = c.id 
                             JOIN manufacturers m ON c.manufacturer_id = m.id 
                             ORDER BY sh.sale_date DESC";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sales.Add(MapSaleHistory(reader));
                        }
                    }
                }
            }
            return sales;
        }

        public bool Add(SaleHistory sale)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string sql = "INSERT INTO sales_history (car_id, sale_date, sale_price) VALUES (@carId, @saleDate, @salePrice)";
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@carId", sale.CarId);
                        cmd.Parameters.AddWithValue("@saleDate", sale.SaleDate);
                        cmd.Parameters.AddWithValue("@salePrice", sale.SalePrice);
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

        public bool Update(SaleHistory sale)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string sql = "UPDATE sales_history SET car_id = @carId, sale_date = @saleDate, sale_price = @salePrice WHERE id = @id";
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", sale.Id);
                        cmd.Parameters.AddWithValue("@carId", sale.CarId);
                        cmd.Parameters.AddWithValue("@saleDate", sale.SaleDate);
                        cmd.Parameters.AddWithValue("@salePrice", sale.SalePrice);
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
                    string sql = "DELETE FROM sales_history WHERE id = @id";
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

        public IEnumerable<SaleHistory> GetSalesByDateRange(DateTime startDate, DateTime endDate)
        {
            var sales = new List<SaleHistory>();
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT sh.*, c.model, c.registration_number, m.name as manufacturer_name 
                             FROM sales_history sh 
                             JOIN cars c ON sh.car_id = c.id 
                             JOIN manufacturers m ON c.manufacturer_id = m.id 
                             WHERE sh.sale_date BETWEEN @startDate AND @endDate 
                             ORDER BY sh.sale_date DESC";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sales.Add(MapSaleHistory(reader));
                        }
                    }
                }
            }
            return sales;
        }

        public IEnumerable<SaleHistory> GetSalesByManufacturer(string manufacturerName)
        {
            var sales = new List<SaleHistory>();
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT sh.*, c.model, c.registration_number, m.name as manufacturer_name 
                             FROM sales_history sh 
                             JOIN cars c ON sh.car_id = c.id 
                             JOIN manufacturers m ON c.manufacturer_id = m.id 
                             WHERE m.name = @manufacturerName 
                             ORDER BY sh.sale_date DESC";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@manufacturerName", manufacturerName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sales.Add(MapSaleHistory(reader));
                        }
                    }
                }
            }
            return sales;
        }

        public decimal GetTotalSalesByDateRange(DateTime startDate, DateTime endDate)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = "SELECT SUM(sale_price) FROM sales_history WHERE sale_date BETWEEN @startDate AND @endDate";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);
                    var result = cmd.ExecuteScalar();
                    return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
                }
            }
        }

        private SaleHistory MapSaleHistory(NpgsqlDataReader reader)
        {
            return new SaleHistory
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                CarId = reader.GetInt32(reader.GetOrdinal("car_id")),
                SaleDate = reader.GetDateTime(reader.GetOrdinal("sale_date")),
                SalePrice = reader.GetDecimal(reader.GetOrdinal("sale_price")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                Car = new Car
                {
                    Model = reader.GetString(reader.GetOrdinal("model")),
                    RegistrationNumber = reader.GetString(reader.GetOrdinal("registration_number")),
                    Manufacturer = new Manufacturer
                    {
                        Name = reader.GetString(reader.GetOrdinal("manufacturer_name"))
                    }
                }
            };
        }
    }
} 