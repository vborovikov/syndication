namespace Syndication.Feeds;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Brackets;

/// <summary>
/// RSS 2.0 feed accoring to specification: https://validator.w3.org/feed/docs/rss2.html
/// </summary>
public record Rss20Feed : BaseFeed
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Rss20Feed"/> class.
    /// Reads a rss 2.0 feed based on the xml given in channel
    /// </summary>
    /// <param name="feedXml">the entire feed xml as string</param>
    /// <param name="channel">the "channel" element in the xml as XElement</param>
    public Rss20Feed(string feedXml, ParentTag channel)
        : base(feedXml, channel)
    {
        // Required channel elements
        this.Description = channel.GetRequiredValue("description");

        // Optional channel elements
        this.Language = channel.GetValue("language");
        this.Copyright = channel.GetValue("copyright");
        this.ManagingEditor = channel.GetValue("managingEditor");
        this.WebMaster = channel.GetValue("webMaster");
        this.PubDateAsString = channel.GetValue("pubDate");
        this.LastBuildDateAsString = channel.GetValue("lastBuildDate");
        ParseDates(this.Language, this.PubDateAsString, this.LastBuildDateAsString);
        this.Categories = channel.GetArray("category", x => x.GetRequiredValue());
        this.Generator = channel.GetValue("generator");
        this.Docs = channel.GetValue("docs");
        if (channel.Tag("cloud") is Tag cloud)
        {
            this.Cloud = new FeedCloud(cloud);
        }
        this.TTL = channel.GetValue("ttl");
        if (channel.GetElement("image") is ParentTag image)
        {
            this.Image = new Rss20FeedImage(image);
        }
        if (channel.GetElement("textinput") is ParentTag textInput)
        {
            this.TextInput = new FeedTextInput(textInput);
        }

        if (channel.GetElement("skipHours") is ParentTag skipHours)
        {
            this.SkipHours = skipHours.GetArray("hour", x => x.GetRequiredValue());
        }
        if (channel.GetElement("skipDays") is ParentTag skipDays)
        {
            this.SkipDays = skipDays.GetArray("day", x => x.GetRequiredValue());
        }

        this.Sy = new SyndicationMetadata(channel);

        // Feed items
        this.Items = channel.GetArray("item", item => new Rss20FeedItem((ParentTag)item));
    }

    /// <summary>
    /// The "description" element
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// The "language" element
    /// </summary>
    public string? Language { get; }

    /// <summary>
    /// The "copyright" element
    /// </summary>
    public string? Copyright { get; }

    /// <summary>
    /// The "managingEditor" element
    /// </summary>
    public string? ManagingEditor { get; }

    /// <summary>
    /// The "webMaster" field
    /// </summary>
    public string? WebMaster { get; }

    /// <summary>
    /// The "pubDate" field
    /// </summary>
    public string? PubDateAsString { get; }

    /// <summary>
    /// The "pubDate" field as DateTime. Null if parsing failed or pubDate is empty.
    /// </summary>
    public DateTimeOffset? PubDate { get; private set; }

    /// <summary>
    /// The "lastBuildDate" element as string
    /// </summary>
    public string? LastBuildDateAsString { get; }

    /// <summary>
    /// The "lastBuildDate" element as DateTime. Null if parsing failed of lastBuildDate is empty.
    /// </summary>
    public DateTimeOffset? LastBuildDate { get; private set; }

    /// <summary>
    /// All "category" elements
    /// </summary>
    public IReadOnlyCollection<string> Categories { get; } = []; // category

    /// <summary>
    /// The "generator" element
    /// </summary>
    public string? Generator { get; }

    /// <summary>
    /// The "docs" element
    /// </summary>
    public string? Docs { get; }

    /// <summary>
    /// The "cloud" element
    /// </summary>
    public FeedCloud? Cloud { get; }

    /// <summary>
    /// The time to life "ttl" element
    /// </summary>
    public string? TTL { get; }

    /// <summary>
    /// The "image" element
    /// </summary>
    public FeedImage? Image { get; }

    /// <summary>
    /// The "textInput" element
    /// </summary>
    public FeedTextInput? TextInput { get; }

    /// <summary>
    /// All "day" elements in "skipDays"
    /// </summary>
    public IReadOnlyCollection<string> SkipDays { get; } = [];

    /// <summary>
    /// All "hour" elements in "skipHours"
    /// </summary>
    public IReadOnlyCollection<string> SkipHours { get; } = [];

    /// <summary>
    /// All elements starting with "sy:"
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
            Type = FeedType.Rss_2_0,
            Description = this.Description,
            LastUpdatedDate = this.LastBuildDate,
            LastUpdatedDateString = this.LastBuildDateAsString,
            Copyright = this.Copyright,
            ImageUrl = this.Image?.Url,
            Language = this.Language,
        };
    }

    /// <summary>
    /// Sets the PublishingDate and LastBuildDate. If parsing fails, then it checks if the language
    /// is set and tries to parse the date based on the culture of the language.
    /// </summary>
    /// <param name="language">language of the feed</param>
    /// <param name="publishingDate">publishing date as string</param>
    /// <param name="lastBuildDate">last build date as string</param>
    private void ParseDates(string language, string publishingDate, string lastBuildDate)
    {
        this.PubDate = Helpers.TryParseDateTime(publishingDate);
        this.LastBuildDate = Helpers.TryParseDateTime(lastBuildDate);

        // check if language is set - if so, check if dates could be parsed or try to parse it with culture of the language
        if (string.IsNullOrWhiteSpace(language))
            return;

        // if publishingDateString is set but PublishingDate is null - try to parse with culture of "Language" property
        bool parseLocalizedPublishingDate = this.PubDate == null && !string.IsNullOrWhiteSpace(this.PubDateAsString);

        // if LastBuildDateString is set but LastBuildDate is null - try to parse with culture of "Language" property
        bool parseLocalizedLastBuildDate = this.LastBuildDate == null && !string.IsNullOrWhiteSpace(this.LastBuildDateAsString);

        // if both dates are set - return
        if (!parseLocalizedPublishingDate && !parseLocalizedLastBuildDate)
            return;

        // dates are set, but one of them couldn't be parsed - so try again with the culture set to the language
        CultureInfo culture;
        try
        {
            culture = new CultureInfo(this.Language);

        }
        catch (CultureNotFoundException)
        {
            // should be replace by a try parse or by getting all cultures and selecting the culture
            // out of the collection. That's unfortunately not available in .net standard 1.3 for now
            // this case should never happen, but in some rare cases it does - so catching the exception
            // is acceptable in that case.
            return; // culture couldn't be found, return as it was already tried to parse with default values
        }

        if (parseLocalizedPublishingDate)
        {
            this.PubDate = Helpers.TryParseDateTime(this.PubDateAsString, culture);
        }

        if (parseLocalizedLastBuildDate)
        {
            this.LastBuildDate = Helpers.TryParseDateTime(this.LastBuildDateAsString, culture);
        }
    }
}
