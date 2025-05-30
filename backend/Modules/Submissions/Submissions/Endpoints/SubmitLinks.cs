﻿using System.Net;

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

/// <summary>
/// Responsible for handling multiple submissions.
/// </summary>
public class SubmitLinksEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/submits", async (
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

            return rslts.Match(Results.Ok, Results.BadRequest);
        })
        .Produces<SubmitLinkResponse>()
        .ProducesProblem((int)HttpStatusCode.BadRequest)
        .WithDescription("Submits one or more urls")
        .WithDisplayName("Submits")
        .WithTags("Submits");
    }
}