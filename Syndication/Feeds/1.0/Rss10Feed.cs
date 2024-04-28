namespace Syndication.Feeds;

using Brackets;

/// <summary>
/// Rss 1.0 Feed according to specification: http://web.resource.org/rss/1.0/spec
/// </summary>
public record Rss10Feed : BaseFeed
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Rss10Feed"/> class.
    /// Reads a rss 1.0 feed based on the xml given in xelement
    /// </summary>
    /// <param name="feedXml">the entire feed xml as string</param>
    /// <param name="channel">the "channel" element in the xml as XElement</param>
    public Rss10Feed(string feedXml, ParentTag channel)
        : base(feedXml, channel)
    {
        this.About = channel.GetAttributeValue("rdf:about");
        this.Description = channel.GetRequiredValue("description");
        if (channel.Parent!.GetElement("image") is ParentTag image)
        {
            this.Image = new Rss10FeedImage(image);
        }
        if (channel.Parent!.GetElement("textinput") is ParentTag textInput)
        {
            this.TextInput = new Rss10FeedTextInput(textInput);
        }

        this.DC = new DublinCoreMetadata(channel);
        this.Sy = new SyndicationMetadata(channel);

        this.Items = channel.Parent!.GetArray("item", item => new Rss10FeedItem((ParentTag)item));
    }

    /// <summary>
    /// The "about" attribute of the element
    /// </summary>
    public string About { get; }

    /// <summary>
    /// All elements starting with "dc:"
    /// </summary>
    public DublinCoreMetadata DC { get; }

    /// <summary>
    /// All elements starting with "sy:"
    /// </summary>
    public SyndicationMetadata Sy { get; }

    /// <summary>
    /// The "description" field
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// The "image" element
    /// </summary>
    public FeedImage? Image { get; }

    /// <summary>
    /// The "textInput" element
    /// </summary>
    public FeedTextInput? TextInput { get; }

    /// <summary>
    /// Creates the base <see cref="Feed"/> element out of this feed.
    /// </summary>
    /// <returns>feed</returns>
    public override Feed ToFeed()
    {
        return new Feed(this)
        {
            Description = this.Description,
            ImageUrl = this.Image?.Url,
            Type = FeedType.Rss_1_0,
            Copyright = this.DC.Rights,
            Language = this.DC.Language,
            LastUpdatedDate = this.DC.Date,
            LastUpdatedDateString = this.DC.DateString,
        };
    }
}
