using FluentResults;

using Links.Interfaces;

using Submissions.Data;
using Submissions.Enums;
using Submissions.Interfaces;

namespace ValidateService;

/// <summary>
/// Responsible for verifying that the link doesn't already exist in the database, <br />
/// AND, that the link is valid (e.g. not a 404, etc.) or a from a banned domain.
/// </summary>
/// <remarks>This is a very crude version. Would like to get RabbitMQ into here at some point</remarks>
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly int _pollingIntervalSeconds = 30;
    private readonly ILinksRepository _linksRepository;
    private readonly ISubmissionsRepository _submissionRepository;

    private const string VERIFYING_STARTED_MESSAGE = "{time}: Verifying Started for: {url}";
    private const string VERIFYING_COMPLETED_MESSAGE = "{time}: Verifying Completed for: {url}";
    private const string VERIFYING_ERROR_MESSAGE = "{time}: Verifying Error for: {url}\nMessage: {message}";
    private const string VERIFYING_LINK_ALREADY_EXISTS_MESSAGE = "{time}: Link already exists for: {url}";


    public Worker(
        ILogger<Worker> logger,
        ILinksRepository linksRepository,
        ISubmissionsRepository submissionRepository)
    {
        _logger = logger ??
            throw new ArgumentNullException(nameof(logger));

        _linksRepository = linksRepository ??
            throw new ArgumentNullException(nameof(linksRepository));

        _submissionRepository = submissionRepository ??
            throw new ArgumentNullException(nameof(submissionRepository));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var rslts = await _submissionRepository.GetByStateAsync(
                    SubmissionState.New, cancellationToken: stoppingToken);

                if (!rslts.IsSuccess)
                {
                    _logger.LogError("Validating - {time}: {errors}", DateTimeOffset.Now, rslts.Errors);

                    Console.WriteLine(rslts.Errors);
                }
                else if (!rslts.ValueOrDefault.Any())
                {
                    _logger.LogInformation("Validating - {time}: No new submissions", DateTimeOffset.Now);

                    Console.WriteLine("No new submissions");
                }
                else
                {
                    await ProcessMessagesAsync(rslts.ValueOrDefault, stoppingToken);
                }
            }
            catch (Exception e)
            {
                // Log the error
                _logger.LogError(e, "Error while attempting to retrieve submissions to validate.");

                // Dead letter?
            }

            await Task.Delay(TimeSpan.FromSeconds(_pollingIntervalSeconds), stoppingToken);
        }
    }

    private async Task ProcessMessagesAsync(IEnumerable<SubmissionEntity> submissions, CancellationToken cancellationToken)
    {
        foreach (var submission in submissions)
        {
            try
            {
                _logger.LogInformation("Validating - {time}: Processing `{url}`",
                    DateTimeOffset.Now, submission.Url);

                // Validation Logic
                submission.State = SubmissionState.Validating; // Update status in DB
                await _submissionRepository.UpdateAsync(submission, cancellationToken);

                if (!submission.Url.StartsWith("http"))
                {
                    _logger.LogWarning(VERIFYING_ERROR_MESSAGE,
                        DateTimeOffset.Now, submission.Url, "Invalid URL format");

                    submission.State = SubmissionState.Rejected;
                    submission.Message = $"Invalid URL: {submission.Url}";

                    await _submissionRepository.UpdateAsync(submission, cancellationToken);

                    continue;
                }

                var domain = ParseDomainFromUrl(submission.Url);

                if (string.IsNullOrWhiteSpace(domain))
                {
                    _logger.LogWarning(VERIFYING_ERROR_MESSAGE,
                        DateTimeOffset.Now, submission.Url, "Domain could not be parsed");

                    submission.State = SubmissionState.Rejected;
                    submission.Message = $"Domain could not be parsed: {submission.Url}";

                    await _submissionRepository.UpdateAsync(submission, cancellationToken);

                    continue;
                }

                submission.Domain = domain;

                // Should check if Url exists in the queue too.

                var doesExistResult = await ValidateAlreadyExistsAsync(submission, cancellationToken);

                if (doesExistResult.IsFailed)
                {
                    return;
                }

                var isBannedDomain = ValidateDomain(submission.Url);

                if (!isBannedDomain.IsSuccess)
                {
                    _logger.LogWarning(VERIFYING_ERROR_MESSAGE, DateTimeOffset.Now, submission.Url, isBannedDomain.Errors);

                    submission.State = SubmissionState.Rejected;
                    submission.Message = $"Banned domain: {submission.Url}";

                    await _submissionRepository.UpdateAsync(submission, cancellationToken);

                    return;
                }

                submission.State = SubmissionState.Validated;
                await _submissionRepository.UpdateAsync(submission, cancellationToken);

                _logger.LogInformation("`{url}` was successfully validated", submission.Url);
            }
            catch (Exception e)
            {
                submission.State = SubmissionState.ValidationError;
                submission.Message = $"Validation error: {e.Message}";

                await _submissionRepository.UpdateAsync(submission, cancellationToken);

                _logger.LogError(e, "Error validating submission: {SubmissionId}", submission.Id);

                // Dead letter?
            }
        }
    }

    /// <summary>
    /// Check to see if the link already exists in the database.
    /// </summary>
    /// <param name="submission"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<Result> ValidateAlreadyExistsAsync(SubmissionEntity submission, CancellationToken cancellationToken)
    {
        // Check if the link already exists in the database
        var existingLink = await _linksRepository.GetLinkByUrlAsync(submission.Url, cancellationToken);

        if (existingLink != null)
        {
            _logger.LogWarning(VERIFYING_LINK_ALREADY_EXISTS_MESSAGE, DateTimeOffset.Now, submission.Url);

            submission.LinkId = existingLink.Id;
            submission.State = SubmissionState.Exists;

            await _submissionRepository.UpdateAsync(submission, cancellationToken);

            return Result.Fail($"`{submission.Url}` already exists");
        }

        return Result.Ok();
    }

    /// <summary>
    /// Check to see if the host of the url is of a banned domain.
    /// </summary>
    /// <param name="domain"></param>
    /// <returns></returns>
    private Result ValidateDomain(string domain)
    {
        // Just an example of a list of (partial) domains.
        // Should add to appsettings or db.
        var bannedHosts = new[]
        {
            "example.com",
            "test.com",
            "mail.",
            "calendar.",
            // Should get list from `https://github.com/Ultimate-Hosts-Blacklist`
            "4k-stream-tv01.blogspot.com",
            "a.aberatii.com",
            "adplay.ru",
            "ads.schmoozecom.net",
            "ads.talkscreativity.com",
            "adt.ad-spire.net",
            "airlogak.com",
            "analytics.wifi4games.com",
            "apemc85.fr",
            "api.7dshfewr-0ewfivjkys.xyz",
        };

        if (bannedHosts.Any(x => domain.Contains(x, StringComparison.OrdinalIgnoreCase)))
            return Result.Fail($"Banned domain: {domain}");

        return Result.Ok();
    }

    private static string ParseDomainFromUrl(string url)
    {

        if (Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
        {
            var host = uri?.Host?.Replace("www.", "").ToLower() ?? string.Empty;

            return host;
        }

        return string.Empty;
    }
}
