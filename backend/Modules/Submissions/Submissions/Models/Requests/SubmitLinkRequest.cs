namespace Submissions.Models.Requests;

public record SubmitLinkRequest(string UserId, string Url, string[]? Tags = default);