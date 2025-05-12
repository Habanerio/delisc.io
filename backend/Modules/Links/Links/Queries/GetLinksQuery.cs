using FluentResults;

using Links.Interfaces;
using Links.Models;

using MediatR;


namespace Links.Queries;

/// <summary>
/// Responsible for attempting to retrieve a collection of links that match the search term, domain, and/or tags. 
/// </summary>
public sealed record GetLinksQuery : IRequest<Result<IEnumerable<LinkItem>>>
{
    public int PageNo { get; init; }

    public int PageSize { get; init; }

    public string Domain { get; init; } = string.Empty;

    public string SearchTerm { get; init; } = string.Empty;

    public string[] Tags { get; init; } = [];

    public GetLinksQuery(int pageNo, int pageSize, string? searchTerm = "", string? domain = "", string[]? tags = null)
    {
        Domain = domain ?? string.Empty;
        SearchTerm = searchTerm ?? string.Empty;
        Tags = tags ?? [];

        PageNo = pageNo;
        PageSize = pageSize;
    }
}

public class GetLinksQueryHandler(ILinksRepository linksRepository) :
    IRequestHandler<GetLinksQuery, Result<IEnumerable<LinkItem>>>
{
    private readonly ILinksRepository _linksRepository = linksRepository ??
        throw new ArgumentNullException(nameof(linksRepository));

    public async Task<Result<IEnumerable<LinkItem>>> Handle(
        GetLinksQuery query,
        CancellationToken cancellationToken)
    {
        var (results, totalPages, totalCount) = await _linksRepository.FindAsync(
            term: query.SearchTerm,
            tags: query.Tags,
            domain: string.Empty,
            pageNo: query.PageNo,
            pageSize: query.PageSize,
            cancellationToken: cancellationToken);

        var linkItems = Mapper.Map<LinkItem>(results);

        return Result.Ok(linkItems);
    }
}

