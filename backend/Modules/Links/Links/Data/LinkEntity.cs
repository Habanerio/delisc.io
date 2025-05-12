using Common.Data;
using Common.Data.Attributes;

using MongoDB.Bson.Serialization.Attributes;

namespace Links.Data;

/// <summary>
/// Represents a link that has been submitted to the central repository after approval.
/// </summary>
[BsonCollection("links")]
public sealed class LinkEntity : MongoDbDocument
{
    /// <summary>
    /// The title of the page, from the page itself, that the link points to.
    /// </summary>
    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The description/summary of the page that the link points to.
    /// This is retrieved/built from the page itself and not user submitted*.
    /// </summary>
    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The domain of the link. If the link is a sub-domain (excluding www or similar), then the sub-domain will be returned.
    /// </summary>
    [BsonElement("domain")]
    public string Domain { get; set; } = string.Empty;

    /// <summary>
    /// The full URL of the image that is associated with this link.
    /// This is retrieved from the page itself, if it exists.
    /// </summary>
    [BsonElement("image_url")]
    public string ImageUrl { get; set; } = string.Empty;


    [BsonElement("keywords")]
    public List<string> Keywords { get; set; } = [];


    [BsonElement("is_active")]
    public bool IsActive { get; set; } = false;

    [BsonElement("is_flagged")]
    public bool IsFlagged { get; set; } = false;

    /// <summary>
    /// The number of times this link has been saved.
    /// </summary>
    [BsonElement("saves_count")]
    public int SavesCount { get; set; }

    /// <summary>
    /// The number of times this link has been liked.
    /// </summary>
    [BsonElement("likes_count")]
    public int LikesCount { get; set; }

    /// <summary>
    /// Represents all the tags that are associated with this link, as well as the number of times they've been used.
    /// </summary>
    /// <value>
    /// The tags.
    /// </value>
    [BsonElement("tags")]
    public List<LinkTagEntity> Tags { get; set; } = [];

    /// <summary>
    /// The URL of the link to go to the page
    /// </summary>
    [BsonElement("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// The id of the user who originally submitted this link.
    /// </summary>
    [BsonElement("created_by")]
    public Guid CreatedById { get; set; }


    [BsonElement("date_Created")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime DateCreated { get; set; }


    [BsonElement("date_updated")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime DateUpdated { get; set; }

    [BsonElement("is_deleted")]
    public bool IsDeleted => DateDeleted != null && DateDeleted != default(DateTime);


    [BsonElement("date_deleted")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? DateDeleted { get; set; }

    [BsonElement("deleted_by")]
    public Guid DeletedById { get; set; }

    private LinkEntity()
    {

    }


    public LinkEntity(Guid id, string url, string title, Guid createdById)
    {
        Id = id;
        CreatedById = createdById;
        Title = title;
        Url = url;

        Tags = new List<LinkTagEntity>();

        LikesCount = 0;
        SavesCount = 0;
        DateCreated = DateTime.UtcNow;
        DateUpdated = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a NEW LinkEntity (one that doesn't already exist)
    /// </summary>
    /// <param name="url">The url to the page</param>
    /// <param name="title">The title of the page</param>
    /// <param name="createdById">The id of the user who submitted the link</param>
    /// <param name="tags">The optional tags that are associated with the link</param>
    /// <returns></returns>
    public static LinkEntity Create(string url, string title, Guid createdById, string[]? tags)
    {
        var now = DateTime.UtcNow;

        return new LinkEntity
        {
            IsActive = true,
            CreatedById = createdById,
            Tags = tags?.Select(x => new LinkTagEntity(x)).ToList() ?? new List<LinkTagEntity>(),
            Title = title,
            Url = url,

            DateCreated = now,
            DateUpdated = now
        };
    }
}