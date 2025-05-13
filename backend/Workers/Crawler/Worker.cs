using System.Net;

using FluentResults;

using HtmlAgilityPack;

using Submissions.Data;
using Submissions.Enums;
using Submissions.Interfaces;

namespace CrawlServices;

public class Worker : BackgroundService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<Worker> _logger;
    private readonly int _pollingIntervalSeconds = 5;
    private readonly ISubmissionsRepository _submissionRepository;

    private const string CRAWL_ERROR_HTTP_CLIENT =
        "{time}: Crawling Error for: {url} {newLine}Message: {message}";

    public Worker(
        HttpClient httpClient,
        ILogger<Worker> logger,
        ISubmissionsRepository submissionRepository)
    {
        _httpClient = httpClient ??
            throw new ArgumentNullException(nameof(httpClient));

        _logger = logger ??
            throw new ArgumentNullException(nameof(logger));

        _submissionRepository = submissionRepository ??
            throw new ArgumentNullException(nameof(submissionRepository));
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var rslts = await _submissionRepository.GetByStateAsync(
                    SubmissionState.Validated, cancellationToken: cancellationToken);

                if (!rslts.IsSuccess)
                {
                    _logger.LogError("Crawling - {time}: {errors}", DateTimeOffset.Now, rslts.Errors);
                }
                else if (!rslts.ValueOrDefault.Any())
                {
                    _logger.LogInformation("Crawling - {time}: No new submissions", DateTimeOffset.Now);
                }
                else
                {
                    await ProcessMessagesAsync(rslts.ValueOrDefault, cancellationToken);
                }
            }
            catch (Exception e)
            {
                // Log the error
                _logger.LogError(e, "Error while attempting to retrieve submissions to crawl.");

                // Dead letter?
            }

            await Task.Delay(_pollingIntervalSeconds, cancellationToken);
        }
    }

    private async Task ProcessMessagesAsync(IEnumerable<SubmissionEntity> submissions, CancellationToken cancellationToken)
    {
        foreach (var submission in submissions)
        {
            submission.State = SubmissionState.Crawling; // Update status in DB
            await _submissionRepository.UpdateAsync(submission, cancellationToken);

            _logger.LogInformation("Crawling - {time}: Processing `{url}`",
                DateTimeOffset.Now, submission.Url);

            try
            {
                var meta = await FetchMetaAsync(submission, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error crawling submission: {SubmissionId}", submission.Id);

                submission.State = SubmissionState.ValidationError;
                submission.Message = $"Crawling error: {e.Message}";

                await _submissionRepository.UpdateAsync(submission, cancellationToken);

                // Dead letter?
            }
        }
    }

    private async Task<Result<MetaData>> FetchMetaAsync(
        SubmissionEntity submission, CancellationToken cancellationToken = default)
    {
        var htmlDocument = new HtmlDocument();
        HttpResponseMessage? response = null;

        try
        {
            response = await _httpClient.GetAsync(submission.Url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            htmlDocument.LoadHtml(content);
        }
        catch (Exception e)
        {
            _logger.LogError(CRAWL_ERROR_HTTP_CLIENT,
                DateTimeOffset.Now, submission.Url, e.Message, Environment.NewLine);

            var statusCode = response?.StatusCode ??
                HttpStatusCode.BadRequest;

            submission.State = SubmissionState.CrawlingError;
            submission.Message = $"Crawling error: {e.Message}";

            await _submissionRepository.UpdateAsync(submission, cancellationToken);

            return Result.Fail($"Could not crawl the link\nStatus: {statusCode}");
        }

        var ogTitle = WebUtility.HtmlDecode(htmlDocument.DocumentNode
            .SelectSingleNode("//meta[@property='og:title']")
            ?.Attributes["content"]
            ?.Value ?? string.Empty).Trim();

        var title = WebUtility.HtmlDecode(htmlDocument.DocumentNode
            .SelectSingleNode("//title")?.InnerText ??
            string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(title))
        {
            if (!string.IsNullOrWhiteSpace(ogTitle))
                title = ogTitle;
            else
                title = submission.Url;
        }

        var description =
            htmlDocument.DocumentNode.SelectSingleNode("//meta[@name='description']")?.Attributes["content"]?.Value.Trim() ??
            string.Empty;

        var ogDescription =
            WebUtility.HtmlDecode(htmlDocument.DocumentNode.SelectSingleNode("//meta[@property='og:description']")
                ?.Attributes["content"]
                ?.Value ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(description))
        {
            if (!string.IsNullOrWhiteSpace(ogDescription))
                description = ogDescription;
        }

        var meta = new MetaData
        {
            Title = title,
            OgTitle = ogTitle,
            Description = description,
            OgDescription = ogDescription,

            Author = htmlDocument.DocumentNode.SelectSingleNode("//meta[@name='author']")?.Attributes["content"]?.Value.Trim() ?? string.Empty,
            CanonicalUrl = htmlDocument.DocumentNode.SelectSingleNode("//link[@rel='canonical']")?.Attributes["href"]?.Value.Trim() ?? string.Empty,
            Keywords = htmlDocument.DocumentNode.SelectSingleNode("//meta[@name='keywords']")?.Attributes["content"]?.Value.Trim() ?? string.Empty,
            LastUpdate = htmlDocument.DocumentNode.SelectSingleNode("//meta[@name='last-modified']")?.Attributes["content"]?.Value.Trim() ?? string.Empty,
            OgImage = htmlDocument.DocumentNode.SelectSingleNode("//meta[@property='og:image']")?.Attributes["content"]?.Value.Trim() ?? string.Empty
        };

        submission.Title = title;
        submission.Description = description;

        submission.MetaData = meta;
        submission.State = SubmissionState.Crawled;
        await _submissionRepository.UpdateAsync(submission, cancellationToken);

        return meta;
    }
}
