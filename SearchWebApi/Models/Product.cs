namespace SearchWebApi.Models
{
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public required string Description { get; set; }

        public required List<string> Category { get; set; }

        public required string Brand { get; set; }

        public List<string> Tags { get; set; } = [];

        public int Price { get; set; }

        public int ? DiscountPrice { get; set; }    

        public required string ImageUrl { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}
