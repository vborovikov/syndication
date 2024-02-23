namespace Syndication.Parser;

using Brackets;
using Feeds;

internal class Rss092Parser : AbstractXmlFeedParser
{
    protected override BaseFeed ParseOverride(Document feedDoc, string feedXml)
    {
        var rss = feedDoc.First<ParentTag>();
        var channel = rss.First<ParentTag>(r => r.Name == "channel");
        Rss092Feed feed = new Rss092Feed(feedXml, channel);
        return feed;
    }
}
