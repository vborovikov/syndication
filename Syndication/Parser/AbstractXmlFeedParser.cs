namespace CodeHollow.FeedReader.Parser
{
    using Brackets;
    using Feeds;

    internal abstract class AbstractXmlFeedParser : IFeedParser
    {
        public BaseFeed Parse(Document feedDoc, string feedXml)
        {
            return this.ParseOverride(feedDoc, feedXml);
        }

        protected abstract BaseFeed ParseOverride(Document feedDoc, string feedXml);
    }
}
