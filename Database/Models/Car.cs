using System;
using AutoSalon.Database.Models;

namespace AutoSalon.Database.Models
{
    public class Car
    {
        public int Id { get; set; }
        public int ManufacturerId { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }
        public string RegistrationNumber { get; set; }
        public int Mileage { get; set; } // Пробег
        public int StatusId { get; set; } // Статус
        public DateTime PurchaseDate { get; set; }
        public DateTime CreatedAt { get; set; }

        // Навигационные свойства
        public Manufacturer Manufacturer { get; set; }
        public CarStatus Status { get; set; }
    }
} 