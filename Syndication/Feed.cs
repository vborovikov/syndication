namespace Syndication
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
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
        public string? Description { get; internal set; }

        /// <summary>
        /// The language of the feed
        /// </summary>
        public string? Language { get; internal set; }

        /// <summary>
        /// The copyright of the feed
        /// </summary>
        public string? Copyright { get; internal set; }

        /// <summary>
        /// The last updated date as string. This is filled, if a last updated
        /// date is set - independent if it is a correct date or not
        /// </summary>
        public string LastUpdatedDateString { get; internal set; }

        /// <summary>
        /// The last updated date as datetime. Null if parsing failed or if
        /// no last updated date is set. If null, please check <see cref="LastUpdatedDateString"/> property.
        /// </summary>
        public DateTimeOffset? LastUpdatedDate { get; internal set; }

        /// <summary>
        /// The url of the image
        /// </summary>
        public string? ImageUrl { get; internal set; }

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
        /// Creates a <see cref="Feed"/> from a <see cref="ReadOnlySpan{T}">span of bytes</see>.
        /// </summary>
        /// <param name="span">The span of bytes.</param>
        /// <returns>The <see cref="Feed"/> instance.</returns>
        public static Feed FromSpan(ReadOnlySpan<byte> span)
        {
            var sample = span;
            if (sample.Length > 128)
                sample = sample[..128];

            var encoding = Encoding.UTF8;
            var sampleStr = encoding.GetString(sample);
            var sampleDoc = Document.Xml.Parse(sampleStr);
            if (TryGetEncoding(sampleDoc, out var docEncoding))
            {
                encoding = docEncoding;
            }

            var content = encoding.GetString(span);
            var document = Document.Xml.Parse(content);
            return GetFeed(document, content);
        }

        private static bool TryGetEncoding(Document document, [MaybeNullWhen(false)]out Encoding encoding)
        {
            if (document.FirstOrDefault<Instruction>() is { Name: "xml", HasAttributes: true } xmlInstruction &&
                    xmlInstruction.Attributes["encoding"] is { Length: > 0 } xmlEncoding)
            {
                try
                {
                    encoding = Encoding.GetEncoding(xmlEncoding.ToString());
                    return true;
                }
                catch (Exception)
                {

                }
            }

            encoding = default;
            return false;
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
        /// Finds all feed links in the <see cref="Document">document</see>.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>An array of <see cref="HtmlFeedLink"/> instances.</returns>
        public static HtmlFeedLink[] FindAll(Document document)
        {
            const StringComparison cmp = StringComparison.OrdinalIgnoreCase;

            if (document.FirstOrDefault<ParentTag>(t => t.Name == "html") is ParentTag html &&
                html.FirstOrDefault<ParentTag>(t => t.Name == "head") is ParentTag head)
            {
                var links = head.FindAll<Tag>(t => t is { Name: "link", HasAttributes: true } &&
                    t.Attributes.Has("rel", "alternate") && t.Attributes.Has("href") &&
                    (t.Attributes.Has("type", "application/rss") || t.Attributes.Has("type", "application/atom")));

                var found = new List<HtmlFeedLink>();
                foreach (var link in links)
                {
                    found.Add(new(
                        WebUtility.HtmlDecode(link.Attributes["title"].ToString()),
                        link.Attributes["href"].ToString(),
                        link.Attributes["type"] switch
                        {
                            var type when type.StartsWith("application/rss", cmp) => FeedType.Rss,
                            var type when type.StartsWith("application/atom", cmp) => FeedType.Atom,
                            _ => FeedType.Unknown,
                        }));
                }

                if (found.Count == 0 && html.FirstOrDefault<ParentTag>(t => t.Name == "body") is ParentTag body)
                {
                    var anchors = body.FindAll<ParentTag>(t => t is { Name: "a", HasAttributes: true } &&
                        t.Attributes["href"] is { Length: > 0 } href && (
                        (href.EndsWith("feed", cmp) || href.EndsWith("feed/", cmp) || 
                        href.EndsWith(".xml", cmp) || href.EndsWith(".xml/", cmp) || 
                        href.EndsWith("rss", cmp) || href.EndsWith("rss/", cmp) || 
                        href.EndsWith("atom", cmp) || href.EndsWith("atom/", cmp)) ||
                        ContainsText(t, "rss")));

                    foreach (var anchor in anchors)
                    {
                        var href = anchor.Attributes["href"].ToString();
                        var feedType = FeedType.Unknown;
                        if (href.Contains("rss", cmp) || ContainsText(anchor, "rss"))
                        {
                            feedType = FeedType.Rss;
                        }
                        else if (href.Contains("atom", cmp) || ContainsText(anchor, "atom"))
                        {
                            feedType = FeedType.Atom;
                        }

                        found.Add(new(WebUtility.HtmlDecode(anchor.ToString()), href, feedType));
                    }
                }

                return [..found];
            }

            return [];
        }

        private static bool ContainsText(ParentTag tag, ReadOnlySpan<char> text)
        {
            foreach (var content in tag.FindAll<Content>(c => true))
            {
                if (content.Contains(text))
                    return true;
            }

            return false;
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

            var documentRoot = document.FirstOrDefault<ParentTag>();
            if (documentRoot is { Name: "html" })
            {
                var feedUrls = FindAll(document);
                throw new HtmlContentDetectedException("HTML content was detected.", feedUrls);
            }

            throw new FeedTypeNotSupportedException($"Unknown feed type '{documentRoot?.Name}'.");
        }
    }
}
