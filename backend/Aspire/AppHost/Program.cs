var builder = DistributedApplication.CreateBuilder(args);

// https://learn.microsoft.com/en-us/dotnet/aspire/database/mongodb-integration?tabs=package-reference
var mongodb = builder
    .AddMongoDB("mongodb")
    .WithDataVolume()
    .WithMongoExpress()
    .WithLifetime(ContainerLifetime.Session)
    .AddDatabase("delisciodb");

//var rabbitMq = builder.AddRabbitMQ("rabbitmq")
//    .WithDataVolume(isReadOnly: false)
//    .WithManagementPlugin();

var ollama = builder.AddOllama("ollama", 11434)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Session)
    .WithOpenWebUI()

// Uncomment if you have an Nvidia GPU and want to use it
//.WithGPUSupport()
;

// Small models that should work on CPU (???)
// If you have a GPU, visit `https://ollama.com` to find other bigger models
//.AddModel("gemma3:1b");
//.AddModel("gemma3:4b");

var llm = ollama.AddModel("llm-model", "gemma3:4b");
//var llm = ollama.AddModel("llm-model", "gemma3:27b");

//.AddModel("qwen3:0.6b");
//.AddModel("qwen3:1.7b");
//.AddModel("qwen3:4b");

//.AddModel("qwen2.5:0.5b");
//.AddModel("qwen2.5:1.5b");
//.AddModel("qwen2.5:3b");

//.AddModel("llama3.2:1b");
//.AddModel("llama3.2:3b");

//.AddModel("deepseek-r1:1.5b");
//var llm = ollama.AddModel("llm-model", "llama3.2:1b");


// Or visit `https://huggingface.co/` to find other models
//var llm = ollama.AddHuggingFaceModel("llm-model", "microsoft/bitnet-b1.58-2B-4T-gguf");



var redis = builder
    .AddRedis("redis")
    .WithRedisCommander()
    .WithRedisInsight();

var redisOutput = builder
    .AddRedis("redis-output")
    .WithRedisCommander()
    .WithRedisInsight();

var api = builder.AddProject<Projects.Api>("api")
    .WithReference(mongodb)
    .WithReference(redis)
    .WithReference(redisOutput)
    .WaitFor(mongodb);

builder.AddProject<Projects.ValidateService>("validate-service")
    .WithReference(mongodb)
    .WithReference(redis)
    .WaitFor(mongodb);

builder.AddProject<Projects.CrawlServices>("crawl-service")
    .WithReference(mongodb)
    .WithReference(redis)
    .WaitFor(mongodb);

builder.AddProject<Projects.TagService>("tag-service")
    .WithReference(mongodb)
    .WithReference(redis)
    .WithReference(llm)
    .WaitFor(mongodb)
    .WaitFor(llm);

builder.AddProject<Projects.FinalizeService>("final-service")
    .WithReference(mongodb)
    .WithReference(redis)
    .WithReference(redisOutput)
    .WaitFor(mongodb);

/*
builder.AddNpmApp("react", "../../../frontend/")
    .WithReference(api)
    .WaitFor(api)
    .WithEnvironment("BROWSER", "none") // Disable opening browser on npm start
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();
*/

await builder.Build().RunAsync();
