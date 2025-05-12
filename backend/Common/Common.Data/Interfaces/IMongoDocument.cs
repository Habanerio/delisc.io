namespace Common.Data.Interfaces;

/// <summary>
/// A wrapper interface for MongoDB entities that specified that the Id should be of type Guid.
/// </summary>
public interface IMongoDocument : IMongoDocument<Guid> { }

public interface IMongoDocument<out TId>
{
    TId Id { get; }
}