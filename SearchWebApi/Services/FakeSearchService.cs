using SearchWebApi.FakeData;
using SearchWebApi.Models;
using SearchWebApi.Services.Interfaces;

namespace SearchWebApi.Services
{
    public class FakeSearchService : ISearchService
    {
        readonly IReadOnlyCollection<Product> products = ProductFakeData.GetProductData();

        public async Task<AmcartListResponse<Product>> Search(SearchCriteriaRequest searchRequest)
        {
            try
            {
                SearchCriteriaRequest criteriaRequest = searchRequest ?? new();
                var filteredProducts = products;
                var filterCriteria = criteriaRequest.FilterCriteria;

                if (filterCriteria != null)
                {
                    if (!string.IsNullOrEmpty(filterCriteria.SearchText))
                    {
                        var searchText = filterCriteria.SearchText;
                        //     request.Query = new Query() { }
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
                        filteredProducts = filteredProducts.Where(p => p.Price >= priceCriteria.MinVal && p.Price <= priceCriteria.MaxVal).ToList();
                    }

                    var colorCriteria = filterCriteria.ColorCriteria;

                    if (colorCriteria != null)
                    {
                        var values = colorCriteria.Values;
                        if (values.Count > 0)
                        {
                            filteredProducts = filteredProducts.Where(f => values.Contains(f.Color)).ToList();
                        }
                    }

                    var brandCriteria = filterCriteria.BrandCriteria;

                    if (brandCriteria != null)
                    {
                        var values = brandCriteria.Values;
                        if (values.Count > 0)
                        {
                            filteredProducts = filteredProducts.Where(f => values.Contains(f.Brand)).ToList();
                        }
                    }
                }

                var sortingCriteria = criteriaRequest.SortingCriteria;
                var sortKey = sortingCriteria.SortKey;
                var isDescending = sortingCriteria.IsDescending;

                //filteredProducts.Sort((p1, p2) =>
                //{
                //    if (sortKey == SortKeyOption.Price)
                //    {

                //        return isDescending ? (int)(p2.Price - p1.Price) : (int)(p1Price - p2.Price);
                //    }
                //    return p1.Name.CompareTo(p2.Name);
                //});

                return await Task.FromResult(new AmcartListResponse<Product>
                {
                    Content = new AmcartListContent<Product> { Total = filteredProducts.Count, Records = [.. filteredProducts] },
                    Status = AmcartRequestStatus.Success,
                });
            }
            catch (Exception ex)
            {
                return new AmcartListResponse<Product>
                {
                    Content = null,
                    Status = AmcartRequestStatus.InternalServerError,
                };
            }
        }

        Task<AmcartResponse<List<Product>>> ISearchService.GetSearchSuggestions(string query)
        {
            throw new NotImplementedException();
        }

        Task<AmcartListResponse<Product>> ISearchService.Search(SearchCriteriaRequest searchRequest)
        {
            throw new NotImplementedException();
        }
    }
}
