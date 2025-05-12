using Common.Application.Messaging;

using FluentResults;

using Submissions.Data;
using Submissions.Interfaces;

namespace Submissions.Commands;

public sealed record SubmitLinkCommand : ICommand<SubmitLinkResponse>
{
    public string UserId { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public string[] UserTags { get; set; } = [];

    public SubmitLinkCommand(string userId, string url, string[]? userTags = default)
    {
        Url = url;
        UserId = userId;
        UserTags = userTags ?? [];
    }
}

public class SubmitLinkCommandHandler(
    ISubmissionsRepository submissionsRepository) :
    ICommandHandler<SubmitLinkCommand, SubmitLinkResponse>
{
    private readonly ISubmissionsRepository _submissionsRepository = submissionsRepository ??
        throw new ArgumentNullException(nameof(submissionsRepository));

    public async Task<Result<SubmitLinkResponse>> Handle(
        SubmitLinkCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Url))
            return Result.Fail<SubmitLinkResponse>(
                "Url cannot be null or whitespace");

        if (string.IsNullOrWhiteSpace(command.UserId))
            return Result.Fail<SubmitLinkResponse>(
                "UserId cannot be null or whitespace");

        if (!Guid.TryParse(command.UserId, out var userGuid) || userGuid.Equals(Guid.Empty))
            return Result.Fail<SubmitLinkResponse>("UserId is not a valid Id");

        var entity = SubmissionEntity.Create(
            userGuid,
            command.Url,
            new UsersData()
            {
                Tags = command.UserTags
            });

        var rslt = await _submissionsRepository.SaveAsync(
            entity, cancellationToken: cancellationToken);

        if (!rslt.IsSuccess)
            return Result.Fail<SubmitLinkResponse>(
                rslt.Errors.FirstOrDefault()?.Message ??
                "Unknown error");

        return Result.Ok(new SubmitLinkResponse(entity.Id.ToString(), command.Url));
    }
}