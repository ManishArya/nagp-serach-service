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
            var cloudId = string.Concat(_elasticSettings.CloudId, _elasticSettings.CloudId2);
           _client = new ElasticsearchClient(cloudId, new ApiKey(_elasticSettings.ApiKey));
        }

        public ElasticsearchClient Client => _client;
    }
}
