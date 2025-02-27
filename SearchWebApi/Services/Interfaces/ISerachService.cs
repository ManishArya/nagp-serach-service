using SearchWebApi.Models;

namespace SearchWebApi.Services.Interfaces
{
    public interface ISearchService
    {
        Task<AmcartListResponse<Product>> Search(SearchCriteriaRequest searchRequest);

        Task<AmcartResponse<List<ProductSuggestion>>> GetSearchSuggestions(string query);

        Task<AmcartResponse<List<Product>>> GetBestSellerProducts();

        Task<AmcartResponse<List<Product>>> GetSimillarProducts(int productId, string category);
    }
}
