namespace Syndication
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Brackets;
    using Feeds;
    using Parser;

    /// <summary>
    /// Generic Feed object that contains some basic properties. If a property is not available
    /// for a specific feed type (e.g. Rss 1.0), then the property is empty.
    /// If a feed has more properties, like the Generator property for Rss 2.0, then you can use
    /// the <see cref="SpecificFeed"/> property.
    /// </summary>
    public record Feed
    {
        /// <summary>
        /// The Type of the feed - Rss 2.0, 1.0, 0.92, Atom or others
        /// </summary>
        public FeedType Type { get; internal set; }

        /// <summary>
        /// The title of the field
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// The link (url) to the feed
        /// </summary>
        public string Link { get; }

        /// <summary>
        /// The description of the feed
        /// </summary>
        public string Description { get; internal set; }

        /// <summary>
        /// The language of the feed
        /// </summary>
        public string Language { get; internal set; }

        /// <summary>
        /// The copyright of the feed
        /// </summary>
        public string Copyright { get; internal set; }

        /// <summary>
        /// The last updated date as string. This is filled, if a last updated
        /// date is set - independent if it is a correct date or not
        /// </summary>
        public string LastUpdatedDateString { get; internal set; }

        /// <summary>
        /// The last updated date as datetime. Null if parsing failed or if
        /// no last updated date is set. If null, please check <see cref="LastUpdatedDateString"/> property.
        /// </summary>
        public DateTime? LastUpdatedDate { get; internal set; }

        /// <summary>
        /// The url of the image
        /// </summary>
        public string ImageUrl { get; internal set; }

        /// <summary>
        /// List of items
        /// </summary>
        public IList<FeedItem> Items { get; }

        /// <summary>
        /// Gets the whole, original feed as string
        /// </summary>
        public string OriginalDocument => this.SpecificFeed.OriginalDocument;

        /// <summary>
        /// The parsed feed element - e.g. of type <see cref="Rss20Feed"/> which contains
        /// e.g. the Generator property which does not exist in others.
        /// </summary>
        public BaseFeed SpecificFeed { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Feed"/> class.
        /// Creates the generic feed object based on a parsed BaseFeed
        /// </summary>
        /// <param name="feed">BaseFeed which is a <see cref="Rss20Feed"/> , <see cref="Rss10Feed"/>, or another.</param>
        internal Feed(BaseFeed feed)
        {
            this.SpecificFeed = feed;

            this.Title = feed.Title;
            this.Link = feed.Link;

            this.Items = feed.Items.Select(x => x.ToFeedItem()).ToArray();
        }

        /// <summary>
        /// Creates a <see cref="Feed"/> from a string.
        /// </summary>
        /// <param name="content">The string content of the feed.</param>
        /// <returns>The <see cref="Feed"/> instance.</returns>
        public static Feed FromString(string content)
        {
            var document = Document.Xml.Parse(content);
            return GetFeed(document, content);
        }

        /// <summary>
        /// Creates a <see cref="Feed"/> from a stream.
        /// </summary>
        /// <param name="stream">The stream of the feed.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task<Feed> FromStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            var document = await Document.Xml.ParseAsync(stream, cancellationToken).ConfigureAwait(false);
            return GetFeed(document, string.Empty);
        }

        /// <summary>
        /// Creates a <see cref="Feed"/> from a <see cref="Document">document</see>.
        /// </summary>
        /// <param name="document">The document object.</param>
        /// <returns>The <see cref="Feed"/> instance.</returns>
        public static Feed FromDocument(Document document) => GetFeed(document, string.Empty);

        private static Feed GetFeed(Document document, string content)
        {
            if (FeedParser.TryParseFeedType(document, out var feedType))
            {
                var parser = Factory.GetParser(feedType);
                var feed = parser.Parse(document, content);

                return feed.ToFeed();
            }

            if (document.Root()?.Name?.Equals("html", StringComparison.OrdinalIgnoreCase) == true)
            {
                var feedUrls = Helpers.ParseFeedUrls(document);
                throw new HtmlContentDetectedException("HTML content was detected.", feedUrls);
            }

            throw new FeedTypeNotSupportedException($"Unknown feed type '{document.Root()?.Name}'.");
        }
    }
}
