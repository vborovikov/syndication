namespace Syndication.Feeds;

using System;
using System.Collections.Generic;
using Brackets;

/// <summary>
/// Rss Feed according to Rss 0.91 specification:
/// http://www.rssboard.org/rss-0-9-1-netscape
/// </summary>
public record Rss091Feed : BaseFeed
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Rss091Feed"/> class.
    /// Reads a rss 0.91 feed based on the xml given in channel
    /// </summary>
    /// <param name="feedXml">the entire feed xml as string</param>
    /// <param name="channel">the "channel" element in the xml as XElement</param>
    public Rss091Feed(string feedXml, ParentTag channel)
        : base(feedXml, channel)
    {
        // Required elements
        this.Description = channel.GetRequiredValue("description");
        this.Language = channel.GetValue("language") ?? string.Empty;

        // Optional elements
        this.Copyright = channel.GetValue("copyright");
        this.Docs = channel.GetValue("docs");
        if (channel.GetElement("image") is ParentTag image)
        {
            this.Image = new Rss091FeedImage(image);
        }
        this.LastBuildDateString = channel.GetValue("lastBuildDate");
        this.LastBuildDate = Helpers.TryParseDateTime(this.LastBuildDateString);
        this.ManagingEditor = channel.GetValue("managingEditor");
        this.PublishingDateString = channel.GetValue("pubDate");
        this.PublishingDate = Helpers.TryParseDateTime(this.PublishingDateString);
        this.Rating = channel.GetValue("rating");
        if (channel.GetElement("skipHours") is ParentTag skipHours)
        {
            this.SkipHours = skipHours.GetArray("hour", x => x.GetRequiredValue());
        }
        if (channel.GetElement("skipDays") is ParentTag skipDays)
        {
            this.SkipDays = skipDays.GetArray("day", x => x.GetRequiredValue());
        }
        if (channel.GetElement("textinput") is ParentTag textInput)
        {
            this.TextInput = new FeedTextInput(textInput);
        }
        this.WebMaster = channel.GetValue("webMaster");

        this.Sy = new SyndicationMetadata(channel);

        this.Items = channel.GetArray("item", item => CreateItem((ParentTag)item));
    }

    /// <summary>
    /// The required field "description"
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// The required field "language"
    /// /// </summary>
    public string Language { get; }

    /// <summary>
    /// The "copyright" field
    /// </summary>
    public string? Copyright { get; }

    /// <summary>
    /// The "docs" field
    /// </summary>
    public string? Docs { get; }

    /// <summary>
    /// The "image" element
    /// </summary>
    public FeedImage? Image { get; }

    /// <summary>
    /// The "lastBuildDate" element
    /// </summary>
    public string? LastBuildDateString { get; }

    /// <summary>
    /// The "lastBuildDate" as DateTime. Null if parsing failed or lastBuildDate is empty.
    /// </summary>
    public DateTimeOffset? LastBuildDate { get; }

    /// <summary>
    /// The "managingEditor" field
    /// </summary>
    public string? ManagingEditor { get; }

    /// <summary>
    /// The "pubDate" field
    /// </summary>
    public string? PublishingDateString { get; }

    /// <summary>
    /// The "pubDate" field as DateTime. Null if parsing failed or pubDate is empty.
    /// </summary>
    public DateTimeOffset? PublishingDate { get; }

    /// <summary>
    /// The "rating" field
    /// </summary>
    public string? Rating { get; }

    /// <summary>
    /// All "day" elements in "skipDays"
    /// </summary>
    public IReadOnlyCollection<string> SkipDays { get; } = [];

    /// <summary>
    /// All "hour" elements in "skipHours"
    /// </summary>
    public IReadOnlyCollection<string> SkipHours { get; } = [];

    /// <summary>
    /// The "textInput" element
    /// </summary>
    public FeedTextInput? TextInput { get; }

    /// <summary>
    /// The "webMaster" element
    /// </summary>
    public string? WebMaster { get; }

    /// <summary>
    /// All elements that start with "sy:"
    /// </summary>
    public SyndicationMetadata Sy { get; }

    /// <summary>
    /// Creates the base <see cref="Feed"/> element out of this feed.
    /// </summary>
    /// <returns>feed</returns>
    public override Feed ToFeed()
    {
        return new Feed(this)
        {
            Type = FeedType.Rss_0_91,
            Description = this.Description,
            Language = this.Language,
            Copyright = this.Copyright,
            ImageUrl = this.Image?.Url,
            LastUpdatedDate = this.LastBuildDate,
            LastUpdatedDateString = this.LastBuildDateString,
        };
    }

    private protected virtual BaseFeedItem CreateItem(ParentTag item) => new Rss091FeedItem(item);
}
