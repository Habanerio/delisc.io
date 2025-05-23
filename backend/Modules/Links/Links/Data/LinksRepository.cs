using System.Linq.Expressions;

using Ardalis.GuardClauses;

using Common.Data;

using FluentResults;

using Links.Interfaces;

using MongoDB.Bson;
using MongoDB.Driver;

namespace Links.Data;

/// <summary>
/// Responsible for interacting with the MongoDb database for the Links module
/// </summary>
public sealed class LinksRepository(IMongoDatabase mongoDb) :
    MongoDbRepository<LinkEntity>(new LinksDbContext<LinkEntity>(mongoDb)),
    ILinksRepository
{
    #region - Links

    public async Task<(IEnumerable<LinkEntity> Results, int TotalPages, int TotalCount)>
        FindAsync(
        string term, string tags, string domain,
        int pageNo, int pageSize, int skip = 0,
        bool? isActive = default, bool? isFlagged = default, bool? isDeleted = false,
        CancellationToken cancellationToken = default)
    {
        if (pageNo < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNo));

        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        var tagsArr = tags.Trim().Split(',')?.Where(t => !string.IsNullOrWhiteSpace(t)).ToArray() ?? [];

        var rslts = await FindAsync(term, tagsArr, domain, pageNo, pageSize, skip, isActive, isFlagged, isDeleted, cancellationToken);

        return rslts;
    }

    public async Task<(IEnumerable<LinkEntity> Results, int TotalPages, int TotalCount)>
        FindAsync(
        string term, string[] tags, string domain,
        int pageNo, int pageSize, int skip = 0,
        bool? isActive = default, bool? isFlagged = default, bool? isDeleted = false,
        CancellationToken cancellationToken = default)
    {
        if (pageNo < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNo));

        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        var hasSearchTerm = !string.IsNullOrWhiteSpace(term);
        var hasDomain = !string.IsNullOrWhiteSpace(domain);
        var hasTags = tags?.Where(t => t.Trim() != "").ToArray().Length > 0;


        Expression<Func<LinkEntity, bool>> predicate = (l => (
              !hasSearchTerm ||
                (
                    l.Title.Contains(term, StringComparison.InvariantCultureIgnoreCase) ||
                    l.Description.Contains(term, StringComparison.InvariantCultureIgnoreCase)
                ))
                &&
                (!hasDomain || l.Domain.Equals(domain, StringComparison.InvariantCultureIgnoreCase))
                &&
                (!hasTags ||
                  tags!.All(tag => l.Tags.Any(linkTag => linkTag.Name == tag))
                )
                && (isActive == null || l.IsActive == isActive.Value)
                && (isFlagged == null || l.IsFlagged == isFlagged.Value)
                && (isDeleted == null || l.IsDeleted == isDeleted.Value));

        var rslts = await FindDocumentsAsync(
            predicate,
            pageNo,
            pageSize,
            descending: true,
            orderBy: x => x.DateCreated,
            cancellationToken);

        return rslts;
    }

    public async Task<LinkEntity?> GetAsync(Guid linkId, CancellationToken cancellationToken = default)
    {
        Guard.Against.NullOrEmpty(linkId);

        return await FirstOrDefaultDocumentAsync(x => x.Id == linkId, cancellationToken);
    }

    public async Task<LinkEntity?> GetLinkByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        Guard.Against.NullOrEmpty(url);

        return await FirstOrDefaultDocumentAsync(x => x.Url == url, cancellationToken);
    }

    public async Task<(IEnumerable<LinkEntity> Results, int TotalPages, int TotalCount)> GetLinksByTagsAsync(IEnumerable<string> tags, int pageNo = 1, int pageSize = 25, CancellationToken cancellationToken = default)
    {
        var arrTags = tags as string[] ?? Array.Empty<string>();

        if (!arrTags.Any())
            return (Enumerable.Empty<LinkEntity>(), 0, 0);

        var filter =
            Builders<LinkEntity>.Filter.All(link => link.Tags.Select(tag => tag.Name), arrTags); // & Builders<LinkEntity>.Filter.Eq(bookmark => bookmark.IsActive, true);

        Expression<Func<LinkEntity, bool>> predicate =
            (l => l.Tags.All(t => arrTags.Contains(t.Name)));

        var links = await FindDocumentsAsync(
            predicate,
            pageNo,
            pageSize,
            descending: true,
    orderBy: x => x.DateCreated,
            cancellationToken);

        return links;
    }

    /// <summary>
    /// Gets a collection of tags that are related to the tags that were specified.
    /// The tags that are returned are the same tags that are in all the Links that would be found with the GetLinksByTagsAsync(), but without including the paging.
    /// </summary>
    /// <param name="tags">The tags to use to get all other related tags</param>
    /// <param name="count">The max number of tags to return</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<LinkTagEntity>> GetTagsAsync(string[] tags, int count, CancellationToken cancellationToken = default)
    {
        if (count < 1)
            return Enumerable.Empty<LinkTagEntity>();

        var newTags = tags
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.ToLowerInvariant())
            .ToArray();

        // If tags.Length is 0, get all tags. Else, get only those that are related to the specified tags
        BsonDocument match = newTags.Length == 0
            ? new BsonDocument("$match", new BsonDocument())
            : new BsonDocument("$match", new BsonDocument("tags.name", new BsonDocument("$all", new BsonArray(newTags))));

        var pipeline = new BsonDocument[]
        {
            match,
            // Unwind the tags array to create a separate document for each tag
            new BsonDocument("$unwind", "$tags"),
            // Group the tags and count the occurrences of each tag
            new BsonDocument("$group",
                new BsonDocument
                {
                    { "_id", "$tags.name" },
                    { "count", new BsonDocument("$sum", 1) }
                }
            ),
            // Project the result to include only the tag name and count
            new BsonDocument("$project",
                new BsonDocument
                {
                    { "_id", 0 },
                    { "TagName", "$_id" },
                    { "Count", "$count" }
                }
            )
        };

        // Execute the aggregation pipeline
        var cursor = await Collection.AggregateAsync<BsonDocument>(pipeline, cancellationToken: cancellationToken);

        var relatedTags = (await cursor!
            .ToListAsync(cancellationToken))
            .OrderByDescending(t => t["Count"].AsInt32)
            .Select(x => new LinkTagEntity(x["TagName"].AsString, x["Count"].AsInt32))
            .ToArray();

        if (!relatedTags.Any())
            return [];

        // Strip out the tags we used to get the related tags (we only want related)
        relatedTags = relatedTags
            .Where(x => !tags.Contains(x.Name))
            .Take(count)
            .ToArray();

        var totalCounts = relatedTags.Sum(x => x.Count);

        foreach (var relatedTag in relatedTags)
        {
            relatedTag.Weight = totalCounts > 0f ? (relatedTag.Count / (float)totalCounts) : 0f;
        }

        return relatedTags.OrderByDescending(t => t.Count).ThenBy(t => t.Name);
    }

    public Task<IEnumerable<LinkTagEntity>> GetRelatedTagsByDomainAsync(string domain, int count, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> SaveAsync(LinkEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity.CreatedById.Equals(Guid.Empty))
            return Result.Fail("UserId cannot be empty");

        if (string.IsNullOrWhiteSpace(entity.Url))
            return Result.Fail("Url cannot be null or whitespace");

        try
        {
            entity.DateCreated = DateTime.UtcNow;
            await AddDocumentAsync(entity, cancellationToken);
        }
        catch (Exception e)
        {
            // Log

            return Result.Fail(e.Message);
        }

        return Result.Ok();
    }

    #endregion

    #region - Tags -

    public async Task AddTagAsync(Guid linkId, string tag, CancellationToken cancellationToken)
    {
        var filter = Builders<LinkEntity>.Filter.Eq("_id", linkId);
        var update = Builders<LinkEntity>.Update.Inc($"Tags.{tag}", 1);

        var updateResult = await Collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true }, cancellationToken: cancellationToken);

        //!(updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)

    }

    /// <summary>
    /// Removes an occurrence of the tag from the Tag collection that belongs to Link with the id.<br />
    /// If the tag's count becomes zero, then it will be removed from the collection.
    /// </summary>
    /// <param name="linkId">The id of the Link to update.</param>
    /// <param name="tag">The tag in which its count will be incremented.</param>
    /// <param name="cancellationToken">The token.</param>
    public async Task RemoveTagAsync(Guid linkId, string tag, CancellationToken cancellationToken)
    {
        var filter = Builders<LinkEntity>.Filter.Eq("_id", linkId);
        var update = Builders<LinkEntity>.Update.Combine(
            Builders<LinkEntity>.Update.Inc($"Tags.{tag}", -1), // Decrease the count of the specified tag
            Builders<LinkEntity>.Update.PullFilter("Tags",
                Builders<BsonDocument>.Filter.Eq(tag, 0)) // Remove the tag if its count becomes zero
        );

        await Collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
    }

    #endregion
}