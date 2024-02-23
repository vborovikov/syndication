namespace Syndication.Parser;

using Brackets;
using Feeds;

internal class Rss091Parser : AbstractXmlFeedParser
{
    protected override BaseFeed ParseOverride(Document feedDoc, string feedXml)
    {
        var rss = feedDoc.First<ParentTag>();
        var channel = rss.First<ParentTag>(r => r.Name == "channel");
        Rss091Feed feed = new Rss091Feed(feedXml, channel);
        return feed;
    }
}
