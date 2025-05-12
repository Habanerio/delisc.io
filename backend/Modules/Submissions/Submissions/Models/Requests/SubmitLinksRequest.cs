namespace Submissions.Models.Requests;

public record SubmitLinksRequest(string UserId, string[] Urls);