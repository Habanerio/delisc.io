using System.Net;

using Common.Endpoints;
using Common.Endpoints.Extensions;

using Links.Models;
using Links.Queries;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Links.Endpoints;

internal sealed class GetLinkEndpoint : IEndpoint
{
    private const string ID_CANNOT_BE_NULL_OR_WHITESPACE = "Id cannot be null or whitespace";

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/links/{linkId}",
                async (
                    [FromRoute] string linkId,
                    [FromServices] ISender mediatr,
                    CancellationToken cancellationToken = default) =>
                {
                    var guidId = Guid.Parse(linkId);
                    if (guidId == Guid.Empty)
                        return Results.BadRequest(ID_CANNOT_BE_NULL_OR_WHITESPACE);

                    var rslt = await mediatr.Send(
                        new GetLinkByIdQuery(guidId), cancellationToken);

                    if (rslt.ValueOrDefault is null)
                        return Results.NotFound($"Link with id {linkId} not found");

                    return rslt.Match(Results.Ok, Results.BadRequest);
                })
            .CacheOutput()
            .Produces<Link?>()
            .ProducesProblem((int)HttpStatusCode.BadRequest)
            .ProducesProblem((int)HttpStatusCode.NotFound)
            .WithDisplayName("Get Link")
            .WithSummary("Gets a single link item")
            .WithTags("Links");
    }
}
