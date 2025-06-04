using System;

namespace AutoSalon.Database.Models
{
    public class Manufacturer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 