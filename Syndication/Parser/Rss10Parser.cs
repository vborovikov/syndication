namespace Syndication.Parser;

using Brackets;
using Feeds;

internal class Rss10Parser : AbstractXmlFeedParser
{
    protected override BaseFeed ParseOverride(Document feedDoc, string feedXml)
    {
        var rdf = feedDoc.First<ParentTag>();
        var channel = rdf.First<ParentTag>(r => r.Name == "channel");
        Rss10Feed feed = new Rss10Feed(feedXml, channel);
        return feed;
    }
}