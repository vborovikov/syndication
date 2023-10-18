namespace CodeHollow.FeedReader.Parser
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Brackets;
    using CodeHollow.FeedReader;

    /// <summary>
    /// Internal FeedParser - returns the type of the feed or the parsed feed.
    /// </summary>
    internal static class FeedParser
    {
        /// <summary>
        /// Returns the feed type - rss 1.0, rss 2.0, atom, ...
        /// </summary>
        /// <param name="doc">the xml document</param>
        /// <returns>the feed type</returns>
        public static FeedType ParseFeedType(Document doc)
        {
            var docRoot = doc.Root();
            var rootElement = docRoot.Name.AsSpan()[(docRoot.Name.IndexOf(':') + 1)..];

            if (rootElement.EqualsIgnoreCase("feed"))
                return FeedType.Atom;

            if (rootElement.EqualsIgnoreCase("rdf"))
                return FeedType.Rss_1_0;

            if (rootElement.EqualsIgnoreCase("rss"))
            {
                var version = docRoot.GetAttributeValue("version");
                if (version.EqualsIgnoreCase("2.0"))
                {
                    if (docRoot.Attribute("media") != null)
                    {
                        return FeedType.MediaRss;
                    }
                    else
                    {
                        return FeedType.Rss_2_0;
                    }
                }

                if (version.EqualsIgnoreCase("0.91"))
                    return FeedType.Rss_0_91;

                if (version.EqualsIgnoreCase("0.92"))
                    return FeedType.Rss_0_92;

                return FeedType.Rss;
            }

            throw new FeedTypeNotSupportedException($"unknown feed type {rootElement}");
        }


        /// <summary>
        /// Returns the parsed feed.
        /// This method checks the encoding of the received file
        /// </summary>
        /// <param name="feedStream">the feed document</param>
        /// <returns>parsed feed</returns>
        public static async Task<Feed> GetFeedAsync(Stream feedStream, CancellationToken cancellationToken)
        {
            var feedDoc = await Document.Xml.ParseAsync(feedStream, cancellationToken).ConfigureAwait(false);
            // in some cases - ISO-8859-1 - Encoding.UTF8.GetString doesn't work correct, so converting
            // from UTF8 to ISO-8859-1 doesn't work and result is wrong. see: FullParseTest.TestRss20ParseSwedishFeedWithIso8859_1
            return GetFeed(feedDoc, string.Empty);
        }

        /// <summary>
        /// Returns the parsed feed
        /// </summary>
        /// <param name="feedContent">the feed document</param>
        /// <returns>parsed feed</returns>
        public static Feed GetFeed(string feedContent)
        {
            var feedDoc = Document.Xml.Parse(feedContent);
            return GetFeed(feedDoc, feedContent);
        }

        private static Feed GetFeed(Document feedDoc, string feedContent)
        {
            var feedType = ParseFeedType(feedDoc);
            var parser = Factory.GetParser(feedType);
            var feed = parser.Parse(feedContent, feedDoc);

            return feed.ToFeed();
        }
    }
}
