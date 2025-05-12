using Common.Data;
using Common.Data.Interfaces;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Submissions.Data;

public class SubmissionsDbContext<TDocument> : MongoDbContext<TDocument> where TDocument : IMongoDocument
{
    public SubmissionsDbContext(IMongoDatabase mongoDb) : base(mongoDb)
    {
        //TODO: Come back and uncomment
        //Configure();
    }

    protected void Configure()
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        BsonClassMap.RegisterClassMap<SubmissionEntity>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true);
            cm.SetDiscriminator("_t");
            cm.SetDiscriminatorIsRequired(true);
            cm.SetIgnoreExtraElements(true);
        });

        var indexKeysDefinition = Builders<SubmissionEntity>.IndexKeys
            .Ascending(a => a.UserId)
            .Ascending(a => a.Id);

        var createIndexModel = new CreateIndexModel<SubmissionEntity>(
            indexKeysDefinition,
            new CreateIndexOptions { Name = "UserId_Id_Index" }
        );

        Collection<SubmissionEntity>().Indexes.CreateOne(createIndexModel);
    }
}