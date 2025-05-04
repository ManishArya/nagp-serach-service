using SearchWebApi.Models;

namespace SearchWebApi.FakeData
{
    public class ProductFakeData
    {
        public static IReadOnlyCollection<Product> GetProductData() 
        {
            return [new()
            {
                Id = 1,
                Name = "Geometric Embroidered Mandarin Collar Mirror Work Cotton Silk Straight Kurta",
                Description = "",
                ImageUrl = "assets/images/kurta.jpg",
                Price = 500,
                Tags = ["Latest style", "Bestseller"],
                Brand = "Roadster",
                Category = ["clothing", "men-topwear", "men-kurta"],
                Colors = ["red", "blue"],
                Color = "orange"
            },
            new()
            {
                Id = 2,
                Name = "Women Geometric Printed Straight Kurtis",
                Description = "",
                ImageUrl = "assets/images/kurti.jpg",
                BasePrice = 200,
                Price = 100,
                Tags = ["Latest style"],
                Brand = "Roadster",
                Category = ["clothing", "fusion-wear", "women-kurti"],
                Colors = ["red", "black"],
                Color = "blue"
            },
            new()
            {
                Id = 3,
                Name = "Men Relaxed Fit Pure Cotton Jeans",
                Description = "",
                ImageUrl = "assets/images/jeans.jpg",
                Price = 1200,
                Brand = "Roadster",
                Category = ["clothing", "men-bottomwear", "men-jeans"],
                IsAvailable = false,
                Colors = ["red", "black"],
                Color = "blue"
            },
            new()
            {
                Id = 4,
                Name = "Men Solid Round Neck Pure Cotton T-shirt",
                Description = "",
                ImageUrl = "assets/images/t-shirt.jpg",
                BasePrice = 950,
                Price = 550,
                Tags = ["New"],
                Brand = "Roadster",
                Category = ["clothing", "men-topwear", "men-tshirts"],
                Colors = ["red", "green"],
                Color = "black"
            },
            new()
            {
                Id=5,
                Name = "Men Regular Fit Self Design Pure Cotton Casual Shirt",
                Description = "",
                ImageUrl = "assets/images/shirt.jpg",
                Price = 850,
                Brand = "Roadster",
                Category = ["clothing", "men-topwear", "men-casual-shirts"],
                Color = "blue"
            },
            new()
            {
                Id = 6,
                Name = "Men Checked Single-Breasted Tailored-Fit Two-Piece Suit",
                Description = "",
                ImageUrl = "assets/images/suit.jpg",
                BasePrice = 1400,
                Price = 1100,
                Brand = "Roadster",
                Category = ["clothing", "men-topwear", "men-suits"],
                Colors = ["brown", "blue"],
                Color = "black"
            }];
        }
    }
}
