using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Clients.Elasticsearch;
using SearchWebApi.Elastic;
using SearchWebApi.Models;
using SearchWebApi.Services.Interfaces;
using SearchWebApi.Utils;
using System.Collections.ObjectModel;

namespace SearchWebApi.Services
{
    public class SearchService(ElasticSearchService elasticSearchService) : ISearchService
    {
        private readonly ElasticSearchService _elasticSearchService = elasticSearchService;

        async Task<AmcartListResponse<Product>> ISearchService.Search(SearchCriteriaRequest searchRequest)
        {
            try
            {
                SearchCriteriaRequest criteriaRequest = searchRequest ?? new();
                var filterCriteria = criteriaRequest.FilterCriteria;

                ICollection<Query> queries = [];

                if (filterCriteria != null)
                {
                    if (!string.IsNullOrEmpty(filterCriteria.SearchText))
                    {
                        var searchText = filterCriteria.SearchText.EscapeCharacters().ToLower();
                        queries.Add(Query.QueryString(new QueryStringQuery() 
                                    { Fields = Fields.FromStrings(["name", "brand", "tags"]), Query = "*"+searchText+"*"}));
                    }

                    var priceCriteria = filterCriteria.PriceCriteria;

                    if (priceCriteria != null)
                    {
                        queries.Add(Query.Range(new NumberRangeQuery(new Field("price")) { Gte = priceCriteria.MinVal, Lte = priceCriteria.MaxVal }));
                    }

                    var colorCriteria = filterCriteria.ColorCriteria;

                    if (colorCriteria != null)
                    {
                        var values = colorCriteria.Values;
                        if (values.Count > 0)
                        {
                            var fieldValues = values.Select(v => FieldValue.String(v)).ToList();
                            queries.Add(Query.Terms(new TermsQuery() { Field = new Field("color"), Terms = new TermsQueryField(new ReadOnlyCollection<FieldValue>(fieldValues)) })); 
                        }
                    }

                    var brandCriteria = filterCriteria.BrandCriteria;

                    if (brandCriteria != null)
                    {
                        var values = brandCriteria.Values;
                        if (values.Count > 0)
                        {
                            var fieldValues = values.Select(v => FieldValue.String(v)).ToList();
                            queries.Add(Query.Terms(new TermsQuery() { Field = new Field("brand"), Terms = new TermsQueryField(new ReadOnlyCollection<FieldValue>(fieldValues)) }));
                        }
                    }
                }

                var sortingCriteria = criteriaRequest.SortingCriteria;
                var sortKey = sortingCriteria.SortKey;
                var order = sortingCriteria.IsDescending ? SortOrder.Desc : SortOrder.Asc;
                var expression = ExpressionBuilder.GetExpression<Product, object>(sortKey.ToString());

                var request = new SearchRequest("mcart")
                {
                    Size = criteriaRequest.PagingCriteria.Page,
                    From = criteriaRequest.PagingCriteria.CurrentPage - 1,
                    Sort = [SortOptions.Field(new Field(expression), new FieldSort() { Order = order, NumericType = FieldSortNumericType.Double })],
                    Query = Query.Bool(new BoolQuery() { Must = queries })
                };

                var response = await _elasticSearchService.Client.SearchAsync<Product>(request);
                AmcartListContent<Product>? content = new();
                AmcartRequestStatus status = AmcartRequestStatus.BadRequest;

                if (response.IsValidResponse)
                {
                    var docs = response.Documents;
                    content.Records = [..docs];
                    content.Total = docs.Count;
                    status = AmcartRequestStatus.Success;
                }

                return new AmcartListResponse<Product>
                {
                    Content = content,
                    Status = status,
                };
            }
            catch (Exception ex)
            {
                return new AmcartListResponse<Product>
                {
                    Content = null,
                    Status = AmcartRequestStatus.Error,
                };
            }
        }
    }
}
