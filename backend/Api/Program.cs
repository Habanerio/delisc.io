using Common.Endpoints.Extensions;

using Defaults;

using Links;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

using Structurizr.Annotations;

using Submissions;

namespace Deliscio.Api;

[Component(Description = "The Deliscio website's API service", Technology = "C#")]
[UsedByPerson("End Users", Description = "Public APIs")]
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();

        // Only register BSON serializer if not already registered (to prevent test conflicts)
        try
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        }
        catch (BsonSerializationException)
        {
            // Serializer already registered, ignore
        }

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        builder.Services.AddCors();

        builder.Services.AddOpenApi();

        // Aspire Docker Services
        builder.AddMongoDBClient(connectionName: "delisciodb");
        builder.AddRedisClient(connectionName: "redis");
        builder.AddRedisOutputCache(connectionName: "redis-output");

        builder.Services.AddLinksModule();
        builder.Services.AddSubmissionsModule();


        var app = builder.Build();

        app.MapDefaultEndpoints();

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

        //app.UseHttpsRedirection();

        app.MapEndpoints();

        app.Run();
    }
}