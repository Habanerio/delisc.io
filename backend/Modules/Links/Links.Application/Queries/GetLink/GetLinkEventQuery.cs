using Common.Application.Messaging;

using FluentResults;

namespace Links.Application.Queries.GetLink;

public sealed record GetLinkEventQuery(Guid LinkId) : ICommand;

internal sealed class GetLinkEventQueryHandler(ILinksRepository linkRepository) : ICommandHandler<GetLinkEventQuery, Result<Link>>
{
    public Task<Result<Link>> Handle(GetLink request, CancellationToken cancellationToken)
    {