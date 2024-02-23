namespace CodeHollow.FeedReader.Parser
{
    using Brackets;
    using CodeHollow.FeedReader;
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