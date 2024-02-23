namespace Syndication.Parser
{
    using Brackets;
    using Syndication;
    using Feeds;

    internal class MediaRssParser : AbstractXmlFeedParser
    {
        protected override BaseFeed ParseOverride(Document feedDoc, string feedXml)
        {
            var rss = feedDoc.Root();
            var channel = rss.Root("channel");
            MediaRssFeed feed = new MediaRssFeed(feedXml, channel);
            return feed;
        }
    }
}