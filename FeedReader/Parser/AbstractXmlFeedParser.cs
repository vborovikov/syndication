namespace CodeHollow.FeedReader.Parser
{
    using Brackets;
    using Feeds;

    internal abstract class AbstractXmlFeedParser : IFeedParser
    {
        public BaseFeed Parse(string feedXml, Document feedDoc)
        {
            return this.ParseOverride(feedXml, feedDoc);
        }

        protected abstract BaseFeed ParseOverride(string feedXml, Document feedDoc);
    }
}
