namespace CodeHollow.FeedReader.Parser
{
    using Brackets;
    using CodeHollow.FeedReader;
    using Feeds;

    internal class AtomParser : AbstractXmlFeedParser
    {
        protected override BaseFeed ParseOverride(string feedXml, Document feedDoc)
        {
            AtomFeed feed = new AtomFeed(feedXml, feedDoc.Root());
            return feed;
        }
    }
}
