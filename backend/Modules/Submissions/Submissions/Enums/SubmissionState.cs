namespace Submissions.Enums;

public enum SubmissionState
{
    New = 0,

    Validating = 10,
    Validated = 11,
    Invalidated = 12,
    ValidationError = 19,

    Crawling = 20,
    Crawled = 21,
    CrawlingError = 29,

    Tagging = 30,
    Tagged = 31,
    TaggingError = 39,

    Approved = 100,
    Exists = 101,
    InvalidProtocol,
    Rejected = 102,
    Banned = 103,
}