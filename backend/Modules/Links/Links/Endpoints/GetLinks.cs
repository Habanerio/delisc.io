using System.Net;

using Common.Endpoints;
using Common.Endpoints.Extensions;
using Common.Models;

using Links.Models;
using Links.Queries;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Links.Endpoints;

internal sealed class GetLinksEndpoint : IEndpoint
{
    private const string PAGE_NO_CANNOT_BE_LESS_THAN_ONE = "PageNo cannot be less than 1";
    private const string PAGE_SIZE_CANNOT_BE_LESS_THAN_ONE = "PageSize cannot be less than 1";

    private const int DEFAULT_PAGE_NO = 1;
    private const int DEFAULT_PAGE_SIZE = 25;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/links",
                async (
                    [FromServices] ISender mediatr,
                    [FromQuery] string term = "",
                    [FromQuery] string domain = "",
                    [FromQuery] string tags = "",
                    [FromQuery] int pageNo = 1,
                    [FromQuery] int pageSize = 25,
                    CancellationToken cancellationToken = default) =>
                {
                    if (pageNo < 1)
                        return Results.BadRequest(PAGE_NO_CANNOT_BE_LESS_THAN_ONE);

                    if (pageSize < 1)
                        return Results.BadRequest(PAGE_SIZE_CANNOT_BE_LESS_THAN_ONE);

                    var newTags = tags?.Split(',') ?? [];

                    var query = new GetLinksQuery(pageNo, pageSize, term, domain, newTags);
                    var rslts = await mediatr.Send(query, cancellationToken);

                    // For a `search`, should an empty result set be returned or a 404?

                    return rslts.Match(Results.Ok, Results.BadRequest);
                })
            .Produces<PagedResults<LinkItem>>()
            .ProducesProblem((int)HttpStatusCode.BadRequest)
            .WithDisplayName("Get Links")
            .WithTags("Links");
    }
}