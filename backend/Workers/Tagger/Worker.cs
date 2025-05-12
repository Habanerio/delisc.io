using Microsoft.Extensions.AI;

using Submissions.Enums;
using Submissions.Interfaces;

namespace TagService;

public class Worker : BackgroundService
{
    private readonly IChatClient? _chatClient;
    private readonly ILogger<Worker> _logger;
    private readonly int _pollingIntervalSeconds = 5;
    private readonly ISubmissionsRepository _submissionsRepository;

    public Worker(
        IChatClient chatClient,
        ILogger<Worker> logger,
        ISubmissionsRepository submissionsRepository)
    {
        _chatClient = chatClient;

        _logger = logger ??
            throw new ArgumentNullException(nameof(logger));

        _submissionsRepository = submissionsRepository ??
            throw new ArgumentNullException(nameof(submissionsRepository));
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var rslt = await _submissionsRepository.GetAsync(
                    SubmissionState.Crawled, cancellationToken);

                if (!rslt.IsSuccess)
                {
                    _logger.LogError("Tagging - {time}: {errors}", DateTimeOffset.Now, rslt.Errors);

                    return;
                }

                if (rslt.ValueOrDefault is null)
                {
                    _logger.LogInformation("Tagging - {time}: No new submissions", DateTimeOffset.Now);

                    return;
                }

                var submission = rslt.ValueOrDefault;

                if (_chatClient is null)
                {
                    _logger.LogInformation("Ollama is null");

                    // WIll mark this as tagged, so that it may continue its journey to submission
                    submission.State = SubmissionState.Tagged;

                    await _submissionsRepository.UpdateAsync(submission, cancellationToken);

                    return;
                }

                _logger.LogInformation("Ollama is alive");

                var prompt =
                    $"You are an expert tag generator for a bookmarking application.{Environment.NewLine}" +
                    $"Your goal is to generate concise, relevant tags that users would use to categorize and find bookmarks.{Environment.NewLine}" +
                    $"Given the following webpage information (Title, URL, Author, Description, Keywords), " +
                    $"extract **5-7** tags that best represent the content.{Environment.NewLine}" +

                    $"*   Tags should be short (1-2 words max).{Environment.NewLine}" +
                    $"*   Tags should be relevant to the content of the webpage.{Environment.NewLine}" +
                    $"*   Consider subject matter, topic, and potential user search terms.{Environment.NewLine}" +
                    $"*   Tags should be in lowercase.{Environment.NewLine}" +
                    $"*   Tags should be comma separated.{Environment.NewLine}" +
                    $"*   Return **ONLY** the comma-separated list of tags.{Environment.NewLine}" +

                    $"No other text or explanation.{Environment.NewLine}" +

                    $"Webpage details could vary in subject manner, such as sports, food, politics, science, news, technology, fashion, etc, ...{Environment.NewLine}" +

                    $"Here is an EXAMPLE of what could be provided:{Environment.NewLine}" +
                    $"Title: Physicists Just Found a New Quantum Paradox That Casts Doubt on a Pillar of Reality : ScienceAlert{Environment.NewLine}" +
                    $"URL: https://www.sciencealert.com/a-new-quantum-paradox-throws-the-foundations-of-observed-reality-into-question/amp{Environment.NewLine}" +
                    $"Author: ScienceAlert Staff " +
                    $"Description: If a tree falls in a forest and no one is there to hear it, does it make a sound? Perhaps not, some say. " +
                    $"             This thought experiment has haunted the field of quantum mechanics for nearly a century, and now physicists " +
                    $"             have found a new way to challenge our perceptions of reality." +
                    $"Keywords: " +

                    $"Example Output: " +
                    $"science, quantum, quantum paradox, quantum mechanics, science, reality, physics, observation, thought experiment " +

                    $"Now, generate the tags for the details below. " +

                    $"These tags will be used to allow users to find pages of interest.{Environment.NewLine}" +
                    $"Title: {submission.Title}{Environment.NewLine} " +
                    $"Url: ${submission.Url}{Environment.NewLine} " +
                    $"Author = {submission.MetaData.Author}{Environment.NewLine} " +
                    $"Description: ${submission.Description}{Environment.NewLine} " +
                    $"Keywords: {submission.MetaData.Keywords}{Environment.NewLine} ";

                var response = await _chatClient.GetResponseAsync(prompt, cancellationToken: cancellationToken);

                if (response.Messages.Count > 0)
                {
                    // Example: azure, architecture, web apps, enterprise, cloud, patterns, development
                    var responseText = response.Messages[0].Text;

                    var tagsArray = responseText?.Split(',').Select(t => t.Trim()).ToList() ?? [];

                    submission.Tags = tagsArray.ToList();
                }

                submission.State = SubmissionState.Tagged;

                await _submissionsRepository.UpdateAsync(submission, cancellationToken);
            }
            catch (Exception e)
            {
                // Log the error
                _logger.LogError(e, "Error processing message.");

                // Dead letter?
            }

            await Task.Delay(_pollingIntervalSeconds, cancellationToken);
        }
    }
}