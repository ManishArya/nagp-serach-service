using SearchWebApi.Models;

namespace SearchWebApi.Services.Interfaces
{
    public interface ISearchService
    {
        Task<AmcartListResponse<Product>> Search(SearchCriteriaRequest searchRequest);

        Task<AmcartResponse<List<Product>>> GetSearchSuggestions(string query);
    }
}
