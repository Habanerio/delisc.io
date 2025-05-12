using Common.Endpoints;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

using Submissions.Commands;
using Submissions.Models.Requests;

namespace Submissions.Endpoints;

public class SubmitLinksEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("submits", async (
            [FromBody] SubmitLinksRequest request,
            [FromServices] ISender mediatr,
            CancellationToken cancellationToken = default) =>
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
                return Results.BadRequest("UserId cannot be null or whitespace");

            if (request.Urls.Length == 0)
                return Results.BadRequest("Urls cannot be null or empty");

            var cmd = new SubmitLinksCommand(request.UserId, request.Urls);
            var rslts = await mediatr.Send(cmd, cancellationToken);

            if (!rslts.IsSuccess)
                return Results.BadRequest(rslts.Errors.First().Message);

            return Results.Ok(rslts.Value);
        });
    }
}