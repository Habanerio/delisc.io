using Common.Data;

using Defaults;

using Microsoft.Extensions.Options;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

using Submissions.Data;
using Submissions.Interfaces;

using TagService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.AddServiceDefaults();

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddSingleton<HttpClient>();

//builder.AddRabbitMQClient(connectionName: "rabbitmq");
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDBSettings"));

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();

    return client.GetDatabase(sp.GetRequiredService<IOptions<MongoDbSettings>>()
        .Value
        .DatabaseName);
});

// Aspire Docker Services

builder.AddMongoDBClient(connectionName: "delisciodb");

// The following is obsolete with the `OllamaSharp 9.4.1-beta.282`. It does work with previous versions.
builder.AddOllamaSharpChatClient("llm-model");


builder.Services.AddSingleton<ISubmissionsRepository, SubmissionsRepository>();

var host = builder.Build();
host.Run();
