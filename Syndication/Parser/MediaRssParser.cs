namespace Syndication.Parser;

using Brackets;
using Feeds;

internal class MediaRssParser : AbstractXmlFeedParser
{
    protected override BaseFeed ParseOverride(Document feedDoc, string feedXml)
    {
        var rss = feedDoc.First<ParentTag>();
        var channel = rss.First<ParentTag>(r => r.Name == "channel");
        return new MediaRssFeed(feedXml, channel);
    }
}