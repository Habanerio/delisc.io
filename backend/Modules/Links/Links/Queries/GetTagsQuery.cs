using FluentResults;

using Links.Interfaces;
using Links.Models;

using MediatR;

namespace Links.Queries;

public record GetTagsQuery(string Tags, int Count) : IRequest<Result<LinkTag[]>>;

public class GetTagsQueryHandler(ILinksRepository linksRepository) : IRequestHandler<GetTagsQuery, Result<LinkTag[]>>
{
    private readonly ILinksRepository _linksRepository = linksRepository ??
        throw new ArgumentNullException(nameof(linksRepository));

    public async Task<Result<LinkTag[]>> Handle(GetTagsQuery query, CancellationToken cancellationToken)
    {
        var tagsArr = query.Tags.Split(',') ?? [];

        var tags = (await _linksRepository.GetTagsAsync(tagsArr, query.Count, cancellationToken));

        var rslts = Mapper.Map(tags)?.ToArray() ?? [];

        return Result.Ok(rslts);
    }
}

