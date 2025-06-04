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
                string sql = @"SELECT c.*, m.name as manufacturer_name, m.country 
                             FROM cars c 
                             JOIN manufacturers m ON c.manufacturer_id = m.id 
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
                string sql = @"SELECT c.*, m.name as manufacturer_name, m.country 
                             FROM cars c 
                             JOIN manufacturers m ON c.manufacturer_id = m.id";
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
                string sql = @"SELECT c.*, m.name as manufacturer_name, m.country 
                             FROM cars c 
                             JOIN manufacturers m ON c.manufacturer_id = m.id 
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
                string sql = @"SELECT c.*, m.name as manufacturer_name, m.country 
                             FROM cars c 
                             JOIN manufacturers m ON c.manufacturer_id = m.id 
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
                string sql = @"SELECT c.*, m.name as manufacturer_name, m.country 
                             FROM cars c 
                             JOIN manufacturers m ON c.manufacturer_id = m.id 
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
                string sql = @"SELECT c.*, m.name as manufacturer_name, m.country 
                             FROM cars c 
                             JOIN manufacturers m ON c.manufacturer_id = m.id 
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
                string sql = @"SELECT c.*, m.name as manufacturer_name, m.country 
                             FROM cars c 
                             JOIN manufacturers m ON c.manufacturer_id = m.id 
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
            return new Car
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                ManufacturerId = reader.GetInt32(reader.GetOrdinal("manufacturer_id")),
                Model = reader.GetString(reader.GetOrdinal("model")),
                Year = reader.GetInt32(reader.GetOrdinal("year")),
                Color = reader.GetString(reader.GetOrdinal("color")),
                Price = reader.GetDecimal(reader.GetOrdinal("price")),
                RegistrationNumber = reader.GetString(reader.GetOrdinal("registration_number")),
                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("purchase_date")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                Manufacturer = new Manufacturer
                {
                    Name = reader.GetString(reader.GetOrdinal("manufacturer_name")),
                    Country = reader.GetString(reader.GetOrdinal("country"))
                }
            };
        }
    }
} 