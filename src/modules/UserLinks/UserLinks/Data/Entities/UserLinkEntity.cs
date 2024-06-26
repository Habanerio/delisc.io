using System.ComponentModel.DataAnnotations.Schema;
using Deliscio.Core.Data.Mongo;
using Deliscio.Core.Data.Mongo.Attributes;
using MongoDB.Bson;

namespace Deliscio.Modules.UserLinks.Data.Entities;

/// <summary>
/// Represents a link that is associated with a individual user.
/// </summary>
/// <seealso cref="MongoEntityBase" />
[Table("UserLink")]
[BsonCollection("user_links")]
public class UserLinkEntity : MongoEntityBase
{
    /// <summary>
    /// Gets/sets the id of the associated link in the central repository.
    /// </summary>
    /// <remarks>
    /// I could have used the Id property since it's a one-to-one.
    /// However, when adding a UserLinkEntity, there is no way to know if the link was added as the Add returns Task/void.
    /// Upon adding this link, it's unique id will be generated if successful.
    /// As well, this will add a layer of secrecy since each link will have a unique id, and therefore can not be guessed,
    /// since Get takes in the user link's id, and not the base link id.
    /// </remarks>
    //[BsonId]
    public ObjectId LinkId { get; set; }

    //[BsonId]
    public ObjectId UserId { get; set; }

    /// <summary>
    /// The description/summary of the page that the link points to.
    /// This is retrieved/built from the page itself and not user submitted*.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    public bool IsPrivate { get; set; } = false;

    /// <summary>
    /// Represents all of the tags that are associated with this link, as well as the number of times they've been used.
    /// </summary>
    /// <value>
    /// The tags.
    /// </value>
    public UserLinkTagEntity[] Tags { get; set; } = Array.Empty<UserLinkTagEntity>();

    /// <summary>
    /// The title of the page, from the page itself, that the link points to.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    public string Url { get; set; }

    private UserLinkEntity() { }

    public UserLinkEntity(string id, string userId, string linkId, DateTimeOffset dateCreated, DateTimeOffset dateUpdated, UserLinkTagEntity[]? tags = default, bool isPrivate = false) : this()
    {
        Id = id.ToObjectId();
        LinkId = linkId.ToObjectId();
        UserId = userId.ToObjectId();

        IsPrivate = isPrivate;
        Tags = tags ?? Array.Empty<UserLinkTagEntity>();

        DateCreated = dateCreated;
        DateUpdated = dateUpdated;
    }

    /// <summary>
    /// Creates an instance of a new UserLinkEntity.
    /// This is to be used when the UserLink does not yet exist in the database.
    /// </summary>
    /// <param name="linkId"></param>
    /// <param name="userId"></param>
    /// <param name="tags"></param>
    /// <param name="isPrivate"></param>
    /// <returns></returns>
    public static UserLinkEntity Create(string userId, string linkId, string title = "", UserLinkTagEntity[]? tags = default, bool isPrivate = false)
    {
        var now = DateTimeOffset.UtcNow;

        return new UserLinkEntity
        {
            UserId = userId.ToObjectId(),
            LinkId = linkId.ToObjectId(),

            IsPrivate = isPrivate,
            Tags = tags ?? Array.Empty<UserLinkTagEntity>(),

            DateCreated = now,
            DateUpdated = now
        };
    }
}