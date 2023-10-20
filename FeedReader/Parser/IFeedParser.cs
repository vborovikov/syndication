using Brackets;
using CodeHollow.FeedReader.Feeds;

namespace CodeHollow.FeedReader.Parser
{
    internal interface IFeedParser
    {
        BaseFeed Parse(Document feedDoc, string feedXml);
    }
}
