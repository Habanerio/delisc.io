using System.Reflection;
using Deliscio.Apis.WebApi.Common.Clients;
using Deliscio.Core.Configuration;
using Deliscio.Core.Data.Mongo;
using Deliscio.Modules.Authentication;
using Deliscio.Modules.Authentication.Common;
using Deliscio.Modules.UserProfiles;
using Deliscio.Modules.UserProfiles.Common.Interfaces;
using Deliscio.Modules.UserProfiles.Common.Models;
using Deliscio.Modules.UserProfiles.Data;
using Deliscio.Modules.UserProfiles.MediatR.Queries;
//using MediatR;

var builder = WebApplication.CreateBuilder(args);

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

var config = ConfigSettingsManager.GetConfigs();
builder.Services.AddSingleton(config);

var mongoConfig = config.GetSection(MongoDbOptions.SectionName);
var mongoConfigConnectionString = config.GetSection($"{MongoDbOptions.SectionName}:ConnectionString").Value;
var mongoConfigDatabaseName = config.GetSection($"{MongoDbOptions.SectionName}:DatabaseName").Value;

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add services to the container.
// Note: Need to add 'Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
//       and then add .AddRazorRuntimeCompilation();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

//builder.Services.Configure<WebApiSettings>(
//    builder.Configuration.GetSection(WebApiSettings.SectionName));

builder.Services.AddHttpClient<AdminApiClient>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddSingleton(config);

builder.Services.AddOptions<MongoDbOptions>()
    .BindConfiguration(MongoDbOptions.SectionName);

var mongoDbAuthOptions = new MongoDbAuthOptions();
builder.Configuration.Bind(MongoDbAuthOptions.SectionName, mongoDbAuthOptions);

builder.Services.RegisterAuthenticationServices(config);


// Cannot seem to extract this away into the extension
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireClaim("Admin"));
});

builder.Services.AddSingleton<IUserProfilesService, UserProfilesService>();
builder.Services.AddSingleton<IUserProfilesRepository, UserProfilesRepository>();
//builder.Services.AddSingleton<IRequestHandler<GetUserProfileQuery, FluentResults.Result<UserProfile?>>, GetUserProfileQueryHandler>();

//builder.Services.AddSingleton<ILinksManager, LinksManager>();
//builder.Services.AddSingleton<ILinksService, LinksService>();
//builder.Services.AddSingleton<ILinksAdminService, LinksAdminService>();

//builder.Services.AddSingleton<ILinksRepository, LinksRepository>();

//builder.Services.AddSingleton<IRequestHandler<FindLinksAdminQuery, PagedResults<LinkItem>>, FindLinksAdminQueryHandler>();

//builder.Services.AddSingleton<IRequestHandler<GetLinkByIdQuery, Link?>, GetLinkByIdQueryHandler>();
//builder.Services.AddSingleton<IRequestHandler<GetLinkByUrlQuery, Link?>, GetLinkByUrlQueryHandler>();

//builder.Services.AddSingleton<IRequestHandler<GetLinksByIdsQuery, IEnumerable<LinkItem>>, GetLinksByIdsQueryHandler>();

//builder.Services.AddSingleton<IRequestHandler<GetLinksByDomainQuery, PagedResults<LinkItem>>, GetLinksByDomainQueryHandler>();
//builder.Services.AddSingleton<IRequestHandler<GetLinksByTagsQuery, PagedResults<LinkItem>>, GetLinksByTagsQueryHandler>();
//builder.Services.AddSingleton<IRequestHandler<GetLinkRelatedLinksQuery, LinkItem[]>, GetLinkRelatedLinksQueryHandler>();
//builder.Services.AddSingleton<IRequestHandler<GetRelatedTagsByTagsQuery, LinkTag[]>, GetRelatedTagsByTagsQueryHandler>();

//builder.Services.AddSingleton<IRequestHandler<AddLinkCommand, Guid>, AddLinkCommandHandler>();
//builder.Services.AddSingleton<IRequestHandler<SubmitLinkByUserCommand, Guid>, SubmitLinkByUserCommandHandler>();
//builder.Services.AddSingleton<IRequestHandler<DeleteLinkCommand, bool>, DeleteLinkCommandHandler>();
//builder.Services.AddSingleton<IRequestHandler<EditLinkCommand, (bool IsSuccess, string Message)>, EditLinkCommandHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
