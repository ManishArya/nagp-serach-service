using SearchWebApi.Models;

namespace SearchWebApi.Services.Interfaces
{
    public interface ISearchService
    {
        Task<AmcartResponse<List<Product>>> Search(SearchCriteriaRequest searchRequest);
    }
}
