using Common.Data;
using Common.Endpoints.Extensions;

using Links;

using Microsoft.Extensions.Options;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

using Submissions;

var builder = WebApplication.CreateBuilder(args);

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddCors();

builder.Services.AddOpenApi();

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
builder.AddRedisClient(connectionName: "redis");
builder.AddRedisOutputCache(connectionName: "redis-output");

builder.Services.AddLinksModule();
builder.Services.AddSubmissionsModule();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors(options =>
    {
        options.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });

    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();

public partial class Program;