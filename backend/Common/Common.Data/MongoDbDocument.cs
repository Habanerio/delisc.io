using Common.Data.Interfaces;

using MongoDB.Bson.Serialization.Attributes;

namespace Common.Data;

public abstract class MongoDbDocument : IMongoDocument
{
    [BsonId]
    public Guid Id { get; set; } = Guid.NewGuid();
}