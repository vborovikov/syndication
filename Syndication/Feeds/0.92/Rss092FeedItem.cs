namespace Syndication.Feeds
{
    using System.Collections.Generic;
    using System.Linq;
    using Brackets;

    /// <summary>
    /// Rss 0.92 feed item according to specification: http://backend.userland.com/rss092
    /// </summary>
    public record Rss092FeedItem : Rss091FeedItem
    {
        /// <summary>
        /// All entries "category" entries
        /// </summary>
        public ICollection<string> Categories { get; }

        /// <summary>
        /// The "enclosure" field
        /// </summary>
        public FeedItemEnclosure Enclosure { get; }

        /// <summary>
        /// The "source" field
        /// </summary>
        public FeedItemSource Source { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rss092FeedItem"/> class.
        /// Creates a new feed item element based on the given xml XELement
        /// </summary>
        /// <param name="item">the xml containing the feed item</param>
        public Rss092FeedItem(ParentTag item)
            : base(item)
        {
            this.Enclosure = new FeedItemEnclosure(item.Tag("enclosure"));
            this.Source = new FeedItemSource(item.Root("source"));

            var categories = item.GetElements("category");
            this.Categories = categories.Select(x => x.GetValue()).ToArray();
        }
    }
}
