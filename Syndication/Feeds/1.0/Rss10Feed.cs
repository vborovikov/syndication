namespace Syndication.Feeds
{
    using Brackets;
    using Syndication;

    /// <summary>
    /// Rss 1.0 Feed according to specification: http://web.resource.org/rss/1.0/spec
    /// </summary>
    public record Rss10Feed : BaseFeed
    {
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
        public FeedImage Image { get; }

        /// <summary>
        /// The "textInput" element
        /// </summary>
        public FeedTextInput TextInput { get; }

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
            this.DC = new DublinCoreMetadata(channel);
            this.Sy = new SyndicationMetadata(channel);
            this.Description = channel.GetValue("description");

            this.Image = new Rss10FeedImage(((ParentTag)channel.Parent).GetElement("image"));
            this.TextInput = new Rss10FeedTextInput(((ParentTag)channel.Parent).GetElement("textinput"));

            var items = ((ParentTag)channel.Parent).GetRoots("item");
            foreach (var item in items)
            {
                this.Items.Add(new Rss10FeedItem(item));
            }
        }

        /// <summary>
        /// Creates the base <see cref="Feed"/> element out of this feed.
        /// </summary>
        /// <returns>feed</returns>
        public override Feed ToFeed()
        {
            Feed f = new Feed(this);

            if (this.DC != null)
            {
                f.Copyright = this.DC.Rights;
                f.Language = this.DC.Language;
                f.LastUpdatedDate = this.DC.Date;
                f.LastUpdatedDateString = this.DC.DateString;
            }

            f.Description = this.Description;
            f.ImageUrl = this.Image?.Url;
            f.Type = FeedType.Rss_1_0;

            return f;
        }
    }
}
