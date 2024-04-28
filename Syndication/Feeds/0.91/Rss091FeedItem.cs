namespace Syndication.Feeds
{
    using System;
    using Brackets;

    /// <summary>
    /// Rss 0.91 Feed Item according to specification: http://www.rssboard.org/rss-0-9-1-netscape#image
    /// </summary>
    public record Rss091FeedItem : BaseFeedItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rss091FeedItem"/> class.
        /// Creates this object based on the xml in the XElement parameter.
        /// </summary>
        /// <param name="item">feed item as xml</param>
        public Rss091FeedItem(ParentTag item)
            : base(item)
        {
            this.Description = item.GetValue("description");
            this.PubDateAsString = item.GetValue("pubDate");
            this.PubDate = Helpers.TryParseDateTime(this.PubDateAsString);
        }

        /// <summary>
        /// The "description" field
        /// </summary>
        public string? Description { get; } // description

        /// <summary>
        /// The "pubDate" field
        /// </summary>
        public string? PubDateAsString { get; }

        /// <summary>
        /// The "pubDate" field as DateTime. Null if parsing failed or pubDate is empty.
        /// </summary>
        public DateTimeOffset? PubDate { get; }

        internal override FeedItem ToFeedItem()
        {
            return new FeedItem(this)
            {
                Id = this.Link,
                Description = this.Description,
                PublishingDate = this.PubDate,
                PublishingDateString = this.PubDateAsString ?? string.Empty,
            };
        }
    }
}
