using Links.Data;
using Links.Interfaces;

using Submissions.Data;
using Submissions.Enums;
using Submissions.Interfaces;

namespace FinalizeService;

/// <summary>
/// Responsible for finalizing the submissions.
/// </summary>
public class Worker : BackgroundService
{
    private readonly ILinksRepository _linksRepository;
    private readonly ISubmissionsRepository _submissionsRepository;
    private readonly ILogger<Worker> _logger;

    private readonly int _pollingIntervalSeconds = 15;

    public Worker(ILinksRepository linksRepository, ILogger<Worker> logger,
        ISubmissionsRepository submissionsRepository)
    {
        _logger = logger;

        _linksRepository = linksRepository ??
                           throw new ArgumentNullException(nameof(linksRepository));

        _submissionsRepository = submissionsRepository ??
                                 throw new ArgumentNullException(nameof(submissionsRepository));
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var taggedRslts = await _submissionsRepository.GetByStateAsync(
                    SubmissionState.Tagged, cancellationToken: cancellationToken);

                var existsRslts = await _submissionsRepository.GetByStateAsync(
                    SubmissionState.Exists, cancellationToken: cancellationToken);

                //TODO: Get submissions that exist.

                if (!taggedRslts.IsSuccess)
                {
                    _logger.LogError("Finalizing - {time}: {errors}", DateTimeOffset.Now, taggedRslts.Errors);
                }
                else if (!taggedRslts.ValueOrDefault.Any())
                {
                    _logger.LogInformation("Finalizing - {time}: No new submissions", DateTimeOffset.Now);
                }
                else
                {
                    await ProcessMessagesAsync(taggedRslts.ValueOrDefault, cancellationToken);
                }
            }
            catch (Exception e)
            {
                // Log the error
                _logger.LogError(e, "Error while attempting to retrieve submissions to finalize.");

                // Dead letter?
            }

            await Task.Delay(_pollingIntervalSeconds, cancellationToken);
        }
    }

    private async Task ProcessMessagesAsync(IEnumerable<SubmissionEntity> submissions,
        CancellationToken cancellationToken)
    {
        foreach (var submission in submissions)
        {
            try
            {
                var linkEntity = LinkEntity.Create(
                    submission.Url, submission.Title, submission.UserId,
                    submission.Tags?.ToArray() ?? []);
                linkEntity.Description = submission.Description;
                linkEntity.Domain = submission.Domain;
                linkEntity.ImageUrl = submission.MetaData.OgImage;
                linkEntity.Keywords = submission.MetaData.Keywords?.Split(',')?.ToList() ?? [];
                linkEntity.DateCreated = submission.DateCreated;

                var didLinkSave = await _linksRepository.SaveAsync(linkEntity, cancellationToken);

                if (!didLinkSave.IsSuccess)
                {
                    _logger.LogError("Error while finalizing the submission: {SubmissionId}", submission.Id);

                    submission.State = SubmissionState.FinalizeError;
                    submission.Message = $"Error while finalizing the submission: {didLinkSave.Errors}";
                }
                else
                {
                    submission.State = SubmissionState.Approved;
                    await _submissionsRepository.UpdateAsync(submission, cancellationToken);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
