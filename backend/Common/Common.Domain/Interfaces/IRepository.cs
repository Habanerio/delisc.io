using System.Linq.Expressions;

using Common.Data;

namespace Common.Domain.Interfaces;

public interface IRepositoryWithTypedId<T, in TId> where T : MongoDbDocument
{
    Task<(IEnumerable<T> Results, int TotalPages, int TotalCount)> FindAsync(
        Expression<Func<T, bool>> predicate,
        int pageNo = 1,
        int pageSize = 25,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a specific entity based on its id of type TId.
    /// </summary>
    /// <param name="id">The unique Id for the entity.</param>
    /// <returns>An entity of type T</returns>
    T? Get(TId id);

    /// <summary>
    /// Gets a collection of one or more backlinks by their id.
    /// </summary>
    /// <param name="ids">The ids.</param>
    /// <returns></returns>
    IEnumerable<T> Get(IEnumerable<TId> ids);

    /// <summary>
    /// Finds a specific entity based on its id of type TId.
    /// </summary>
    /// <param name="id">The unique Id for the entity.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>An entity of type T</returns>
    Task<T?> GetAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a collection of one or more T by their id.
    /// </summary>
    /// <param name="ids">The ids.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<T>> GetAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default);

    Task<(IEnumerable<T> Results, int TotalPages, int TotalCount)> GetAsync(
        int pageNo,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);


    #region - CRUD -
    /// <summary>
    /// Adds a single entity of type T to the repository.
    /// </summary>
    /// <param name="entity">The entity to be saved.</param>
    void Add(T entity);

    /// <summary>
    /// Adds a single entity of type T to the repository - asynchronously.
    /// </summary>
    /// <param name="entity">The entity to be saved.</param>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a collection of entities of type T to the repository.
    /// </summary>
    /// <param name="entities">The entity.</param>
    void AddRange(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    void Remove(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a single entity of type T from the repository.
    /// </summary>
    /// <param name="entity">The entity.</param>
    void Remove(T entity, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(TId id, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a collection of entities.
    /// </summary>
    /// <param name="ids">The ids of the entities to be removed.</param>
    /// <param name="cancellationToken">The token.</param>
    void RemoveRange(IEnumerable<TId> ids, CancellationToken cancellationToken = default);

    Task RemoveRangeAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default);

    void Save();

    Task SaveAsync(CancellationToken cancellationToken = default);

    void Update(T entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    #endregion


}

public interface IRepository<T> : IRepositoryWithTypedId<T, Guid> where T : MongoDbDocument
{
}