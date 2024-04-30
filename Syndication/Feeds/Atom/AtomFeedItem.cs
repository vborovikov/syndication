namespace Syndication.Feeds;

using System;
using System.Collections.Generic;
using System.Linq;
using Brackets;

/// <summary>
/// Atom 1.0 feed item object according to specification: https://validator.w3.org/feed/docs/atom.html
/// </summary>
public record AtomFeedItem : BaseFeedItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AtomFeedItem"/> class.
    /// Reads an atom feed based on the xml given in item
    /// </summary>
    /// <param name="item">feed item as xml</param>
    public AtomFeedItem(ParentTag item)
        : base(item)
    {
        // Required elements of <entry>
        this.Id = item.GetRequiredValue("id");
        this.UpdatedAsString = item.GetValue("updated") ?? item.GetValue("published") ?? string.Empty;
        this.Updated = Helpers.TryParseDateTime(this.UpdatedAsString);

        // Recommended elements of <entry>
        if (item.GetElement("author") is ParentTag author)
        {
            this.Author = new AtomPerson(author);
        }
        this.Content = item.GetValue("content");
        this.Links = item.GetArray("link", link => new AtomLink(link));
        this.Link = (this.Links.FirstOrDefault(link => link.Relation == "alternate") ??
            this.Links.FirstOrDefault())?.Href ?? string.Empty;
        this.Summary = item.GetValue("summary");

        // Optional elements of<entry>
        this.Categories = item.GetArray("category", x => x.GetAttributeValue("term"));
        if (item.GetElement("contributor") is ParentTag contributor)
        {
            this.Contributor = new AtomPerson(contributor);
        }
        this.PublishedAsString = item.GetValue("published");
        this.Published = Helpers.TryParseDateTime(this.PublishedAsString);
        this.Rights = item.GetValue("rights");
        this.Source = item.GetValue("source");
    }

    /// <summary>
    /// The "id" element
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// The "updated" element
    /// </summary>
    public string UpdatedAsString { get; }

    /// <summary>
    /// The "updated" element as DateTime. Null if parsing failed or updated is empty
    /// </summary>
    public DateTimeOffset? Updated { get; }

    /// <summary>
    /// The "author" element
    /// </summary>
    public AtomPerson? Author { get; }

    /// <summary>
    /// The "content" element
    /// </summary>
    public string? Content { get; }

    /// <summary>
    /// All "link" elements
    /// </summary>
    public IReadOnlyCollection<AtomLink> Links { get; } = [];

    /// <summary>
    /// The "summary" element
    /// </summary>
    public string? Summary { get; }

    /// <summary>
    /// All "category" elements
    /// </summary>
    public IReadOnlyCollection<string> Categories { get; } = [];

    /// <summary>
    /// The "contributor" element
    /// </summary>
    public AtomPerson? Contributor { get; }

    /// <summary>
    /// The "published" date as string
    /// </summary>
    public string? PublishedAsString { get; }

    /// <summary>
    /// The "published" element as DateTime. Null if parsing failed or published is empty.
    /// </summary>
    public DateTimeOffset? Published { get; }

    /// <summary>
    /// The "rights" element
    /// </summary>
    public string? Rights { get; }

    /// <summary>
    /// The "source" element
    /// </summary>
    public string? Source { get; }

    /// <inheritdoc/>
    internal override FeedItem ToFeedItem()
    {
        return new FeedItem(this)
        {
            Author = this.Author?.ToString(),
            Categories = this.Categories,
            Content = this.Content,
            Description = this.Summary,
            Id = this.Id,
            PublishingDate = this.Published ?? this.Updated,
            PublishingDateString = this.PublishedAsString ?? this.UpdatedAsString ?? string.Empty,
        };
    }
}
