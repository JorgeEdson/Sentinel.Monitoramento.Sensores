using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Graph;
using RabbitMQ.Client;
using Sentinel.Monitoramento.Sensores.Modelos;
using Sentinel.Monitoramento.Sensores.Services;
using System;

var builder = WebApplication.CreateBuilder(args);

//rabbitMQ
var rabbitMQHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
var factory = new ConnectionFactory() { HostName = rabbitMQHost };
var connection = factory.CreateConnection();
builder.Services.AddSingleton<IConnection>(connection);
builder.Services.AddSingleton<RabbitMQService>();

//elasticsearch
var elasticsearchUri = builder.Configuration["Elasticsearch:Uri"]
    ?? throw new ArgumentNullException(nameof(builder.Configuration), "A URL do Elasticsearch não foi configurada corretamente.");
var elasticsearchClientSettings = new ElasticsearchClientSettings(new Uri(elasticsearchUri));
var elasticsearchClient = new ElasticsearchClient(elasticsearchClientSettings);
builder.Services.AddSingleton(elasticsearchClient);
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
