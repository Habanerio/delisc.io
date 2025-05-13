namespace Submissions.Enums;

public enum SubmissionState
{
    New = 0,

    Validating = 10,
    Validated = 11,
    ValidationError = 19,

    Crawling = 20,
    Crawled = 21,
    CrawlingError = 29,

    Tagging = 30,
    Tagged = 31,
    TaggingError = 39,

    Finalizing = 90,
    FinalizeError = 99,

    Approved = 100,
    Exists = 101,
    Rejected = 102
}