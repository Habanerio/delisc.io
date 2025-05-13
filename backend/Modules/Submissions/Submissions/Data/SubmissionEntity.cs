using Common.Data;
using Common.Data.Attributes;

using MongoDB.Bson.Serialization.Attributes;

using Submissions.Enums;

namespace Submissions.Data;

[BsonCollection("submissions")]
public class SubmissionEntity : MongoDbDocument
{
    [BsonElement("user_id")]
    public Guid UserId { get; set; } = Guid.Empty;

    [BsonElement("url")]
    public string Url { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("domain")]
    public string Domain { get; set; } = string.Empty;

    [BsonElement("link_id")]
    public Guid LinkId { get; set; } = Guid.Empty;

    [BsonElement("message")]
    public string Message { get; set; } = string.Empty;

    [BsonElement("meta_data")]
    public MetaData MetaData { get; set; } = new();

    [BsonElement("state")]
    public SubmissionState State { get; set; } = SubmissionState.New;

    [BsonElement("tags")]
    public List<string> Tags { get; set; } = [];

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("users_data")]
    public UsersData UsersData { get; set; } = new();

    [BsonElement("date_created")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    [BsonElement("date_updated")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime DateUpdated { get; set; } = DateTime.UtcNow;

    private SubmissionEntity() { }

    public static SubmissionEntity Create(Guid userId, string url, UsersData? usersData = null)
    {
        return new SubmissionEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Url = url,
            State = SubmissionState.New,
            UsersData = usersData ?? new UsersData(),
            DateCreated = DateTime.UtcNow,
        };
    }

    public static SubmissionEntity Load(
        Guid submissionId,
        Guid userId,
        string url,
        string[] tags,
        SubmissionState state,
        DateTime dateCreated,
        DateTime dateUpdated)
    {
        return new SubmissionEntity()
        {
            Id = submissionId,
            UserId = userId,
            Url = url,
            Tags = tags.ToList(),
            State = state,
            DateCreated = dateCreated,
            DateUpdated = dateUpdated
        };
    }
}

public record MetaData
{
    public string Author { get; set; } = string.Empty;
    public string CanonicalUrl { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
    public string? LastUpdate { get; set; } = string.Empty;
    public string OgImage { get; set; } = string.Empty;
    public string OgDescription { get; set; } = string.Empty;
    public string OgTitle { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
}

public sealed record UsersData
{
    public string? Description { get; set; }

    public string[]? Tags { get; set; }

    public string? Title { get; set; }

    public static UsersData Create(string title = "", string description = "", string[]? tags = default)
    {
        var rslt = new UsersData
        {
            Description = description,
            Tags = tags ?? Array.Empty<string>(),
            Title = title
        };

        return rslt;
    }
}