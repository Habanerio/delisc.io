using System.Net;

using Common.Endpoints;
using Common.Endpoints.Extensions;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

using Submissions.Commands;
using Submissions.Models.Requests;

namespace Submissions.Endpoints;

public class SubmitLinkEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/submit", async (
            [FromBody] SubmitLinkRequest request,
            [FromServices] ISender mediatr,
            CancellationToken cancellationToken = default) =>
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
                return Results.BadRequest("UserId cannot be null or whitespace");

            if (string.IsNullOrWhiteSpace(request.Url))
                return Results.BadRequest("Url cannot be null or whitespace");

            var cmd = new SubmitLinkCommand(request.UserId, request.Url, request.Tags ?? []);
            var rslt = await mediatr.Send(cmd, cancellationToken);

            return rslt.Match(Results.Ok, Results.BadRequest);
        })
        .Produces<SubmitLinkResponse>()
        .ProducesProblem((int)HttpStatusCode.BadRequest)
        .WithDescription("Submits a single url")
        .WithDisplayName("Submit")
        .WithTags("Submits");
    }
}