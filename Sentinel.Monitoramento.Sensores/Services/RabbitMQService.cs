using RabbitMQ.Client;
using Sentinel.Monitoramento.Sensores.Modelos;
using System.Text.Json;
using System.Text;

namespace Sentinel.Monitoramento.Sensores.Services
{
    public class RabbitMQService(IConnection connection,IConfiguration configuration)
    {
        private readonly IModel _channel = connection.CreateModel();
        private readonly string _exchangeName = configuration["RabbitMQ:Exchange"] ?? "sensor-data-exchange";
        private readonly string _routingKey = configuration["RabbitMQ:RoutingKey"] ?? "sensor-data-routing-key";

        public async Task<DadosSensor> EnviarParaExchangeAsync(DadosSensor dadosSensor)
        {
            try 
            {
                var message = JsonSerializer.Serialize(dadosSensor);
                var body = Encoding.UTF8.GetBytes(message);

                _channel.BasicPublish(
                    exchange: _exchangeName,
                    routingKey: _routingKey,
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
