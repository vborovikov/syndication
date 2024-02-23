namespace Syndication.Parser;

using Brackets;
using Feeds;

internal class AtomParser : AbstractXmlFeedParser
{
    protected override BaseFeed ParseOverride(Document feedDoc, string feedXml)
    {
        AtomFeed feed = new AtomFeed(feedXml, feedDoc.First<ParentTag>());
        return feed;
    }
}
