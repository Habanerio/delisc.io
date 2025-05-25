using Defaults;

using FinalizeService;

using Links.Data;
using Links.Interfaces;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

using Submissions.Data;
using Submissions.Interfaces;

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

builder.Services.AddSingleton<ILinksRepository, LinksRepository>();
builder.Services.AddSingleton<ISubmissionsRepository, SubmissionsRepository>();

var host = builder.Build();
host.Run();
