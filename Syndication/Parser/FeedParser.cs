namespace Syndication.Parser
{
    using System;
    using System.Text;
    using Brackets;

    /// <summary>
    /// Internal FeedParser - returns the type of the feed or the parsed feed.
    /// </summary>
    internal static class FeedParser
    {
        /// <summary>
        /// Determines the feed type.
        /// </summary>
        /// <param name="doc">The xml document</param>
        /// <returns>The feed type</returns>
        /// <exception cref="FeedTypeNotSupportedException">The feed type couldn't be determined.</exception>
        public static FeedType ParseFeedType(Document doc)
        {
            if (TryParseFeedType(doc, out var feedType))
                return feedType;

            throw new FeedTypeNotSupportedException($"Unknown feed type '{doc.FirstOrDefault<ParentTag>()?.Name}'.");
        }

        public static bool TryParseFeedType(Document doc, out FeedType feedType)
        {
            var docRoot = doc.FirstOrDefault<ParentTag>();
            if (docRoot is null)
            {
                feedType = FeedType.Unknown;
                return false;
            }
            var rootElement = docRoot.Name.AsSpan()[(docRoot.Name.IndexOf(':') + 1)..];

            if (rootElement.EqualsIgnoreCase("feed"))
            {
                feedType = FeedType.Atom;
                return true;
            }

            if (rootElement.EqualsIgnoreCase("rdf"))
            {
                feedType = FeedType.Rss_1_0;
                return true;
            }

            if (rootElement.EqualsIgnoreCase("rss"))
            {
                var version = docRoot.GetAttributeValue("version");
                if (version.EqualsIgnoreCase("2.0"))
                {
                    if (docRoot.Attribute("media") != null)
                    {
                        feedType = FeedType.MediaRss;
                        return true;
                    }
                    else
                    {
                        feedType = FeedType.Rss_2_0;
                        return true;
                    }
                }

                if (version.EqualsIgnoreCase("0.91"))
                {
                    feedType = FeedType.Rss_0_91;
                    return true;
                }

                if (version.EqualsIgnoreCase("0.92"))
                {
                    feedType = FeedType.Rss_0_92;
                    return true;
                }

                feedType = FeedType.Rss;
                return true;
            }

            feedType = FeedType.Unknown;
            return false;
        }


        /// <summary>
        /// Returns the parsed feed.
        /// This method checks the encoding of the received file
        /// </summary>
        /// <param name="feedContentData">the feed document</param>
        /// <returns>parsed feed</returns>
        public static Feed GetFeed(byte[] feedContentData)
        {
            string feedContent = Encoding.UTF8.GetString(feedContentData); // 1.) get string of the content
            var feedDoc = Document.Xml.Parse(feedContent); // 2.) read document to get the used encoding
            Encoding encoding = GetEncoding(feedDoc); // 3.) get used encoding

            if (encoding != Encoding.UTF8) // 4.) if not UTF8 - reread the data :
                                           // in some cases - ISO-8859-1 - Encoding.UTF8.GetString doesn't work correct, so converting
                                           // from UTF8 to ISO-8859-1 doesn't work and result is wrong. see: FullParseTest.TestRss20ParseSwedishFeedWithIso8859_1
            {
                feedContent = encoding.GetString(feedContentData);
                feedDoc = Document.Xml.Parse(feedContent);
            }

            var feedType = ParseFeedType(feedDoc);
            var parser = Factory.GetParser(feedType);
            var feed = parser.Parse(feedDoc, feedContent);

            return feed.ToFeed();
        }

        /// <summary>
        /// Returns the parsed feed
        /// </summary>
        /// <param name="feedContent">the feed document</param>
        /// <returns>parsed feed</returns>
        public static Feed GetFeed(string feedContent)
        {
            Document feedDoc = Document.Xml.Parse(feedContent);

            var feedType = ParseFeedType(feedDoc);

            var parser = Factory.GetParser(feedType);
            var feed = parser.Parse(feedDoc, feedContent);

            return feed.ToFeed();
        }

        /// <summary>
        /// reads the encoding from a feed document, returns UTF8 by default
        /// </summary>
        /// <param name="feedDoc">rss feed document</param>
        /// <returns>encoding or utf8 by default</returns>
        private static Encoding GetEncoding(Document feedDoc)
        {
            Encoding encoding = Encoding.UTF8;

            if (feedDoc.FirstOrDefault(e => e is Instruction { Name: "xml", HasAttributes: true }) is Instruction xml &&
                xml.Attributes.FirstOrDefault(a => a is Attr { Name: "encoding", HasValue: true }) is Attr encodingAttr)
            {
                var encodingStr = encodingAttr.ToString();
                if (!string.IsNullOrWhiteSpace(encodingStr))
                {
                    encoding = Encoding.GetEncoding(encodingStr);
                }
            }

            return encoding;
        }
    }
}
