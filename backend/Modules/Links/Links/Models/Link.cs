namespace Links.Models;

public sealed record Link
{
    public string Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Domain { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the image for the site/page that the link belongs to.
    /// </summary>
    /// <value>
    /// The image.
    /// </value>
    public string ImageUrl { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public bool IsFlagged { get; set; }

    public string[] Keywords { get; set; } = [];

    public int LikesCount { get; set; }

    public int SavesCount { get; set; }

    public List<LinkTag> Tags { get; set; } = [];

    public string Title { get; set; }

    public string Url { get; set; }

    public string SubmittedById { get; set; } = Guid.Empty.ToString();

    public DateTime DateCreated { get; set; }

    public DateTime DateUpdated { get; set; }

    // Needed for deserialization (Json constructor?)
    public Link() { }

    public Link(string id, string url, string submittedById)
    {
        Id = id;
        Title = string.Empty;
        Url = url;
        SubmittedById = submittedById;
    }

    public Link(Guid id, string url, Guid submittedById)
    {
        if (id.Equals(Guid.Empty))
            throw new ArgumentNullException(nameof(id), "Id cannot be empty.");

        if (submittedById.Equals(Guid.Empty))
            throw new ArgumentNullException(nameof(submittedById), "SubmittedById cannot be empty.");

        Id = id.ToString();
        Title = string.Empty;
        Url = url;
        SubmittedById = submittedById.ToString();
    }
}