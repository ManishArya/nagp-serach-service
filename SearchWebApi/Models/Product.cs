namespace SearchWebApi.Models
{
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public string Description { get; set; } = string.Empty;

        public required List<string> Category { get; set; }

        public required string Brand { get; set; }

        public List<string> Tags { get; set; } = [];

        public double Price { get; set; }

        public double ? DiscountPrice { get; set; }    

        public required string ImageUrl { get; set; }

        public bool IsAvailable { get; set; } = true;

        public List<string> Colors { get; set; } = [];

        public required string Color { get; set; }

    }
}
