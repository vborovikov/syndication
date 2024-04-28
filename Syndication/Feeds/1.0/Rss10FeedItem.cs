namespace Syndication.Feeds;

using Brackets;

/// <summary>
/// Rss 1.0 Feed Item according to specification: http://web.resource.org/rss/1.0/spec
/// </summary>
public record Rss10FeedItem : BaseFeedItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Rss10FeedItem"/> class.
    /// Reads a rss 1.0 feed item based on the xml given in item
    /// </summary>
    /// <param name="item">feed item as xml</param>
    public Rss10FeedItem(ParentTag item)
        : base(item)
    {
        this.DC = new DublinCoreMetadata(item);
        this.Sy = new SyndicationMetadata(item);

        this.About = item.GetAttributeValue("rdf:about");
        this.Description = item.GetValue("description");
    }

    /// <summary>
    /// The "about" attribute of the element
    /// </summary>
    public string About { get; }

    /// <summary>
    /// The "description" field
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// All elements starting with "dc:"
    /// </summary>
    public DublinCoreMetadata DC { get; }

    /// <summary>
    /// All elements starting with "sy:"
    /// </summary>
    public SyndicationMetadata Sy { get; }

    /// <inheritdoc/>
    internal override FeedItem ToFeedItem()
    {
        return new FeedItem(this)
        {
            Id = this.Link,
            Description = this.Description,
            Content = this.DC.Description ?? this.Description ?? this.About,
            Author = this.DC.Creator,
            PublishingDate = this.DC.Date,
            PublishingDateString = this.DC.DateString ?? string.Empty,
        };
    }
}
