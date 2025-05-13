using FluentResults;

using Links.Data;

namespace Links.Interfaces;

public interface ILinksRepository
{
    #region - Links -

    Task<(IEnumerable<LinkEntity> Results, int TotalPages, int TotalCount)> FindAsync(
        string term, string tags, string domain,
        int pageNo, int pageSize, int skip = 0,
        bool? isActive = default, bool? isFlagged = default, bool? isDeleted = false,
        CancellationToken cancellationToken = default);

    Task<(IEnumerable<LinkEntity> Results, int TotalPages, int TotalCount)> FindAsync(
        string term, string[] tags, string domain,
        int pageNo, int pageSize, int skip = 0,
        bool? isActive = default, bool? isFlagged = default, bool? isDeleted = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an individual Link by its id.
    /// </summary>
    /// <param name="linkId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<LinkEntity?> GetAsync(Guid linkId, CancellationToken cancellationToken = default);

    Task<(IEnumerable<LinkEntity> Results, int TotalPages, int TotalCount)> GetLinksByTagsAsync(
        IEnumerable<string> tags, int pageNo = 1, int pageSize = 25,
        CancellationToken cancellationToken = default);

    Task<LinkEntity?> GetLinkByUrlAsync(string url, CancellationToken cancellationToken = default);

    Task<Result> SaveAsync(LinkEntity entity, CancellationToken cancellationToken = default);

    #endregion

    #region - Tags -
    /// <summary>
    /// Adds a tag to the Link with the id.<br />
    /// If the tag already exists, then its count will be incremented by 1.
    /// </summary>
    /// <param name="linkId">The id of the Link to update.</param>
    /// <param name="tag">The tag in which its count will be incremented.</param>
    /// <param name="cancellationToken">The token.</param>
    /// <returns>Task</returns>
    Task AddTagAsync(Guid linkId, string tag, CancellationToken cancellationToken);

    Task<IEnumerable<LinkTagEntity>> GetRelatedTagsAsync(
        string[] tags, int count, CancellationToken cancellationToken = default);

    Task<IEnumerable<LinkTagEntity>> GetRelatedTagsByDomainAsync(
        string domain, int count, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an occurrence of the tag from the Tag collection that belongs to Link with the id.<br />
    /// </summary>
    /// <param name="linkId">The id of the Link to update.</param>
    /// <param name="tag">The tag in which its count will be incremented.</param>
    /// <param name="cancellationToken">The token.</param>
    /// <returns>Task</returns>
    Task RemoveTagAsync(Guid linkId, string tag, CancellationToken cancellationToken);
    #endregion

}