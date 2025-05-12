using MongoDB.Driver;

namespace Common.Data.Interfaces;

public interface IMongoDbContext<TDocument> where TDocument : IMongoDocument
{
    IMongoCollection<TDocument> Collection { get; }
}

public interface IMongoDbContext
{
    IMongoCollection<TDocument> Collection<TDocument>();
}