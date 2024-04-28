namespace Syndication.Feeds
{
    using System.Collections.Generic;
    using Brackets;

    /// <summary>
    /// Rss 0.92 feed item according to specification: http://backend.userland.com/rss092
    /// </summary>
    public record Rss092FeedItem : Rss091FeedItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rss092FeedItem"/> class.
        /// Creates a new feed item element based on the given xml XELement
        /// </summary>
        /// <param name="item">the xml containing the feed item</param>
        public Rss092FeedItem(ParentTag item)
            : base(item)
        {
            if (item.Tag("enclosure") is Tag enclosure)
            {
                this.Enclosure = new FeedItemEnclosure(enclosure);
            }
            if (item.GetElement("source") is ParentTag source)
            {
                this.Source = new FeedItemSource(source);
            }

            this.Categories = item.GetArray("category", x => x.GetRequiredValue());
        }

        /// <summary>
        /// All entries "category" entries
        /// </summary>
        public IReadOnlyCollection<string> Categories { get; } = [];

        /// <summary>
        /// The "enclosure" field
        /// </summary>
        public FeedItemEnclosure? Enclosure { get; }

        /// <summary>
        /// The "source" field
        /// </summary>
        public FeedItemSource? Source { get; }
    }
}
