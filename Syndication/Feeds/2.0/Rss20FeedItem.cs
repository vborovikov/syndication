namespace Syndication.Feeds
{
    using System;
    using System.Collections.Generic;
    using Brackets;

    /// <summary>
    /// RSS 2.0 feed item accoring to specification: https://validator.w3.org/feed/docs/rss2.html
    /// </summary>
    public record Rss20FeedItem : BaseFeedItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rss20FeedItem"/> class.
        /// Reads a new feed item element based on the given xml item
        /// </summary>
        /// <param name="item">the xml containing the feed item</param>
        public Rss20FeedItem(ParentTag item)
            : base(item)
        {
            this.Description = item.GetValue("description");
            
            if (item.GetElement("author", ignoreNamespace: true) is ParentTag author)
            {
                this.Author = author.GetValue("name", ignoreNamespace: true);
            }
            if (string.IsNullOrEmpty(this.Author))
            {
                this.Author = item.GetValue("author");
            }

            this.Categories = item.GetArray("category", x => x.GetRequiredValue());
            this.Comments = item.GetValue("comments");
            if (item.GetElement("enclosure") is Tag enclosure)
            {
                this.Enclosure = new FeedItemEnclosure(enclosure);
            }
            this.Guid = item.GetValue("guid");
            this.PubDateAsString = item.GetValue("pubDate") ?? item.GetValue("updated", ignoreNamespace: true);
            this.PubDate = Helpers.TryParseDateTime(this.PubDateAsString);
            if (item.GetElement("source") is ParentTag source)
            {
                this.Source = new FeedItemSource(source);
            }
            this.DC = new DublinCoreMetadata(item);
            this.Content = item.GetValue("content:encoded");
        }

        /// <summary>
        /// The "description" field of the feed item
        /// </summary>
        public string? Description { get; }

        /// <summary>
        /// The "author" field of the feed item
        /// </summary>
        public string? Author { get; }

        /// <summary>
        /// The "comments" field of the feed item
        /// </summary>
        public string? Comments { get; }

        /// <summary>
        /// The "enclosure" field
        /// </summary>
        public FeedItemEnclosure? Enclosure { get; }

        /// <summary>
        /// The "guid" field
        /// </summary>
        public string? Guid { get; }

        /// <summary>
        /// The "pubDate" field
        /// </summary>
        public string? PubDateAsString { get; }

        /// <summary>
        /// The "pubDate" field as DateTime. Null if parsing failed or pubDate is empty.
        /// </summary>
        public DateTimeOffset? PubDate { get; }

        /// <summary>
        /// The "source" field
        /// </summary>
        public FeedItemSource? Source { get; }

        /// <summary>
        /// All entries "category" entries
        /// </summary>
        public IReadOnlyCollection<string> Categories { get; } = [];

        /// <summary>
        /// The "content:encoded" field
        /// </summary>
        public string? Content { get; }

        /// <summary>
        /// All elements starting with "dc:"
        /// </summary>
        public DublinCoreMetadata DC { get; }

        /// <inheritdoc/>
        internal override FeedItem ToFeedItem()
        {
            return new FeedItem(this)
            {
                Id = this.Guid ?? string.Empty,
                Description = this.Description,
                Content = this.Content,
                Author = this.Author ?? this.DC?.Creator,
                Categories = this.Categories,
                PublishingDate = this.PubDate,
                PublishingDateString = this.PubDateAsString ?? string.Empty,
            };
        }
    }
}
