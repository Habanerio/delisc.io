using Deliscio.Api;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

using Testcontainers.MongoDb;


namespace Tests.Functional.Modules;

public class BaseFunctionalTests : IAsyncLifetime, IClassFixture<WebApplicationFactory<Program>>
{
    private const string API_VERSION = "v1";
    private const string BASE_URL = $"/api/{API_VERSION}";

    private HttpClient _httpClient;
    private IConfiguration _configuration;

    private WebApplicationFactory<Program> _factory;
    private readonly MongoDbContainer _mongoDbContainer = new MongoDbBuilder().Build();

    protected const int DEFAULT_PAGE_NO = 1;
    protected const int DEFAULT_PAGE_SIZE = 25;
    protected const int DEFAULT_TAG_COUNT = 50;

    protected HttpClient HttpClient => _httpClient;
    protected IConfiguration Config => _configuration;

    public async Task InitializeAsync()
    {
        await _mongoDbContainer.StartAsync(); _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureAppConfiguration((context, configBuilder) =>
                {
                    configBuilder.AddJsonFile(
                        "appsettings.Testing.json",
                        optional: true,
                        reloadOnChange: true);

                    var mongoConnectionString = _mongoDbContainer.GetConnectionString();

                    configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        { "ConnectionStrings:delisciodb", mongoConnectionString },
                        { "Aspire:MongoDB:Driver:ConnectionString", mongoConnectionString }
                    });
                });

                builder.ConfigureServices(services =>
                {
                    // Remove existing MongoDB registrations if any
                    var descriptors = services.Where(d =>
                        d.ServiceType == typeof(IMongoClient) ||
                        d.ServiceType == typeof(IMongoDatabase)).ToList();

                    foreach (var descriptor in descriptors)
                    {
                        services.Remove(descriptor);
                    }

                    // Register MongoDB services for testing
                    var mongoConnectionString = _mongoDbContainer.GetConnectionString();
                    services.AddSingleton<IMongoClient>(provider =>
                    {
                        return new MongoClient(mongoConnectionString);
                    });

                    services.AddSingleton(provider =>
                    {
                        var client = provider.GetRequiredService<IMongoClient>();
                        return client.GetDatabase("delisciodb"); // Use a test database name
                    });
                });
            });

        _configuration = _factory.Services.GetRequiredService<IConfiguration>();
        _httpClient = _factory.CreateClient();

        var apiKey = Config.GetSection("ApiKey").Value;
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            HttpClient.DefaultRequestHeaders.Add("deliscio-api-key", apiKey);
        }

        // Seed test data
        await SeedTestDataAsync();
    }

    public async Task DisposeAsync()
    {
        HttpClient?.Dispose();
        await _factory.DisposeAsync();
        await _mongoDbContainer.DisposeAsync();
    }

    /// <summary>
    /// Makes a Http GET request to the specified URL.
    /// </summary>
    /// <returns>HttpResponseMessage</returns>
    protected async Task<HttpResponseMessage> GetAsync(string requestUrl)
    {
        var response = await HttpClient.GetAsync($"{BASE_URL}{requestUrl}");
        return response;
    }

    /// <summary>
    /// Seeds test data into the MongoDB container for testing.
    /// </summary>
    private async Task SeedTestDataAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var mongoDatabase = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();

        await SeedTestLinksDataAsync(mongoDatabase);
    }

    /// <summary>
    /// Seeds test links data into the MongoDB container.
    /// </summary>
    /// <param name="mongoDatabase"></param>
    /// <returns></returns>
    static async Task SeedTestLinksDataAsync(IMongoDatabase mongoDatabase)
    {
        var linksCollection = mongoDatabase.GetCollection<BsonDocument>("links");

        var existingCount = await linksCollection.CountDocumentsAsync(new BsonDocument());
        if (existingCount > 0)
               return;

        var json = await File.ReadAllTextAsync("./data/links.json");
        var testLinks = BsonSerializer.Deserialize<List<BsonDocument>>(json);

        await linksCollection.InsertManyAsync(testLinks);
    }
}