using Common.Data;

using FluentResults;

using MongoDB.Driver;

using Submissions.Enums;
using Submissions.Interfaces;

namespace Submissions.Data;

public class SubmissionsRepository(IMongoDatabase mongoDb) :
    MongoDbRepository<SubmissionEntity>(new SubmissionsDbContext<SubmissionEntity>(mongoDb)),
    ISubmissionsRepository
{
    public Task<Result<string>> GetAsync(Guid submissionId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets a collection of submissions that are in a specified state.
    /// </summary>
    /// <param name="state">The state of the submission, for which to filter by</param>
    /// <param name="count"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result<IEnumerable<SubmissionEntity>>> GetByStateAsync(SubmissionState state, int count = 25, CancellationToken cancellationToken = default)
    {
        var rslts = await FindDocumentsAsync(d => d.State.Equals(state), cancellationToken);

        return Result.Ok(rslts ?? []);
    }

    public async Task<Result> SaveAsync(SubmissionEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity.UserId.Equals(Guid.Empty))
            return Result.Fail("UserId cannot be empty");

        if (string.IsNullOrWhiteSpace(entity.Url))
            return Result.Fail("Url cannot be null or whitespace");

        try
        {
            entity.DateCreated = DateTime.UtcNow;
            await AddDocumentAsync(entity, cancellationToken);
        }
        catch (Exception e)
        {
            // Log

            return Result.Fail(e.Message);
        }

        return Result.Ok();
    }

    public async Task<Result> SaveAsync(IEnumerable<SubmissionEntity> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            await AddDocumentsAsync(entities, cancellationToken);
        }
        catch (Exception e)
        {
            // Log

            return Result.Fail(e.Message);
        }

        return Result.Ok();
    }

    public async Task<Result> UpdateAsync(SubmissionEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            entity.DateUpdated = DateTime.UtcNow;
            await UpdateDocumentAsync(entity, cancellationToken);
        }
        catch (Exception e)
        {
            // Log

            return Result.Fail(e.Message);
        }

        return Result.Ok();
    }
}