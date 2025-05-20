using Links.Data;
using Links.Interfaces;

using Submissions.Data;
using Submissions.Enums;
using Submissions.Interfaces;

namespace FinalizeService;

/// <summary>
/// Responsible for finalizing the submissions.
/// </summary>
public class Worker(
    ILinksRepository linksRepository,
    ILogger<Worker> logger,
    ISubmissionsRepository submissionsRepository)
    : BackgroundService
{
    private readonly ILinksRepository _linksRepository = linksRepository ??
        throw new ArgumentNullException(nameof(linksRepository));

    private readonly ISubmissionsRepository _submissionsRepository = submissionsRepository ??
        throw new ArgumentNullException(nameof(submissionsRepository));

    private readonly int _pollingIntervalSeconds = 15;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var taggedRslts = await _submissionsRepository.GetByStateAsync(
                    SubmissionState.Tagged, cancellationToken: stoppingToken);

                var existsRslts = await _submissionsRepository.GetByStateAsync(
                    SubmissionState.Exists, cancellationToken: stoppingToken);

                //TODO: Get submissions that exist.

                if (!taggedRslts.IsSuccess)
                {
                    logger.LogError("Finalizing - {time}: {errors}", DateTimeOffset.Now, taggedRslts.Errors);
                }
                else if (!taggedRslts.ValueOrDefault.Any())
                {
                    logger.LogInformation("Finalizing - {time}: No new submissions", DateTimeOffset.Now);
                }
                else
                {
                    await ProcessMessagesAsync(taggedRslts.ValueOrDefault, stoppingToken);
                }
            }
            catch (Exception e)
            {
                // Log the error
                logger.LogError(e, "Error while attempting to retrieve submissions to finalize.");

                // Dead letter?
            }

            await Task.Delay(_pollingIntervalSeconds, stoppingToken);
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
                    logger.LogError("Error while finalizing the submission: {SubmissionId}", submission.Id);

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
