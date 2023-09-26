using Ardalis.GuardClauses;
using Deliscio.Core.Data.Mongo;
using Deliscio.Modules.Links.Data.Entities;
using Deliscio.Modules.Links.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Deliscio.Modules.Links.Data.Mongo;

/// <summary>
/// Responsible for interacting with the MongoDb database for the Links module
/// </summary>
public sealed class LinksRepository : MongoRepository<LinkEntity>, ILinksRepository
{
    #region - Constructors -
    public LinksRepository(IOptions<MongoDbOptions> options) : base(options) { }

    public LinksRepository(string connectionString, string databaseName) : base(connectionString, databaseName) { }

    public LinksRepository(MongoDbContext<LinkEntity> context) : base(context) { }
    #endregion

    #region - Links

    public async Task<LinkEntity?> GetByUrlAsync(string url, CancellationToken token = default)
    {
        Guard.Against.NullOrEmpty(url);

        return await FirstOrDefault(x => x.Url == url, token);
    }

    public async Task<(IEnumerable<LinkEntity> Results, int TotalPages, int TotalCount)> GetByDomainAsync(string domain, int pageNo = 1, int pageSize = 25, CancellationToken token = default)
    {
        Guard.Against.NullOrEmpty(domain);

        return await FindAsync(x => x.Domain == domain, pageNo, pageSize, token);
    }

    public async Task<(IEnumerable<LinkEntity> Results, int TotalPages, int TotalCount)> GetByTagsAsync(IEnumerable<string> tags, int pageNo = 1, int pageSize = 25, CancellationToken token = default)
    {
        var arrTags = tags as string[] ?? Array.Empty<string>();

        if (!arrTags.Any())
            return (Enumerable.Empty<LinkEntity>(), 0, 0);

        var filter =
            Builders<LinkEntity>.Filter.All(link => link.Tags.Select(tag => tag.Name), arrTags); // & Builders<LinkEntity>.Filter.Eq(bookmark => bookmark.IsActive, true);

        var links = await FindAsync(filter, pageNo, pageSize, token);

        return links;
    }

    /// <summary>
    /// Gets a collection of tags that are related to the tags that were specified.
    /// The tags that are returned are the same tags that are in all of the Links that would be found with the GetByTagsAsync(), but without including the paging.
    /// </summary>
    /// <param name="tags">The tags to use to get all other related tags</param>
    /// <param name="count">The max number of tags to return</param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<IEnumerable<LinkTagEntity>> GetRelatedTagsAsync(string[] tags, int? count = default, CancellationToken token = default)
    {
        if (!tags.Any())
            return Enumerable.Empty<LinkTagEntity>();

        var newCount = count ?? 50;

        if (newCount < 1)
            return Enumerable.Empty<LinkTagEntity>();

        // Create aggregation pipeline to filter bookmarks and extract distinct tags
        var pipeline = new BsonDocument[]
        {
            // Match bookmarks that have at least one of the specified tags
            new BsonDocument("$match",
                new BsonDocument("Tags.Name", new BsonDocument("$in", new BsonArray(tags)))
            ),
            // Unwind the tags array to create a separate document for each tag
            new BsonDocument("$unwind", "$Tags"),
            // Group the tags and count the occurrences of each tag
            new BsonDocument("$group",
                new BsonDocument
                {
                    { "_id", "$Tags.Name" },
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
        var cursor = await Collection.AggregateAsync<BsonDocument>(pipeline, cancellationToken: token);

        var relatedTags = cursor?.ToList(token).OrderByDescending(t => t.Count()).Take(newCount).Select(x => new LinkTagEntity(x["TagName"].AsString, x["Count"].AsInt32)).ToArray() ?? Array.Empty<LinkTagEntity>();

        if (!relatedTags.Any())
            return Enumerable.Empty<LinkTagEntity>();

        var totalCounts = relatedTags.Sum(x => x.Count);

        foreach (var relatedTag in relatedTags)
        {
            relatedTag.Weight = totalCounts > 0m ? (relatedTag.Count / (decimal)totalCounts) : 0m;

            if (relatedTag.Weight == decimal.Zero)
                continue;
        }

        return relatedTags;
    }

    #endregion

    #region - Tags -

    public async Task AddTagAsync(Guid linkId, string tag, CancellationToken token)
    {
        var filter = Builders<LinkEntity>.Filter.Eq("_id", linkId);
        var update = Builders<LinkEntity>.Update.Inc($"Tags.{tag}", 1);

        var updateResult = await Collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true }, cancellationToken: token);

        //!(updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)

    }

    public async Task<IEnumerable<LinkTagEntity>> GetTagsAsync(Guid linkId, CancellationToken token)
    {
        //var filter = Builders<LinkEntity>.Filter.Eq("_id", id);

        //var rslt = await Collection.FindSync(filter, cancellationToken: token).SingleOrDefaultAsync(cancellationToken: token);

        //if (rslt == null)
        //{
        //    return Enumerable.Empty<TagEntity>();
        //}

        //return new rslt.Ta.;

        return Enumerable.Empty<LinkTagEntity>();
    }

    /// <summary>
    /// Removes an occurrence of the tag from the Tag collection that belongs to Link with the id.<br />
    /// If the tag's count becomes zero, then it will be removed from the collection.
    /// </summary>
    /// <param name="linkId">The id of the Link to update.</param>
    /// <param name="tag">The tag in which its count will be incremented.</param>
    /// <param name="token">The token.</param>
    public async Task RemoveTagAsync(Guid linkId, string tag, CancellationToken token)
    {
        var filter = Builders<LinkEntity>.Filter.Eq("_id", linkId);
        var update = Builders<LinkEntity>.Update.Combine(
            Builders<LinkEntity>.Update.Inc($"Tags.{tag}", -1), // Decrease the count of the specified tag
            Builders<LinkEntity>.Update.PullFilter("Tags",
                Builders<BsonDocument>.Filter.Eq(tag, 0)) // Remove the tag if its count becomes zero
        );

        await Collection.UpdateOneAsync(filter, update, cancellationToken: token);
    }

    #endregion
}