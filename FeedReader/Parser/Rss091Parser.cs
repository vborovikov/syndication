namespace CodeHollow.FeedReader.Parser
{
    using Brackets;
    using CodeHollow.FeedReader;
    using Feeds;

    internal class Rss091Parser : AbstractXmlFeedParser
    {
        protected override BaseFeed ParseOverride(string feedXml, Document feedDoc)
        {
            var rss = feedDoc.Root();
            var channel = rss.Root("channel");
            Rss091Feed feed = new Rss091Feed(feedXml, channel);
            return feed;
        }
    }
}
