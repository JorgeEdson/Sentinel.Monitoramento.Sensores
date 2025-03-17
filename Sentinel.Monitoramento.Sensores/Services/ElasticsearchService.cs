using Elastic.Clients.Elasticsearch;
using Sentinel.Monitoramento.Sensores.Modelos;

namespace Sentinel.Monitoramento.Sensores.Services
{
    public class ElasticsearchService(IConfiguration configuration,ElasticsearchClient elasticsearchClient)
    {
        private readonly ElasticsearchClient _client = elasticsearchClient;
        private readonly string _indexName = configuration["Elasticsearch:Index"] ?? "dados_sensores";

        public async Task IndexarDadosAsync(DadosSensor dadosSensor)
        {
            var response = await _client.IndexAsync(dadosSensor, idx => idx.Index(_indexName));

            if (!response.IsValidResponse)
            {
                throw new Exception($"Erro ao indexar no Elasticsearch: {response.DebugInformation}");
            }
        }
    }
}
