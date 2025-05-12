﻿using System.Linq.Expressions;

using Common.Data.Interfaces;

using MongoDB.Driver;

namespace Common.Data;

//TODO: Merge these two together. Create new BaseDbRepository and have MongoDbRepository inherit from it.
public class MongoDbRepository<TDocument> :
    MongoDbRepository<TDocument, Guid> where TDocument :
    IMongoDocument
{
    protected MongoDbRepository(MongoDbContext<TDocument> dbContext) : base(dbContext) { }

    public override async Task<TDocument> GetDocumentAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id.Equals(Guid.Empty))
            throw new ArgumentException(EXCEPTION_ID_CANT_BE_EMPTY, nameof(id));

        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);

        var results = await Collection.FindAsync(filter, cancellationToken: cancellationToken);

        return await results.SingleOrDefaultAsync(cancellationToken);
    }

    public override async Task<IEnumerable<TDocument>> GetDocumentAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        var idsArray = ids?.ToArray() ?? [];

        if (!idsArray.Any())
            throw new ArgumentException(EXCEPTION_ID_CANT_BE_EMPTY, nameof(ids));

        var filter = Builders<TDocument>.Filter.In("_id", idsArray);

        var results = await FindDocumentsAsync(filter, cancellationToken);

        return results ?? Enumerable.Empty<TDocument>();
    }

    public override bool RemoveDocument(Guid id, CancellationToken cancellationToken = default)
    {
        var rslt = Collection.DeleteOne(d => d.Id == id, cancellationToken: cancellationToken);

        return rslt is { IsAcknowledged: true, DeletedCount: > 0 };
    }

    public override async Task<bool> RemoveDocumentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var rslt = await Collection.DeleteOneAsync(d => d.Id == id, cancellationToken);

        return rslt is { IsAcknowledged: true, DeletedCount: > 0 };
    }


    public override bool RemoveDocumentRange(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TDocument>.Filter.In(doc => doc.Id, ids);

        var rslt = Collection.DeleteMany(filter, cancellationToken: cancellationToken);

        return rslt is { IsAcknowledged: true, DeletedCount: > 0 };
    }

    public override async Task<bool> RemoveDocumentRangeAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TDocument>.Filter.In(doc => doc.Id, ids);

        var rslt = await Collection.DeleteManyAsync(filter, cancellationToken: cancellationToken);

        return rslt is { IsAcknowledged: true, DeletedCount: > 0 };
    }
}

public abstract class MongoDbRepository<TDocument, TId>(MongoDbContext<TDocument> dbContext) :
    IMongoDbRepository<TDocument, TId>
    where TDocument : IMongoDocument
{
    private readonly IMongoDbContext _context = dbContext;

    // Best to have your messages in a const for performance reasons
    internal const string EXCEPTION_COLLECTION_NOT_FOUND = "The collection was not found";
    internal const string EXCEPTION_ID_CANT_BE_EMPTY = "The Id(s) cannot be empty";

    protected IMongoCollection<TDocument> Collection => _context.Collection<TDocument>() ??
        throw new InvalidOperationException(EXCEPTION_COLLECTION_NOT_FOUND);

    public virtual void AddDocument(TDocument document)
    {
        Collection.InsertOne(document);
    }

    public async Task AddDocumentAsync(TDocument document, CancellationToken cancellationToken = default)
    {
        await Collection.InsertOneAsync(document, new InsertOneOptions(), cancellationToken);
    }

    public void AddDocuments(IEnumerable<TDocument> documents)
    {
        var documentsArray = documents?.ToArray() ?? [];

        if (!documentsArray.Any())
            throw new ArgumentException("The documents cannot be empty", nameof(documents));

        Collection.InsertManyAsync(documentsArray, new InsertManyOptions());
    }

    public async Task AddDocumentsAsync(
        IEnumerable<TDocument> documents,
        CancellationToken cancellationToken = default)
    {
        var documentsArray = documents?.ToArray() ?? [];

        if (!documentsArray.Any())
            throw new ArgumentException("The documents cannot be empty", nameof(documents));

        await Collection.InsertManyAsync(documentsArray, new InsertManyOptions(), cancellationToken: cancellationToken);
    }


    public Task<IEnumerable<TDocument>> FindDocumentsAsync(
        Expression<Func<TDocument, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<TDocument>.Filter.Where(predicate);

        return FindDocumentsAsync(filter, cancellationToken);
    }

    public Task<(IEnumerable<TDocument> Results, int TotalPages, int TotalCount)>
        FindDocumentsAsync(
            Expression<Func<TDocument, bool>> predicate,
            int pageNo,
            int pageSize,
            bool descending,
            Expression<Func<TDocument, object>>? orderBy = null,
            CancellationToken cancellationToken = default)
    {
        var filter = Builders<TDocument>.Filter.Where(predicate);

        return FindDocumentsAsync(filter, pageNo, pageSize, descending, orderBy, cancellationToken);
    }


    public async Task<TDocument?> FirstOrDefaultDocumentAsync(
        Expression<Func<TDocument, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<TDocument>.Filter.Where(predicate);

        var rslts = await FindDocumentsAsync(filter, cancellationToken);

        return rslts.FirstOrDefault();
    }


    public abstract Task<TDocument> GetDocumentAsync(
        TId id,
        CancellationToken cancellationToken = default);

    public abstract Task<IEnumerable<TDocument>> GetDocumentAsync(
        IEnumerable<TId> ids,
        CancellationToken cancellationToken = default);


    public Task<(IEnumerable<TDocument> Results, int TotalPages, int TotalCount)>
        ListDocumentsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<(IEnumerable<TDocument> Results, int TotalPages, int TotalCount)>
        ListDocumentsAsync(int pageNo, int pageSize, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public abstract bool RemoveDocument(TId id, CancellationToken cancellationToken = default);

    public bool RemoveDocument(TDocument entity, CancellationToken cancellationToken = default)
    {
        var rslt = Collection.DeleteOne(d =>
            d.Id == entity.Id,
            cancellationToken: cancellationToken);

        return rslt is { IsAcknowledged: true, DeletedCount: > 0 };
    }

    public abstract Task<bool> RemoveDocumentAsync(
        TId id, CancellationToken cancellationToken = default);

    public async Task<bool> RemoveDocumentAsync(
        TDocument entity, CancellationToken cancellationToken = default)
    {
        var id = entity.Id;

        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);

        var rslt = await Collection.DeleteOneAsync(
            filter, cancellationToken: cancellationToken);

        return rslt is { IsAcknowledged: true, DeletedCount: > 0 };
    }

    public abstract bool RemoveDocumentRange(
        IEnumerable<TId> ids, CancellationToken cancellationToken = default);

    public bool RemoveDocumentRange(
        IEnumerable<TDocument> entities, CancellationToken cancellationToken = default)
    {
        var ids = entities.Select(x => x.Id).AsEnumerable();

        var filter = Builders<TDocument>.Filter.In(doc => doc.Id, ids);

        var rslt = Collection.DeleteMany(filter, cancellationToken: cancellationToken);

        return rslt is { IsAcknowledged: true, DeletedCount: > 0 };
    }


    public abstract Task<bool> RemoveDocumentRangeAsync(
        IEnumerable<TId> ids,
        CancellationToken cancellationToken = default);

    public async Task<bool> RemoveDocumentRangeAsync(
        IEnumerable<TDocument> entities,
        CancellationToken cancellationToken = default)
    {
        var ids = entities.Select(x => x.Id).AsEnumerable();

        var filter = Builders<TDocument>.Filter.In(doc => doc.Id, ids);

        var rslt = await Collection.DeleteManyAsync(
            filter, cancellationToken: cancellationToken);

        return rslt is { IsAcknowledged: true, DeletedCount: > 0 };
    }

    public virtual async Task<long> UpdateDocumentAsync(
        TDocument document,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var rslt = await Collection
                .ReplaceOneAsync(d => d.Id == document.Id,
                    document,
                    cancellationToken: cancellationToken);

            return rslt.ModifiedCount;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    #region - Privates -

    protected async Task<IEnumerable<TDocument>> FindDocumentsAsync(
        FilterDefinition<TDocument> filter,
        CancellationToken cancellationToken = default)
    {
        if (filter is null)
            throw new ArgumentNullException(nameof(filter));

        try
        {
            var cursor = await Collection.FindAsync(filter, null, cancellationToken);

            var results = await cursor.ToListAsync(cancellationToken);

            return results;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }

    internal async Task<(IEnumerable<TDocument> Results, int TotalPages, int TotalCount)>
        FindDocumentsAsync(
        FilterDefinition<TDocument> filter,
        int pageNo,
        int pageSize,
        bool descending,
        Expression<Func<TDocument, object>>? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);
        ArgumentOutOfRangeException.ThrowIfNegative(pageNo);
        ArgumentOutOfRangeException.ThrowIfNegative(pageSize);

        var skip = (pageNo - 1) * pageSize;

        var totalCount = await Collection.CountDocumentsAsync(
            filter,
            null,
            cancellationToken);

        if (totalCount == 0)
            return (Enumerable.Empty<TDocument>(), 0, 0);

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var sortDefinition = descending
            ? Builders<TDocument>.Sort.Descending(t => t.Id)
            : Builders<TDocument>.Sort.Ascending(t => t.Id);

        if (orderBy is not null)
        {
            if (descending)
                sortDefinition = Builders<TDocument>.Sort.Descending(orderBy);
            else
                sortDefinition = Builders<TDocument>.Sort.Ascending(orderBy);
        }

        var options = new FindOptions<TDocument>
        {
            Skip = skip,
            Limit = pageSize,
            Sort = sortDefinition
        };

        var cursor = await Collection.FindAsync(filter, options, cancellationToken);

        var results = await cursor.ToListAsync(cancellationToken);

        return (results, totalPages, (int)totalCount);
    }

    #endregion
}