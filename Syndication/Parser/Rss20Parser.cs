namespace Syndication.Parser;

using Brackets;
using Feeds;

internal class Rss20Parser : AbstractXmlFeedParser
{
    protected override BaseFeed ParseOverride(Document feedDoc, string feedXml)
    {
        var rss = feedDoc.First<ParentTag>();
        var channel = rss.First<ParentTag>(r => r.Name == "channel");
        Rss20Feed feed = new Rss20Feed(feedXml, channel);
        return feed;
    }
}