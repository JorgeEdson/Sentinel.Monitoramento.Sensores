using Elastic.Clients.Elasticsearch;
using Sentinel.Monitoramento.Sensores.Modelos;

namespace Sentinel.Monitoramento.Sensores.Services
{
    public class ElasticsearchService(IConfiguration configuration)
    {
        private static readonly string IndexName = "dados_sensores";

        private readonly ElasticsearchClient _client = new(
            new ElasticsearchClientSettings(new Uri(configuration["Elasticsearch:Url"]!))
            .DefaultIndex(IndexName)
        );

        public async Task IndexarDadosAsync(DadosSensor dadosSensor)
        {
            var response = await _client.IndexAsync(dadosSensor, idx => idx.Index(IndexName));

            if (!response.IsValidResponse)
            {
                throw new Exception($"Erro ao indexar no Elasticsearch: {response.DebugInformation}");
            }
        }
    }
}
