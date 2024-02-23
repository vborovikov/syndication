namespace Syndication.Feeds
{
    using Brackets;

    /// <summary>
    /// item source object from rss 2.0 according to specification: https://validator.w3.org/feed/docs/rss2.html
    /// </summary>
    public record FeedItemSource
    {
        /// <summary>
        /// The "url" element
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// The value of the element
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedItemSource"/> class.
        /// Reads a rss feed item based on the xml given in element
        /// </summary>
        /// <param name="element">item source element as xml</param>
        public FeedItemSource(ParentTag element)
        {
            this.Url = element.GetAttributeValue("url");
            this.Value = element.GetValue();
        }
    }
}
