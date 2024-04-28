namespace Syndication.Feeds;

using System;
using System.Collections.Generic;
using System.Linq;
using Brackets;

/// <summary>
/// Atom 1.0 feed object according to specification: https://validator.w3.org/feed/docs/atom.html
/// </summary>
public record AtomFeed : BaseFeed
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AtomFeed"/> class.
    /// Reads an atom feed based on the xml given in channel
    /// </summary>
    /// <param name="feedXml">the entire feed xml as string</param>
    /// <param name="feed">the feed element in the xml as XElement</param>
    public AtomFeed(string feedXml, ParentTag feed)
        : base(feedXml, feed)
    {
        // Required feed elements
        this.Id = feed.GetRequiredValue("id");
        this.UpdatedAsString = feed.GetValue("updated") ?? feed.GetValue("published") ?? string.Empty;
        this.Updated = Helpers.TryParseDateTime(this.UpdatedAsString);

        // Recommended feed elements
        if (feed.GetElement("author") is ParentTag author)
        {
            this.Author = new AtomPerson(author);
        }
        this.Links = feed.GetArray("link", link => new AtomLink(link));
        this.Link = (this.Links.FirstOrDefault(link => link.Relation == "self") ??
            this.Links.FirstOrDefault())?.Href ?? string.Empty;

        // Optional feed elements
        this.Categories = feed.GetArray("category", x => x.GetRequiredValue());
        if (feed.GetElement("contributor") is ParentTag contributor)
        {
            this.Contributor = new AtomPerson(contributor);
        }
        this.Generator = feed.GetValue("generator");
        this.Icon = feed.GetValue("icon");
        this.Logo = feed.GetValue("logo");
        this.Rights = feed.GetValue("rights");
        this.Subtitle = feed.GetValue("subtitle");

        // Feed items
        this.Items = feed.GetArray("entry", entry => new AtomFeedItem((ParentTag)entry));
    }

    /// <summary>
    /// The "id" element
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// The "updated" element as string
    /// </summary>
    public string UpdatedAsString { get; }

    /// <summary>
    /// The "updated" element as DateTime. Null if parsing failed of updatedDate is empty.
    /// </summary>
    public DateTimeOffset? Updated { get; }

    /// <summary>
    /// The "author" element
    /// </summary>
    public AtomPerson? Author { get; }

    /// <summary>
    /// All "category" elements
    /// </summary>
    public IReadOnlyCollection<string> Categories { get; } = [];

    /// <summary>
    /// The "contributor" element
    /// </summary>
    public AtomPerson? Contributor { get; }

    /// <summary>
    /// The "generator" element
    /// </summary>
    public string? Generator { get; }

    /// <summary>
    /// The "icon" element
    /// </summary>
    public string? Icon { get; }

    /// <summary>
    /// The "logo" element
    /// </summary>
    public string? Logo { get; }

    /// <summary>
    /// The "rights" element
    /// </summary>
    public string? Rights { get; }

    /// <summary>
    /// The "subtitle" element
    /// </summary>
    public string? Subtitle { get; }

    /// <summary>
    /// All "link" elements
    /// </summary>
    public IReadOnlyCollection<AtomLink> Links { get; } = [];

    /// <summary>
    /// Creates the base <see cref="Feed"/> element out of this feed.
    /// </summary>
    /// <returns>feed</returns>
    public override Feed ToFeed()
    {
        return new Feed(this)
        {
            Type = FeedType.Atom,
            Description = this.Subtitle,
            LastUpdatedDate = this.Updated,
            LastUpdatedDateString = this.UpdatedAsString,
            Copyright = this.Rights,
            ImageUrl = this.Icon,
            Language = null,
        };
    }
}
