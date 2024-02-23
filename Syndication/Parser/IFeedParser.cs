using Brackets;
using Syndication.Feeds;

namespace Syndication.Parser
{
    internal interface IFeedParser
    {
        BaseFeed Parse(Document feedDoc, string feedXml);
    }
}
