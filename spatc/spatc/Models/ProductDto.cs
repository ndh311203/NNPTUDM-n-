namespace spatc.Models
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public string? Image { get; set; }
        public decimal Rating { get; set; }
        public int SoldCount { get; set; }
        public string Type { get; set; } = string.Empty; // "pet", "food", "accessory"
        public int ReviewCount { get; set; }
        public string? Brand { get; set; }
        public string? Species { get; set; }
        public string? Category { get; set; }
        public string? ProductType { get; set; }
    }
}

