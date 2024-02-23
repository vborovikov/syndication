namespace Syndication.Parser
{
    using Brackets;
    using Syndication;
    using Feeds;

    internal class Rss10Parser : AbstractXmlFeedParser
    {
        protected override BaseFeed ParseOverride(Document feedDoc, string feedXml)
        {
            var rdf = feedDoc.Root();
            var channel = rdf.Root("channel");
            Rss10Feed feed = new Rss10Feed(feedXml, channel);
            return feed;
        }
    }
}