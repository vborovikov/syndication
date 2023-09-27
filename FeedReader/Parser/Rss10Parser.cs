namespace CodeHollow.FeedReader.Parser
{
    using Brackets;
    using CodeHollow.FeedReader;
    using Feeds;

    internal class Rss10Parser : AbstractXmlFeedParser
    {
        protected override BaseFeed ParseOverride(string feedXml, Document feedDoc)
        {
            var rdf = feedDoc.Root();
            var channel = rdf.Root("channel");
            Rss10Feed feed = new Rss10Feed(feedXml, channel);
            return feed;
        }
    }
}