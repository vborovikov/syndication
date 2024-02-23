namespace CodeHollow.FeedReader.Feeds
{
    using Brackets;

    /// <summary>
    /// feed image object that is used in feed (rss 0.91, 2.0, atom, ...)
    /// </summary>
    public record FeedImage
    {
        /// <summary>
        /// The "title" element
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// The "url" element
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// The "link" element
        /// </summary>
        public string Link { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedImage"/> class.
        /// Reads a feed image based on the xml given in element
        /// </summary>
        /// <param name="element">feed image as xml</param>
        public FeedImage(ParentTag element)
        {
            this.Title = element.GetValue("title");
            this.Url = element.GetValue("url");
            this.Link = element.GetValue("link");
        }
    }
}
