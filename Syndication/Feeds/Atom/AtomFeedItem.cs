namespace Syndication.Feeds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Brackets;

    /// <summary>
    /// Atom 1.0 feed item object according to specification: https://validator.w3.org/feed/docs/atom.html
    /// </summary>
    public record AtomFeedItem : BaseFeedItem
    {
        /// <summary>
        /// The "author" element
        /// </summary>
        public AtomPerson Author { get; }

        /// <summary>
        /// All "category" elements
        /// </summary>
        public ICollection<string> Categories { get; }

        /// <summary>
        /// The "content" element
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// The "contributor" element
        /// </summary>
        public AtomPerson Contributor { get; }

        /// <summary>
        /// The "id" element
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The "published" date as string
        /// </summary>
        public string PublishedDateString { get; }

        /// <summary>
        /// The "published" element as DateTime. Null if parsing failed or published is empty.
        /// </summary>
        public DateTimeOffset? PublishedDate { get; }

        /// <summary>
        /// The "rights" element
        /// </summary>
        public string Rights { get; }

        /// <summary>
        /// The "source" element
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// The "summary" element
        /// </summary>
        public string Summary { get; }

        /// <summary>
        /// The "updated" element
        /// </summary>
        public string UpdatedDateString { get; }

        /// <summary>
        /// The "updated" element as DateTime. Null if parsing failed or updated is empty
        /// </summary>
        public DateTimeOffset? UpdatedDate { get; }

        /// <summary>
        /// All "link" elements
        /// </summary>
        public ICollection<AtomLink> Links { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomFeedItem"/> class.
        /// Reads an atom feed based on the xml given in item
        /// </summary>
        /// <param name="item">feed item as xml</param>
        public AtomFeedItem(ParentTag item)
            : base(item)
        {
            this.Link = item.Tag("link")?.GetAttributeValue("href");

            this.Author = new AtomPerson(item.GetElement("author"));

            var categories = item.GetElements("category");
            this.Categories = categories.Select(x => x.GetAttributeValue("term")).ToArray();

            this.Content = item.GetValue("content");
            this.Contributor = new AtomPerson(item.GetElement("contributor"));
            this.Id = item.GetValue("id");

            this.PublishedDateString = item.GetValue("published");
            this.PublishedDate = Helpers.TryParseDateTime(this.PublishedDateString);
            this.Links = item.GetElements("link").Select(x => new AtomLink(x)).ToArray();

            this.Rights = item.GetValue("rights");
            this.Source = item.GetValue("source");
            this.Summary = item.GetValue("summary");

            this.UpdatedDateString = item.GetValue("updated");
            this.UpdatedDate = Helpers.TryParseDateTime(this.UpdatedDateString);
        }

        /// <inheritdoc/>
        internal override FeedItem ToFeedItem()
        {
            FeedItem fi = new FeedItem(this)
            {
                Author = this.Author?.ToString(),
                Categories = this.Categories,
                Content = this.Content,
                Description = this.Summary,
                Id = this.Id,
                PublishingDate = this.PublishedDate,
                PublishingDateString = this.PublishedDateString
            };
            return fi;
        }
    }
}
