namespace CodeHollow.FeedReader.Parser
{
    using Brackets;
    using CodeHollow.FeedReader;
    using Feeds;

    internal class AtomParser : AbstractXmlFeedParser
    {
        protected override BaseFeed ParseOverride(Document feedDoc, string feedXml)
        {
            AtomFeed feed = new AtomFeed(feedXml, feedDoc.Root());
            return feed;
        }
    }
}
