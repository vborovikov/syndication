namespace Syndication.Feeds
{
    using Brackets;

    /// <summary>
    /// The base object for all feed items
    /// </summary>
    public abstract record BaseFeedItem
    {
        /// <summary>
        /// The "title" element
        /// </summary>
        public string Title { get; } // title

        /// <summary>
        /// The "link" element
        /// </summary>
        public string Link { get; protected set; } // link

        /// <summary>
        /// Gets the underlying XElement in order to allow reading properties that are not available in the class itself
        /// </summary>
        public ParentTag Element { get; }

        internal abstract FeedItem ToFeedItem();

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFeedItem"/> class.
        /// Reads a base feed item based on the xml given in element
        /// </summary>
        /// <param name="item">feed item as xml</param>
        protected BaseFeedItem(ParentTag item)
        {
            this.Title = item.GetValue("title");
            this.Link = item.GetValue("link");
            this.Element = item;
        }
    }
}
