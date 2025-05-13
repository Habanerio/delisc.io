using Common.Data;

using MongoDB.Bson.Serialization.Attributes;

namespace Links.Data;

// Note: LinkTagEntity and UserLinkTagEntity are identical.
//       Not sure how to share them nicely, while also keeping the domains separate.
//       I could create a shared library in Modules, but, then things will be spread out more so than they are now.
public class LinkTagEntity : MongoDbDocument // MongoEntityBase, IIsSoftDeletableBy<Guid>
{
    private string _name = string.Empty;

    [BsonElement("name")]
    public string Name
    {
        get => _name;
        set => _name = value.ToLowerInvariant();
    }

    [BsonElement("count")]
    public int Count { get; set; }

    [BsonElement("weight")]
    public float Weight { get; set; }

    public LinkTagEntity(string name, int count = 1, float weight = 0)
    {
        Name = name.Replace('/', '-').ToLowerInvariant().Trim();
        Count = count;
        Weight = weight;
    }

    public static LinkTagEntity Create(string name)
    {
        return new LinkTagEntity(name);
    }
}