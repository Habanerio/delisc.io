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

namespace Links.Endpoints;

internal sealed class GetTagsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/links/tags",
            async (
                [FromServices] ISender mediatr,
                [FromQuery] string tags = "",
                [FromQuery] int count = 50,
                CancellationToken cancellationToken = default) =>
            {
                if (count < 1)
                    return Results.BadRequest("Count cannot be less than 1");

                var query = new GetTagsQuery(tags, count);

                var rslts = await mediatr.Send(query, cancellationToken);

                return rslts.Match(Results.Ok, Results.BadRequest);
            })
            .Produces<LinkTag[]>()
            .ProducesProblem((int)HttpStatusCode.BadRequest)
            .WithDisplayName("Get Tags")
            .WithTags("Links");
    }
}