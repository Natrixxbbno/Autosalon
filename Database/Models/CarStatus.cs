using System;

namespace AutoSalon.Database.Models
{
    public class CarStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; } // Для отображения цветного бейджа
        public DateTime CreatedAt { get; set; }
    }
} 