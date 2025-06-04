using System;

namespace AutoSalon.Database.Models
{
    public class SaleHistory
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal SalePrice { get; set; }
        public DateTime CreatedAt { get; set; }

        // Навигационные свойства
        public Car Car { get; set; }
    }
} 