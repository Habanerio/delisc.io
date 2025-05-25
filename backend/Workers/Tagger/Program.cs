using Defaults;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

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

// Aspire Docker Services

builder.AddMongoDBClient(connectionName: "delisciodb");

// The following is obsolete with the `OllamaSharp 9.4.1-beta.282`. It does work with previous versions.
builder.AddOllamaSharpChatClient("llm-model");


builder.Services.AddSingleton<ISubmissionsRepository, SubmissionsRepository>();

var host = builder.Build();
host.Run();
