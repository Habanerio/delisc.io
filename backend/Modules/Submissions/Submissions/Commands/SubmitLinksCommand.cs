using Common.Application.Messaging;

using FluentResults;

using Submissions.Data;
using Submissions.Interfaces;

namespace Submissions.Commands;

public sealed record SubmitLinksCommand : ICommand<List<SubmitLinkResponse>>
{
    public string UserId { get; set; } = string.Empty;

    public string[] Urls { get; set; } = [];

    public SubmitLinksCommand(string userId, string[] urls, string[]? userTags = default)
    {
        Urls = urls;
        UserId = userId;
    }
}

public class SubmitLinksCommandHandler(
    ISubmissionsRepository submissionsRepository) :
    ICommandHandler<SubmitLinksCommand, List<SubmitLinkResponse>>
{
    private readonly ISubmissionsRepository _submissionsRepository = submissionsRepository ??
        throw new ArgumentNullException(nameof(submissionsRepository));

    public async Task<Result<List<SubmitLinkResponse>>> Handle(
        SubmitLinksCommand command,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(command.UserId, out var userGuid) || userGuid.Equals(Guid.Empty))
            return Result.Fail("UserId is not a valid Id");

        var entities = command.Urls.Select(url =>
            SubmissionEntity.Create(userGuid, url)).ToList();

        var rslt = await _submissionsRepository.SaveAsync(
            entities, cancellationToken: cancellationToken);

        if (!rslt.IsSuccess)
            return Result.Fail(
                rslt.Errors.FirstOrDefault()?.Message ??
                "Unknown error");

        var response = entities.Select(e => new SubmitLinkResponse(e.Id.ToString(), e.Url)).ToList();

        return Result.Ok(response);
    }
}