using Ardalis.GuardClauses;

using FluentResults;

using Links.Interfaces;
using Links.Models;

using MediatR;

namespace Links.Queries;

public sealed record GetLinkByIdQuery : IRequest<Result<Link?>>
{
    public Guid Id { get; init; }

    public GetLinkByIdQuery(Guid id)
    {
        Guard.Against.NullOrEmpty(id);

        Id = id;
    }
}

/// <summary>
/// Handles getting a single link from the central repository, by the link's id
/// </summary>
public class GetLinkByIdQueryHandler(ILinksRepository linksRepository) :
    IRequestHandler<GetLinkByIdQuery, Result<Link?>>
{
    private readonly ILinksRepository _linksRepository = linksRepository ??
        throw new ArgumentNullException(nameof(linksRepository));

    public async Task<Result<Link?>> Handle(
        GetLinkByIdQuery query,
        CancellationToken cancellationToken)
    {
        var link = await _linksRepository.GetAsync(query.Id, cancellationToken);

        var model = Mapper.Map<Link>(link);

        return model;
    }
}