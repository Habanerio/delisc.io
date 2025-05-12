namespace Common.Data.Attributes;

// https://medium.com/@marekzyla95/mongo-repository-pattern-700986454a0e

/// <summary>
/// Add this above your Mongo Documents
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class BsonCollectionAttribute(string collectionName) : Attribute
{
    public string CollectionName { get; } = collectionName;
}