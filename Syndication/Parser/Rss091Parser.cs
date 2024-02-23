namespace Syndication.Parser
{
    using Brackets;
    using Syndication;
    using Feeds;

    internal class Rss091Parser : AbstractXmlFeedParser
    {
        protected override BaseFeed ParseOverride(Document feedDoc, string feedXml)
        {
            var rss = feedDoc.Root();
            var channel = rss.Root("channel");
            Rss091Feed feed = new Rss091Feed(feedXml, channel);
            return feed;
        }
    }
}
