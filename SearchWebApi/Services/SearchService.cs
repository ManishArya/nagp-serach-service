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
                Name = "Geometric Embroidered Mandarin Collar Mirror Work Cotton Silk Straight Kurta",
                Description = "",
                ImageUrl = "assets/images/kurta.jpg",
                Price = 500,
                Tags = ["Latest style", "Bestseller"],
                Brand = "Roadster",
                Category = ["clothing", "men-topwear", "men-kurta"]
            },
            new()
            {
                Name = "Women Geometric Printed Straight Kurtis",
                Description = "",
                ImageUrl = "assets/images/kurti.jpg",
                Price = 200,
                DiscountPrice = 100,
                Tags = ["Latest style"],
                Brand = "Roadster",
                Category = ["clothing", "fusion-wear", "women-kurti"]
            },
            new()
            {
                Name = "Men Relaxed Fit Pure Cotton Jeans",
                Description = "",
                ImageUrl = "assets/images/jeans.jpg",
                Price = 1200,
                Brand = "Roadster",
                Category = ["clothing", "men-bottomwear", "men-jeans"],
                IsAvailable = false
            },
            new()
            {
                Name = "Men Solid Round Neck Pure Cotton T-shirt",
                Description = "",
                ImageUrl = "assets/images/kurta.jpg",
                Price = 950,
                DiscountPrice = 550,
                Tags = ["New"],
                Brand = "Roadster",
                Category = ["clothing", "men-topwear", "men-tshirts"]
            },
            new()
            {
                Name = "Men Regular Fit Self Design Pure Cotton Casual Shirt",
                Description = "",
                ImageUrl = "assets/images/kurta.jpg",
                Price = 850,
                Brand = "Roadster",
                Category = ["clothing", "men-topwear", "men-casual-shirts"]
            },
            new()
            {
                Name = "Men Checked Single-Breasted Tailored-Fit Two-Piece Suit",
                Description = "",
                ImageUrl = "assets/images/suit.jpg",
                Price = 1400,
                DiscountPrice = 1100,
                Brand = "Roadster",
                Category = ["clothing", "men-topwear", "men-suits"]
            }
        ];

        async Task<AmcartResponse<List<Product>>> ISearchService.Search(SearchCriteriaRequest searchRequest)
        {
            SearchCriteriaRequest criteriaRequest = searchRequest ?? new();

            if (criteriaRequest.FilterCriteria != null) 
            {
                if(!string.IsNullOrEmpty(criteriaRequest.FilterCriteria.SearchText)) 
                {
                    var searchText = criteriaRequest.FilterCriteria.SearchText.ToLower();

                    
                }
            }

            if (criteriaRequest.SortingCriteria != null) 
            {

            }

            return await Task.FromResult(new AmcartResponse<List<Product>>
            {
                Content = products,
                Status = AmcartRequestStatus.Success,
            });
        }
    }
}
