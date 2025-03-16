using RabbitMQ.Client;
using Sentinel.Monitoramento.Sensores.Modelos;
using System.Text.Json;
using System.Text;

namespace Sentinel.Monitoramento.Sensores.Services
{
    public class RabbitMQService(IConnection connection)
    {
        private static readonly string ExchangeName = "sensor-data-exchange";
        private static readonly string RoutingKey = "sensor-data-routing-key";

        private readonly IModel _channel = connection.CreateModel();

        public async Task<DadosSensor> EnviarParaExchangeAsync(DadosSensor dadosSensor)
        {
            try 
            {
                var message = JsonSerializer.Serialize(dadosSensor);
                var body = Encoding.UTF8.GetBytes(message);

                _channel.BasicPublish(
                    exchange: ExchangeName,
                    routingKey: RoutingKey,
                    basicProperties: null,
                    body: body
                );

                dadosSensor.Enviado = true;

                return dadosSensor;
            }
            catch (Exception ex)
            {
                dadosSensor.Enviado = false;

                return dadosSensor;              
            }
        }
    }
}
