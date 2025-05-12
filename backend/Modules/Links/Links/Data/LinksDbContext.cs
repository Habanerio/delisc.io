using Common.Data;
using Common.Data.Interfaces;

using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Links.Data;

public class LinksDbContext<TDocument> : MongoDbContext<TDocument> where TDocument : IMongoDocument
{
    public LinksDbContext(IMongoDatabase mongoDb) : base(mongoDb)
    {
        //TODO: Come back and uncomment
        Configure();
    }

    protected void Configure()
    {
        BsonClassMap.RegisterClassMap<LinkEntity>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true);
            cm.SetDiscriminator("_t");
            cm.SetDiscriminatorIsRequired(true);
            cm.SetIgnoreExtraElements(true);
        });

        var indexKeysDefinition = Builders<LinkEntity>.IndexKeys
            .Ascending(a => a.CreatedById)
            .Ascending(a => a.Id);

        var createIndexModel = new CreateIndexModel<LinkEntity>(
            indexKeysDefinition,
            new CreateIndexOptions { Name = "UserId_Id_Index" }
        );

        //Collection<LinkEntity>().Indexes.CreateOne(createIndexModel);
    }
}