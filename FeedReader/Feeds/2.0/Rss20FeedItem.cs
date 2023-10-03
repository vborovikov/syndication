﻿namespace CodeHollow.FeedReader.Feeds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Brackets;

    /// <summary>
    /// RSS 2.0 feed item accoring to specification: https://validator.w3.org/feed/docs/rss2.html
    /// </summary>
    public record Rss20FeedItem : BaseFeedItem
    {
        /// <summary>
        /// The "description" field of the feed item
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The "author" field of the feed item
        /// </summary>
        public string Author { get; }

        /// <summary>
        /// The "comments" field of the feed item
        /// </summary>
        public string Comments { get; }

        /// <summary>
        /// The "enclosure" field
        /// </summary>
        public FeedItemEnclosure Enclosure { get; }

        /// <summary>
        /// The "guid" field
        /// </summary>
        public string Guid { get; }

        /// <summary>
        /// The "pubDate" field
        /// </summary>
        public string PublishingDateString { get; }

        /// <summary>
        /// The "pubDate" field as DateTime. Null if parsing failed or pubDate is empty.
        /// </summary>
        public DateTime? PublishingDate { get; }

        /// <summary>
        /// The "source" field
        /// </summary>
        public FeedItemSource Source { get; }

        /// <summary>
        /// All entries "category" entries
        /// </summary>
        public ICollection<string> Categories { get; }

        /// <summary>
        /// The "content:encoded" field
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// All elements starting with "dc:"
        /// </summary>
        public DublinCore DC { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rss20FeedItem"/> class.
        /// Reads a new feed item element based on the given xml item
        /// </summary>
        /// <param name="item">the xml containing the feed item</param>
        public Rss20FeedItem(ParentTag item)
            : base(item)
        {
            this.Comments = item.GetValue("comments");
            this.Author = item.GetValue("author");
            this.Enclosure = new FeedItemEnclosure(item.GetElement("enclosure"));
            this.PublishingDateString = item.GetValue("pubDate");
            this.PublishingDate = Helpers.TryParseDateTime(this.PublishingDateString);
            this.DC = new DublinCore(item);
            this.Source = new FeedItemSource(item.GetElement("source"));

            var categories = item.GetElements("category");
            this.Categories = categories.Select(x => x.GetValue()).ToList();

            this.Guid = item.GetValue("guid");
            this.Description = item.GetValue("description");
            this.Content = item.GetValue("content:encoded")?.HtmlDecode();
        }

        /// <inheritdoc/>
        internal override FeedItem ToFeedItem()
        {
            var fi = new FeedItem(this)
            {
                Author = this.Author ?? this.DC?.Creator,
                Categories = this.Categories,
                Content = this.Content,
                Description = this.Description,
                Id = this.Guid,
                PublishingDate = this.PublishingDate,
                PublishingDateString = this.PublishingDateString
            };
            return fi;
        }
    }
}
