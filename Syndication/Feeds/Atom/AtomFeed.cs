namespace Syndication.Feeds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Brackets;

    /// <summary>
    /// Atom 1.0 feed object according to specification: https://validator.w3.org/feed/docs/atom.html
    /// </summary>
    public record AtomFeed : BaseFeed
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
        /// The "contributor" element
        /// </summary>
        public AtomPerson Contributor { get; }

        /// <summary>
        /// The "generator" element
        /// </summary>
        public string Generator { get; }

        /// <summary>
        /// The "icon" element
        /// </summary>
        public string Icon { get; }

        /// <summary>
        /// The "id" element
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The "logo" element
        /// </summary>
        public string Logo { get; }

        /// <summary>
        /// The "rights" element
        /// </summary>
        public string Rights { get; }

        /// <summary>
        /// The "subtitle" element
        /// </summary>
        public string Subtitle { get; }

        /// <summary>
        /// The "updated" element as string
        /// </summary>
        public string UpdatedDateString { get; }

        /// <summary>
        /// The "updated" element as DateTime. Null if parsing failed of updatedDate is empty.
        /// </summary>
        public DateTime? UpdatedDate { get; }

        /// <summary>
        /// All "link" elements
        /// </summary>
        public ICollection<AtomLink> Links { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomFeed"/> class.
        /// default constructor (for serialization)
        /// </summary>
        public AtomFeed()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomFeed"/> class.
        /// Reads an atom feed based on the xml given in channel
        /// </summary>
        /// <param name="feedXml">the entire feed xml as string</param>
        /// <param name="feed">the feed element in the xml as XElement</param>
        public AtomFeed(string feedXml, ParentTag feed)
            : base(feedXml, feed)
        {
            this.Link = feed.Tag("link")?.GetAttributeValue("href");

            this.Author = new AtomPerson(feed.GetElement("author"));

            var categories = feed.GetElements("category");
            this.Categories = categories.Select(x => x.GetValue()).ToArray();

            this.Contributor = new AtomPerson(feed.GetElement("contributor"));
            this.Generator = feed.GetValue("generator");
            this.Icon = feed.GetValue("icon");
            this.Id = feed.GetValue("id");
            this.Logo = feed.GetValue("logo");
            this.Rights = feed.GetValue("rights");
            this.Subtitle = feed.GetValue("subtitle");

            this.Links = feed.GetElements("link").Select(x => new AtomLink(x)).ToArray();

            this.UpdatedDateString = feed.GetValue("updated");
            this.UpdatedDate = Helpers.TryParseDateTime(this.UpdatedDateString);

            var items = feed.GetRoots("entry");

            foreach (var item in items)
            {
                this.Items.Add(new AtomFeedItem(item));
            }
        }

        /// <summary>
        /// Creates the base <see cref="Feed"/> element out of this feed.
        /// </summary>
        /// <returns>feed</returns>
        public override Feed ToFeed()
        {
            Feed f = new Feed(this)
            {
                Copyright = this.Rights,
                Description = null,
                ImageUrl = this.Icon,
                Language = null,
                LastUpdatedDate = this.UpdatedDate,
                LastUpdatedDateString = this.UpdatedDateString,
                Type = FeedType.Atom
            };
            return f;
        }
    }
}
