namespace Syndication.Parser
{
    using Brackets;
    using Syndication;
    using Feeds;

    internal class Rss092Parser : AbstractXmlFeedParser
    {
        protected override BaseFeed ParseOverride(Document feedDoc, string feedXml)
        {
            var rss = feedDoc.Root();
            var channel = rss.Root("channel");
            Rss092Feed feed = new Rss092Feed(feedXml, channel);
            return feed;
        }
    }
}
