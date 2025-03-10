using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Clients.Elasticsearch;
using SearchWebApi.Elastic;
using SearchWebApi.Models;
using SearchWebApi.Services.Interfaces;
using SearchWebApi.Utils;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Options;

namespace SearchWebApi.Services
{
    public class SearchService(ElasticSearchService elasticSearchService, ILogger<SearchService> logger, IOptions<ImageSettings> options) : ISearchService
    {
        private readonly ElasticSearchService _elasticSearchService = elasticSearchService;
        private readonly ILogger<SearchService> _logger = logger;
        private readonly ImageSettings _imageSettings = options.Value;
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
                        Size = 10,
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

                ICollection<Query> must = [];

                if (filterCriteria != null)
                {
                    if (!string.IsNullOrEmpty(filterCriteria.Category))
                    {
                        var category = filterCriteria.Category.ToLower();
                        var categoryQuery = Query.Term(new TermQuery(new Field("category")) { Boost = 2, Value = category, CaseInsensitive = true });
                        must.Add(categoryQuery);
                    }

                    else if (!string.IsNullOrEmpty(filterCriteria.SearchText))
                    {
                        var searchText = filterCriteria.SearchText.ToLower();

                        searchText = filterCriteria.SearchText.EscapeCharacters();

                        var split = searchText.Split(' ');
                        var len = split.Length;
                        if (split.Any(s => gender.Contains(s)))
                        {
                            var termQuery = GetTermQuery("gender", [.. split]);
                            must.Add(termQuery);
                        }

                        var multiMatchQuery = Query.
                                                   MultiMatch(new MultiMatchQuery()
                                                   {
                                                       Fields = Fields.FromStrings(["name^2", "brand", "tags", "color", "category", "gender"]),
                                                       Query = searchText,
                                                       MinimumShouldMatch = 2,
                                                       Analyzer= "my_analyzer",
                                                   });
                        var queryString = Query.
                                              QueryString(new QueryStringQuery()
                                              {
                                                  Fields = Fields.FromStrings(["name^2", "brand", "tags", "color", "category", "gender"]),
                                                  Query = "*" + searchText + "*",
                                                  MinimumShouldMatch = 2,
                                                  Analyzer = "my_analyzer"

                                              });

                        must.Add(new BoolQuery() { Should = [multiMatchQuery, queryString] });
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
                        must.Add(rangeQuery);
                    }

                    var colorCriteria = filterCriteria.ColorCriteria;

                    if (colorCriteria != null)
                    {
                        var values = colorCriteria.Values;
                        if (values.Count > 0)
                        {
                            var termQuery = GetTermQuery("color.keyword", values);
                            must.Add(termQuery);
                        }
                    }

                    var brandCriteria = filterCriteria.BrandCriteria;

                    if (brandCriteria != null)
                    {
                        var values = brandCriteria.Values;
                        if (values.Count > 0)
                        {
                            var termQuery = GetTermQuery("brand.keyword", values);
                            must.Add(termQuery);
                        }
                    }
                }

                var request = new SearchRequest("mcart")
                {
                    Size = criteriaRequest.PagingCriteria.Page,
                    From = criteriaRequest.PagingCriteria.CurrentPage - 1,
                    Sort = GetSortOptions(criteriaRequest.SortingCriteria),
                    Query = Query.Bool(new BoolQuery() { Must = must }),
                };

                var response = await _elasticSearchService.Client.SearchAsync<Product>(request);
                AmcartListContent<Product>? content = new();
                AmcartRequestStatus status = AmcartRequestStatus.BadRequest;

                if (response.IsValidResponse)
                {
                    var docs = response.Documents;
                    AppendToImageServerUrl(docs);
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

        async Task<AmcartResponse<List<Product>>> ISearchService.GetBestSellerProducts()
        {
            try
            {
                var amcartResponse = new AmcartResponse<List<Product>>() { Status = AmcartRequestStatus.BadRequest };
                var request = new SearchRequest("mcart")
                {
                    Size = 20,
                    From = 0,
                    Query = Query.Bool(new BoolQuery()
                    {
                        Should = [Query.Match(new MatchQuery(new Field("tags")) { Query = "Bestseller" })]
                    })
                };

                var response = await _elasticSearchService.Client.SearchAsync<Product>(request);

                if (response.IsValidResponse)
                {
                    var docs = response.Documents;
                    AppendToImageServerUrl(docs);
                    amcartResponse.Content = [.. docs];
                    amcartResponse.Status = AmcartRequestStatus.Success;
                }

                return amcartResponse;

            }
            catch (Exception ex)
            {
                _logger.LogError("{Message}", ex.Message);
                return new AmcartResponse<List<Product>> { Status = AmcartRequestStatus.InternalServerError };
            }
        }

        async Task<AmcartResponse<List<Product>>> ISearchService.GetSimillarProducts(int productId, string category)
        {
            try
            {
                var amcartResponse = new AmcartResponse<List<Product>>() { Status = AmcartRequestStatus.BadRequest };
                var request = new SearchRequest("mcart")
                {
                    Size = 10,
                    From = 0,
                    Query = Query.Bool(new BoolQuery()
                    {
                        Must = [Query.Match(new MatchQuery(new Field("category")) { Query = category })],
                        MustNot = [Query.Match(new MatchQuery(new Field("id")) { Query = productId })]
                    })
                };

                var response = await _elasticSearchService.Client.SearchAsync<Product>(request);

                if (response.IsValidResponse)
                {
                    var docs = response.Documents;
                    AppendToImageServerUrl(docs);
                    amcartResponse.Content = [.. docs];
                    amcartResponse.Status = AmcartRequestStatus.Success;
                }

                return amcartResponse;

            }
            catch (Exception ex)
            {
                _logger.LogError("{Message}", ex.Message);
                return new AmcartResponse<List<Product>> { Status = AmcartRequestStatus.InternalServerError };
            }
        }

        private void AppendToImageServerUrl(IReadOnlyCollection<Product> products) 
        {
            foreach (var product in products)
            {
                var imageName = product.ImageUrl;
                product.ImageUrl = string.Concat(_imageSettings.Url, imageName);
            }
        }

        private static ICollection<SortOptions> GetSortOptions(SortingCriteria sortingCriteria)
        {
            var order = sortingCriteria.IsDescending ? SortOrder.Desc : SortOrder.Asc;
            var fieldSort = new FieldSort() { Order = order };
            var sortKey = sortingCriteria.SortKey;

            Field field;

            if (sortKey != SortKeyOption._score)
            {
                fieldSort.NumericType = FieldSortNumericType.Double;
                var expression = ExpressionBuilder.GetExpression<Product, object>(sortKey.ToString());
                field = new Field(expression);
            }
            else
            {
                field = new Field("_score");
            }

            var sortOptions = SortOptions.Field(field, fieldSort);
            return [sortOptions];
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
