using Common.Endpoints.Extensions;

using Microsoft.Extensions.DependencyInjection;

using Submissions.Data;
using Submissions.Interfaces;

namespace Submissions;

public static class SubmissionsModule
{
    public static IServiceCollection AddSubmissionsModule(this IServiceCollection services)
    {
        services.AddScoped<ISubmissionsRepository, SubmissionsRepository>();

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(AssemblyReference.Assembly);

            //TODO: Exception handling, logging, and validation pipeline behaviors are registered here.
        });


        services.AddEndpoints(AssemblyReference.Assembly);


        return services;
    }

}