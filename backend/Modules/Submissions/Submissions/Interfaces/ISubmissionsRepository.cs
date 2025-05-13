using FluentResults;

using Submissions.Data;
using Submissions.Enums;

namespace Submissions.Interfaces;

public interface ISubmissionsRepository
{
    Task<Result<string>> GetAsync(Guid submissionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to get the first submission that is in the specified state.
    /// </summary>
    /// <param name="state"></param>
    /// <param name="count">The number of submissions for this state, to be returned</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<IEnumerable<SubmissionEntity>>> GetByStateAsync(SubmissionState state, int count = 25, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves a submission to the database.
    /// </summary>
    /// <returns>The unique id for the submission</returns>
    Task<Result> SaveAsync(SubmissionEntity entity, CancellationToken cancellationToken = default);

    Task<Result> SaveAsync(IEnumerable<SubmissionEntity> entities, CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(SubmissionEntity entity, CancellationToken cancellationToken = default);
}