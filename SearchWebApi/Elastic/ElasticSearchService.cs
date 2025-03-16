using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Options;

namespace SearchWebApi.Elastic
{
   
    public class ElasticSearchService
    {
        private readonly ElasticsearchClient _client;
        private readonly ElasticSettings _elasticSettings;

        public ElasticSearchService(IOptions<ElasticSettings> options)
        {
            _elasticSettings = options.Value;
           _client = new ElasticsearchClient(_elasticSettings.CloudId, new ApiKey(_elasticSettings.ApiKey));
        }

        public ElasticsearchClient Client => _client;
    }
}
