﻿namespace CodeHollow.FeedReader.Feeds
{
    using Brackets;

    /// <summary>
    /// Rss 1.0 Feed Item according to specification: http://web.resource.org/rss/1.0/spec
    /// </summary>
    public record Rss10FeedItem : BaseFeedItem
    {
        /// <summary>
        /// The "about" attribute of the element
        /// </summary>
        public string About { get; }

        /// <summary>
        /// The "description" field
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// All elements starting with "dc:"
        /// </summary>
        public DublinCore DC { get; }

        /// <summary>
        /// All elements starting with "sy:"
        /// </summary>
        public Syndication Sy { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rss10FeedItem"/> class.
        /// Reads a rss 1.0 feed item based on the xml given in item
        /// </summary>
        /// <param name="item">feed item as xml</param>
        public Rss10FeedItem(ParentTag item)
            : base(item)
        {
            this.DC = new DublinCore(item);
            this.Sy = new Syndication(item);

            this.About = item.GetAttributeValue("rdf:about");
            this.Description = item.GetValue("description");
        }

        /// <inheritdoc/>
        internal override FeedItem ToFeedItem()
        {
            FeedItem f = new FeedItem(this);

            if (this.DC != null)
            {
                f.Author = this.DC.Creator;
                f.Content = this.DC.Description;
                f.PublishingDate = this.DC.Date;
                f.PublishingDateString = this.DC.DateString;
            }

            f.Description = this.Description;
            if (string.IsNullOrEmpty(f.Content))
                f.Content = this.Description;
            f.Id = this.Link;

            return f;
        }
    }
}
