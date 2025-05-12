using MongoDB.Driver;

namespace Common.Data.Interfaces;

public interface IMongoDbClient
{
    IMongoDatabase Database { get; }
}