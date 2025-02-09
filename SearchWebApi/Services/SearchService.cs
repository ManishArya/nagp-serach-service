using SearchWebApi.Models;
using SearchWebApi.Services.Interfaces;

namespace SearchWebApi.Services
{
    public class SearchService : ISearchService
    {
        public List<Product> products =
        [
            new()
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
                Price = 200,
                DiscountPrice = 100,
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
                Price = 950,
                DiscountPrice = 550,
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
                Price = 1400,
                DiscountPrice = 1100,
                Brand = "Roadster",
                Category = ["clothing", "men-topwear", "men-suits"],
                Colors = ["brown", "blue"],
                Color = "black"
            }
        ];

        async Task<AmcartListResponse<Product>> ISearchService.Search(SearchCriteriaRequest searchRequest)
        {
            SearchCriteriaRequest criteriaRequest = searchRequest ?? new();
            var filteredProducts = products;
            var filterCriteria = criteriaRequest.FilterCriteria;

            if (filterCriteria != null)
            {
                if (!string.IsNullOrEmpty(filterCriteria.SearchText))
                {
                    var searchText = filterCriteria.SearchText;
                    filteredProducts = products.Where(p =>
                    {
                        return (p.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) || (
                                    string.Join("/", p.Category).Contains(searchText, StringComparison.CurrentCultureIgnoreCase)
                        ));
                    }).ToList();
                }

                var priceCriteria = filterCriteria.PriceCriteria;

                if (priceCriteria != null)
                {
                    filteredProducts = filteredProducts.Where(p => (p.DiscountPrice ?? p.Price) >= priceCriteria.MinVal && (p.DiscountPrice ?? p.Price) <= priceCriteria.MaxVal).ToList();
                }

                var colorCriteria = filterCriteria.ColorCriteria;

                if (colorCriteria != null)
                {
                    var values = colorCriteria.Values;
                    filteredProducts = filteredProducts.Where(f => values.Contains(f.Color)).ToList();
                }

                var brandCriteria = filterCriteria.BrandCriteria;

                if (brandCriteria != null)
                {
                    var values = brandCriteria.Values;
                    filteredProducts = filteredProducts.Where(f => values.Contains(f.Brand)).ToList();
                }
            }

            var sortingCriteria = criteriaRequest.SortingCriteria;
            var sortKey = sortingCriteria.SortKey;
            var isDescending = sortingCriteria.IsDescending;

            filteredProducts.Sort((p1, p2) =>
            {
                if(sortKey == SortKeyOption.Price) 
                {
                    var p1Price = p1.DiscountPrice ?? p1.Price;
                    var p2Price = p2.DiscountPrice ?? p2.Price;
                    return isDescending ? (int)(p2Price - p1Price) : (int)(p1Price - p2Price);
                } 
                return p1.Name.CompareTo(p2.Name);
            });

            return await Task.FromResult(new AmcartListResponse<Product>
            {
                Content = new AmcartListContent<Product> { Total = filteredProducts.Count, Records = filteredProducts },
                Status = AmcartRequestStatus.Success,
            });
        }
    }
}
