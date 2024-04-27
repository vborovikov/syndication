﻿namespace Syndication
{
    using Feeds;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Generic feed item object that contains some basic properties. The feed item is typically
    /// an article or a blog post. If a property is not available
    /// for a specific feed type (e.g. Rss 1.0), then the property is empty.
    /// If a feed item has more properties, like the Generator property for Rss 2.0, then you can use
    /// the <see cref="SpecificItem"/> property.
    /// </summary>
    public record FeedItem
    {
        /// <summary>
        /// The title of the feed item
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// The link (url) to the feed item
        /// </summary>
        public string Link { get; }

        /// <summary>
        /// The description of the feed item
        /// </summary>
        public string Description { get; internal init; }

        /// <summary>
        /// The publishing date as string. This is filled, if a publishing
        /// date is set - independent if it is a correct date or not
        /// </summary>
        public string PublishingDateString { get; internal init; }

        /// <summary>
        /// The published date as datetime. Null if parsing failed or if
        /// no publishing date is set. If null, please check <see cref="PublishingDateString"/> property.
        /// </summary>
        public DateTimeOffset? PublishingDate { get; internal init; }

        /// <summary>
        /// The author of the feed item
        /// </summary>
        public string? Author { get; internal init; }

        /// <summary>
        /// The id of the feed item
        /// </summary>
        public string Id { get; internal init; }

        /// <summary>
        /// The categories of the feeditem
        /// </summary>
        public ICollection<string> Categories { get; internal init; }

        /// <summary>
        /// The content of the feed item
        /// </summary>
        public string Content { get; internal init; }

        /// <summary>
        /// The parsed feed item element - e.g. of type <see cref="Rss20FeedItem"/> which contains
        /// e.g. the Enclosure property which does not exist in other feed types.
        /// </summary>
        public BaseFeedItem SpecificItem { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedItem"/> class.
        /// Creates the generic feed item object based on a parsed <see cref="BaseFeedItem"/>
        /// </summary>
        /// <param name="feedItem">BaseFeedItem which is a <see cref="Rss20FeedItem"/> , <see cref="Rss10FeedItem"/>, or another.</param>
        internal FeedItem(BaseFeedItem feedItem)
        {
            this.Title = feedItem.Title;
            this.Link = feedItem.Link;
            this.Categories = Array.Empty<string>();
            this.SpecificItem = feedItem;
        }
    }
}
