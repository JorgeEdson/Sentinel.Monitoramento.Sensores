using Sentinel.Monitoramento.Sensores.Modelos;
using Sentinel.Monitoramento.Sensores.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<RabbitMQService>();
builder.Services.AddSingleton<ElasticsearchService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/registrar-dadosSensor", async (DadosSensor dadosSensor, RabbitMQService rabbitMQService, ElasticsearchService elasticsearchService) =>
{
    var dados = await rabbitMQService.EnviarParaExchangeAsync(dadosSensor);
    
    await elasticsearchService.IndexarDadosAsync(dados);
    
    return Results.Ok(dados.Enviado
        ? "Dados enviados para o RabbitMQ e indexados no Elasticsearch."
        : "Falha ao enviar dados para o RabbitMQ, mas os dados foram indexados no Elasticsearch.");
});

app.Run();
