using Microsoft.AspNetCore.Routing;

namespace Common.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
