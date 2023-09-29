namespace CodeHollow.FeedReader.Feeds
{
    using System.Collections.Generic;
    using Brackets;
    using CodeHollow.FeedReader;

    /// <summary>
    /// BaseFeed object which contains the basic properties that each feed has.
    /// </summary>
    public abstract record BaseFeed
    {
        /// <summary>
        /// creates the generic <see cref="Feed"/> object.
        /// </summary>
        /// <returns>Feed</returns>
        public abstract Feed ToFeed();

        /// <summary>
        /// The title of the feed
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// The link (url) to the feed
        /// </summary>
        public string Link { get; protected set; }

        /// <summary>
        /// The items that are in the feed
        /// </summary>
        public ICollection<BaseFeedItem> Items { get; }

        /// <summary>
        /// Gets the whole, original feed as string
        /// </summary>
        public string OriginalDocument { get; }

        /// <summary>
        /// Gets the underlying XElement in order to allow reading properties that are not available in the class itself
        /// </summary>
        public ParentTag Element { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFeed"/> class.
        /// default constructor (for serialization)
        /// </summary>
        protected BaseFeed()
        {
            this.Items = new List<BaseFeedItem>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFeed"/> class.
        /// Reads a base feed based on the xml given in element
        /// </summary>
        /// <param name="feedXml">the entire feed xml as string</param>
        /// <param name="channel">the "channel" element in the xml as XElement</param>
        protected BaseFeed(string feedXml, ParentTag channel)
            : this()
        {
            this.OriginalDocument = feedXml;

            this.Title = channel.GetValue("title");
            this.Link = channel.GetValue("link");
            this.Element = channel;
        }
    }
}
