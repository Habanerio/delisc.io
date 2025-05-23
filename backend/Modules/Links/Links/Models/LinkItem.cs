﻿namespace Links.Models;

/// <summary>
/// Represents a trimmed down version of the Link model, used for displaying links in lists or collections.
/// </summary>
public sealed record LinkItem
{
    public string Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Domain { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public int Likes { get; set; }

    public int Saves { get; set; }

    public List<LinkTag> Tags { get; set; } = [];

    public string Title { get; set; }

    public string Url { get; set; }

    public DateTimeOffset DateCreated { get; set; }

    public DateTimeOffset DateUpdated { get; set; }

    // Needed for deserialization
    public LinkItem() { }

    public LinkItem(
        Guid id,
        string url,
        string title,
        string description,
        string domain,
        string imageUrl,
        IEnumerable<LinkTag>? tags,
        DateTimeOffset dateCreated,
        DateTimeOffset dateUpdated) :
        this(
            id.ToString(),
            url,
            title,
            description,
            domain,
            imageUrl,
            tags,
            dateCreated,
            dateUpdated)
    { }

    public LinkItem(
        string id,
        string url,
        string title,
        string description,
        string domain,
        string imageUrl,
        IEnumerable<LinkTag>? tags,
        DateTimeOffset dateCreated,
        DateTimeOffset dateUpdated)
    {
        Id = id;
        Description = description;
        Domain = domain;
        ImageUrl = imageUrl;
        Tags = tags?.ToList() ?? [];
        Title = title;
        Url = url;
        DateCreated = dateCreated;
        DateUpdated = dateUpdated;
    }
}