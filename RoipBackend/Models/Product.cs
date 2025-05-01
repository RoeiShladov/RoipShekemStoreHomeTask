using Microsoft.EntityFrameworkCore;

namespace RoipBackend.Models
{
    public class Product
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; } = 10;
        public bool IsAvailable { get; set; } = false;
        public int Quantity { get; set; } = 0;
    }
}
