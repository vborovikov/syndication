namespace Syndication.Feeds
{
    using Brackets;

    /// <summary>
    /// Enclosure object of Rss 2.0 according to specification: https://validator.w3.org/feed/docs/rss2.html
    /// </summary>
    public record FeedItemEnclosure
    {
        /// <summary>
        /// The "url" element
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// The "length" element as int
        /// </summary>
        public int? Length { get; }

        /// <summary>
        /// The "type" element
        /// </summary>
        public string MediaType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedItemEnclosure"/> class.
        /// Reads a rss feed item enclosure based on the xml given in element
        /// </summary>
        /// <param name="element">enclosure element as xml</param>
        public FeedItemEnclosure(Tag element)
        {
            this.Url = element.GetAttributeValue("url");
            this.Length = Helpers.TryParseInt(element.GetAttributeValue("length"));
            this.MediaType = element.GetAttributeValue("type");
        }
    }
}
