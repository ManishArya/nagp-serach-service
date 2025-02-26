using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Clients.Elasticsearch;
using SearchWebApi.Elastic;
using SearchWebApi.Models;
using SearchWebApi.Services.Interfaces;
using SearchWebApi.Utils;
using System.Collections.ObjectModel;

namespace SearchWebApi.Services
{
    public class SearchService(ElasticSearchService elasticSearchService, ILogger<SearchService> logger) : ISearchService
    {
        private readonly ElasticSearchService _elasticSearchService = elasticSearchService;
        private readonly ILogger<SearchService> _logger = logger;
        private readonly string[] gender = ["men", "women"];
        async Task<AmcartResponse<List<ProductSuggestion>>> ISearchService.GetSearchSuggestions(string query)
        {
            try
            {
                var amcartResponse = new AmcartResponse<List<ProductSuggestion>>() { Status = AmcartRequestStatus.BadRequest };

                if (!string.IsNullOrEmpty(query))
                {
                    query = query
                              .ReplaceUnwantedCharacter()
                                .EscapeCharacters();

                    var wildcardQuery = query + "*";
                    var request = new SearchRequest("suggestions")
                    {
                        Size = 50,
                        From = 0,
                        Query = Query.Bool(new BoolQuery()
                        {
                            Should = [Query.QueryString(new QueryStringQuery()
                                        {
                                            Fields = Fields.FromString("name"),
                                            Query = wildcardQuery
                                         }),
                                      ]
                        })
                    };

                    var response = await _elasticSearchService.Client.SearchAsync<ProductSuggestion>(request);

                    if (response.IsValidResponse)
                    {
                        var docs = response.Documents;
                        amcartResponse.Content = [.. docs];
                        amcartResponse.Status = AmcartRequestStatus.Success;
                    }
                }

                return amcartResponse;

            }
            catch (Exception ex)
            {
                _logger.LogError("{Message}", ex.Message);
                return new AmcartResponse<List<ProductSuggestion>> { Status = AmcartRequestStatus.InternalServerError };
            }
        }

        async Task<AmcartListResponse<Product>> ISearchService.Search(SearchCriteriaRequest searchRequest)
        {
            try
            {
                SearchCriteriaRequest criteriaRequest = searchRequest ?? new();
                var filterCriteria = criteriaRequest.FilterCriteria;

                ICollection<Query> queries = [];

                if (filterCriteria != null)
                {
                    BoolQuery boolQuery = new();

                    if (!string.IsNullOrEmpty(filterCriteria.Category))
                    {
                        var category = filterCriteria.Category.ToLower();
                        var categoryQuery = Query.Term(new TermQuery(new Field("category")) { Boost = 2, Value = category, CaseInsensitive = true });
                        boolQuery.Must = [categoryQuery];
                        queries.Add(boolQuery);

                    }
                    else if (!string.IsNullOrEmpty(filterCriteria.SearchText))
                    {
                        var searchText = filterCriteria.SearchText.EscapeCharacters().ToLower();
                        var multiMatchQuery = Query.
                                                   MultiMatch(new MultiMatchQuery()
                                                   {
                                                       Fields = Fields.FromStrings(["name", "brand", "color", "tags", "category"]),
                                                       Query = searchText,
                                                       Boost = 3
                                                   });

                        var multiMatchFuzzyQuery = Query.
                                                    MultiMatch(new MultiMatchQuery()
                                                    {
                                                        Fields = Fields.FromStrings(["name", "brand", "color", "tags", "category"]),
                                                        Boost = 2,
                                                        Query = searchText,
                                                        Fuzziness = new Fuzziness("AUTO"),
                                                    });
                        var queryString = Query.
                                              QueryString(new QueryStringQuery()
                                              {
                                                  Fields = Fields.FromStrings(["name", "brand", "tags", "color", "category"]),
                                                  Query = "*" + searchText + "*"
                                              });
                        if(gender.Contains(searchText))
                        {
                            boolQuery.Must = [Query.Match(new MatchQuery(new Field("gender")) { Query = searchText})];
                        }

                        boolQuery.Should = [multiMatchFuzzyQuery, multiMatchQuery, queryString];
                        queries.Add(boolQuery);
                    }

                    var priceCriteria = filterCriteria.PriceCriteria;
                    if (priceCriteria != null)
                    {
                        var maxPrice = priceCriteria.MaxVal;
                        var minPrice = priceCriteria.MinVal;
                        var numberRangeQuery = new NumberRangeQuery(new Field("price"));

                        if (maxPrice != null & minPrice == null)
                        {
                            numberRangeQuery.Gte = maxPrice;
                        }
                        else if (minPrice != null && maxPrice == null)
                        {
                            numberRangeQuery.Lte = minPrice;
                        }
                        else
                        {
                            numberRangeQuery.Lte = maxPrice;
                            numberRangeQuery.Gte = minPrice;
                        }

                        var rangeQuery = Query.Range(numberRangeQuery);
                        queries.Add(rangeQuery);
                    }

                    var colorCriteria = filterCriteria.ColorCriteria;

                    if (colorCriteria != null)
                    {
                        var values = colorCriteria.Values;
                        if (values.Count > 0)
                        {
                            var termQuery = GetTermQuery("color", values);
                            queries.Add(termQuery);
                        }
                    }

                    var brandCriteria = filterCriteria.BrandCriteria;

                    if (brandCriteria != null)
                    {
                        var values = brandCriteria.Values;
                        if (values.Count > 0)
                        {
                            var termQuery = GetTermQuery("brand", values);
                            queries.Add(termQuery);
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
                    Query = Query.Bool(new BoolQuery() { Must = queries }),
                };

                var response = await _elasticSearchService.Client.SearchAsync<Product>(request);
                AmcartListContent<Product>? content = new();
                AmcartRequestStatus status = AmcartRequestStatus.BadRequest;

                if (response.IsValidResponse)
                {
                    var docs = response.Documents;
                    content.Records = [.. docs];
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
                _logger.LogError("{Message}", ex.Message);
                return new AmcartListResponse<Product>
                {
                    Content = null,
                    Status = AmcartRequestStatus.InternalServerError,
                };
            }
        }

        private static Query GetTermQuery(string field, List<string> values)
        {
            var fieldValues = values.Select(v => FieldValue.String(v)).ToList();
            return Query.Terms(new TermsQuery()
            {
                Field = new Field(field),
                Terms = new TermsQueryField(new ReadOnlyCollection<FieldValue>(fieldValues))
            });
        }
    }
}
