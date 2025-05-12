using Common.Endpoints.Extensions;

using Links.Data;
using Links.Interfaces;

using Microsoft.Extensions.DependencyInjection;

namespace Links;

public static class LinksModule
{
    public static IServiceCollection AddLinksModule(this IServiceCollection services)
    {
        services.AddScoped<ILinksRepository, LinksRepository>();

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(AssemblyReference.Assembly);

            //TODO: Exception handling, logging, and validation pipeline behaviors are registered here.
        });


        services.AddEndpoints(AssemblyReference.Assembly);


        return services;
    }

}