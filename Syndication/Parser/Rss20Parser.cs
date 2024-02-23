namespace Syndication.Parser
{
    using Brackets;
    using Syndication;
    using Feeds;

    internal class Rss20Parser : AbstractXmlFeedParser
    {
        protected override BaseFeed ParseOverride(Document feedDoc, string feedXml)
        {
            var rss = feedDoc.Root();
            var channel = rss.Root("channel");
            Rss20Feed feed = new Rss20Feed(feedXml, channel);
            return feed;
        }
    }
}