namespace CodeHollow.FeedReader.Parser
{
    using Brackets;
    using CodeHollow.FeedReader;
    using Feeds;

    internal class Rss092Parser : AbstractXmlFeedParser
    {
        protected override BaseFeed ParseOverride(string feedXml, Document feedDoc)
        {
            var rss = feedDoc.Root();
            var channel = rss.Root("channel");
            Rss092Feed feed = new Rss092Feed(feedXml, channel);
            return feed;
        }
    }
}
