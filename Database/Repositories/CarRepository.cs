using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Npgsql;
using AutoSalon.Database.Models;

namespace AutoSalon.Database.Repositories
{
    public class CarRepository : IRepository<Car>
    {
        public Car GetById(int id)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT c.*, m.name as manufacturer_name, m.country, s.name as status_name, s.color as status_color 
                             FROM cars c 
                             JOIN manufacturers m ON c.manufacturer_id = m.id 
                             LEFT JOIN car_statuses s ON c.status_id = s.id 
                             WHERE c.id = @id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapCar(reader);
                        }
                    }
                }
            }
            return null;
        }

        public IEnumerable<Car> GetAll()
        {
            var cars = new List<Car>();
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT c.*, m.name as manufacturer_name, m.country, s.name as status_name, s.color as status_color 
                             FROM cars c 
                             JOIN manufacturers m ON c.manufacturer_id = m.id
                             LEFT JOIN car_statuses s ON c.status_id = s.id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cars.Add(MapCar(reader));
                        }
                    }
                }
            }
            return cars;
        }

        public bool Add(Car car)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string sql = @"INSERT INTO cars 
                        (manufacturer_id, model, year, color, price, registration_number, purchase_date) 
                        VALUES (@manufacturerId, @model, @year, @color, @price, @registrationNumber, @purchaseDate)";
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@manufacturerId", car.ManufacturerId);
                        cmd.Parameters.AddWithValue("@model", car.Model);
                        cmd.Parameters.AddWithValue("@year", car.Year);
                        cmd.Parameters.AddWithValue("@color", car.Color);
                        cmd.Parameters.AddWithValue("@price", car.Price);
                        cmd.Parameters.AddWithValue("@registrationNumber", car.RegistrationNumber);
                        cmd.Parameters.AddWithValue("@purchaseDate", car.PurchaseDate);
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

        public bool Update(Car car)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string sql = @"UPDATE cars 
                        SET manufacturer_id = @manufacturerId,
                            model = @model,
                            year = @year,
                            color = @color,
                            price = @price,
                            registration_number = @registrationNumber,
                            purchase_date = @purchaseDate
                        WHERE id = @id";
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", car.Id);
                        cmd.Parameters.AddWithValue("@manufacturerId", car.ManufacturerId);
                        cmd.Parameters.AddWithValue("@model", car.Model);
                        cmd.Parameters.AddWithValue("@year", car.Year);
                        cmd.Parameters.AddWithValue("@color", car.Color);
                        cmd.Parameters.AddWithValue("@price", car.Price);
                        cmd.Parameters.AddWithValue("@registrationNumber", car.RegistrationNumber);
                        cmd.Parameters.AddWithValue("@purchaseDate", car.PurchaseDate);
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
                    string sql = "DELETE FROM cars WHERE id = @id";
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

        // Дополнительные методы для специфических запросов

        public IEnumerable<Car> GetCarsByManufacturer(string manufacturerName)
        {
            var cars = new List<Car>();
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT c.*, m.name as manufacturer_name, m.country, s.name as status_name, s.color as status_color 
                             FROM cars c 
                             JOIN manufacturers m ON c.manufacturer_id = m.id 
                             LEFT JOIN car_statuses s ON c.status_id = s.id 
                             WHERE m.name = @manufacturerName";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@manufacturerName", manufacturerName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cars.Add(MapCar(reader));
                        }
                    }
                }
            }
            return cars;
        }

        public IEnumerable<Car> GetCarsByColor(string color)
        {
            var cars = new List<Car>();
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT c.*, m.name as manufacturer_name, m.country, s.name as status_name, s.color as status_color 
                             FROM cars c 
                             JOIN manufacturers m ON c.manufacturer_id = m.id 
                             LEFT JOIN car_statuses s ON c.status_id = s.id 
                             WHERE LOWER(c.color) = LOWER(@color)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@color", color);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cars.Add(MapCar(reader));
                        }
                    }
                }
            }
            return cars;
        }

        public IEnumerable<Car> GetCarsByYear(int year)
        {
            var cars = new List<Car>();
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT c.*, m.name as manufacturer_name, m.country, s.name as status_name, s.color as status_color 
                             FROM cars c 
                             JOIN manufacturers m ON c.manufacturer_id = m.id 
                             LEFT JOIN car_statuses s ON c.status_id = s.id 
                             WHERE c.year = @year";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@year", year);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cars.Add(MapCar(reader));
                        }
                    }
                }
            }
            return cars;
        }

        public IEnumerable<Car> GetCarsByPriceRange(decimal minPrice, decimal maxPrice)
        {
            var cars = new List<Car>();
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT c.*, m.name as manufacturer_name, m.country, s.name as status_name, s.color as status_color 
                             FROM cars c 
                             JOIN manufacturers m ON c.manufacturer_id = m.id 
                             LEFT JOIN car_statuses s ON c.status_id = s.id 
                             WHERE c.price BETWEEN @minPrice AND @maxPrice";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@minPrice", minPrice);
                    cmd.Parameters.AddWithValue("@maxPrice", maxPrice);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cars.Add(MapCar(reader));
                        }
                    }
                }
            }
            return cars;
        }

        public Car GetCarByRegistrationNumber(string registrationNumber)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT c.*, m.name as manufacturer_name, m.country, s.name as status_name, s.color as status_color 
                             FROM cars c 
                             JOIN manufacturers m ON c.manufacturer_id = m.id 
                             LEFT JOIN car_statuses s ON c.status_id = s.id 
                             WHERE c.registration_number = @registrationNumber";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@registrationNumber", registrationNumber);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapCar(reader);
                        }
                    }
                }
            }
            return null;
        }

        private Car MapCar(NpgsqlDataReader reader)
        {
            var car = new Car
            {
                Id = Convert.ToInt32(reader["id"]),
                ManufacturerId = Convert.ToInt32(reader["manufacturer_id"]),
                Model = reader["model"].ToString(),
                Year = Convert.ToInt32(reader["year"]),
                Color = reader["color"].ToString(),
                Price = Convert.ToDecimal(reader["price"]),
                RegistrationNumber = reader["registration_number"].ToString(),
                Mileage = reader["mileage"] != DBNull.Value ? Convert.ToInt32(reader["mileage"]) : 0,
                StatusId = reader["status_id"] != DBNull.Value ? Convert.ToInt32(reader["status_id"]) : 0,
                PurchaseDate = Convert.ToDateTime(reader["purchase_date"]),
                CreatedAt = Convert.ToDateTime(reader["created_at"]),
                Manufacturer = new Manufacturer
                {
                    Id = Convert.ToInt32(reader["manufacturer_id"]),
                    Name = reader["manufacturer_name"].ToString(),
                    Country = reader["country"].ToString(),
                    CreatedAt = Convert.ToDateTime(reader["created_at"])
                },
                Status = new CarStatus
                {
                    Id = reader["status_id"] != DBNull.Value ? Convert.ToInt32(reader["status_id"]) : 0,
                    Name = reader["status_name"]?.ToString() ?? string.Empty,
                    Color = reader["status_color"]?.ToString() ?? "#cccccc",
                    CreatedAt = DateTime.Now // или reader["status_created_at"] если есть
                }
            };
            return car;
        }
    }
} 