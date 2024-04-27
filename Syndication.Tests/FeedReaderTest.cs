namespace Syndication.Tests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Brackets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class FeedReaderTest
{
    [TestMethod]
    public async Task TestParseRssLinksCodehollow()
    {
        await TestParseRssLinksAsync("Web/codehollow_com.html", 2);
    }

    [TestMethod]
    public async Task TestParseRssLinksHeise()
    {
        await TestParseRssLinksAsync("Web/heise_de.html", 2);
    }

    [TestMethod]
    public async Task TestParseRssLinksNYTimes()
    {
        await TestParseRssLinksAsync("Web/nytimes_com.html", 1);
    }

    private static async Task TestParseRssLinksAsync(string url, int expectedNumberOfLinks)
    {
        var document = await Document.Html.ParseAsync(Samples.GetStream(url), default);
        var urls = Feed.FindAll(document);
        Assert.AreEqual(expectedNumberOfLinks, urls.Length);
    }

    [TestMethod]
    public async Task TestParseAndAbsoluteUrlDerStandard1()
    {
        string url = "derstandard.at";
        var document = await Document.Html.ParseAsync(Samples.GetStream("Web/derstandard_at.html"), default);
        var links = Feed.FindAll(document);

        foreach (var link in links)
        {
            var absoluteUrl = FeedReader.GetAbsoluteFeedUrl(url, link);
            Assert.IsTrue(absoluteUrl.Url.StartsWith("https://"));
        }
    }

    [TestMethod]
    public void GetAbsoluteFeedUrl_RelativePath_Concat()
    {
        var feedLink = FeedReader.GetAbsoluteFeedUrl("https://devleader.substack.com/", new HtmlFeedLink("Dev Leader", "/feed", FeedType.Rss_2_0));
        Assert.AreEqual("https://devleader.substack.com/feed", feedLink.Url);
    }

    [TestMethod]
    public async Task TestReadSimpleFeed()
    {
        var feed = await FeedReader.ReadAsync("https://arminreiter.com/feed");
        string title = feed.Title;
        Assert.AreEqual("arminreiter.com", title);
        Assert.AreEqual(10, feed.Items.Count());
    }

    [TestMethod]
    public async Task TestReadRss20GermanFeed()
    {
        var feed = await FeedReader.ReadAsync("http://guidnew.com/feed");
        string title = feed.Title;
        Assert.AreEqual("Guid.New", title);
        Assert.IsTrue(feed.Items.Count > 0);
    }

    [TestMethod]
    public async Task TestReadRss10GermanFeed()
    {
        var feed = await FeedReader.ReadAsync("http://rss.orf.at/news.xml");
        string title = feed.Title;
        Assert.AreEqual("news.ORF.at", title);
        Assert.IsTrue(feed.Items.Count > 10);
    }

    [TestMethod]
    public async Task TestReadAtomFeedHeise()
    {
        var feed = await FeedReader.ReadAsync("https://www.heise.de/newsticker/heise-atom.xml");
        Assert.IsTrue(!string.IsNullOrEmpty(feed.Title));
        Assert.IsTrue(feed.Items.Count > 1);
    }

    [TestMethod]
    public async Task TestReadAtomFeedGitHub()
    {
        try
        {
            var feed = await FeedReader.ReadAsync("http://github.com/codehollow/AzureBillingRateCardSample/commits/master.atom");
            //Assert.IsTrue(!string.IsNullOrEmpty(feed.Title));
        }
        catch (Exception ex)
        {
            Assert.AreEqual(ex.InnerException.GetType(), typeof(System.Net.WebException));
            Assert.AreEqual(ex.InnerException.Message, "The request was aborted: Could not create SSL/TLS secure channel.");
        }
    }

    [TestMethod]
    public async Task TestReadRss20GermanFeedPowershell()
    {
        var feed = await FeedReader.ReadAsync("http://www.powershell.co.at/feed/");
        Assert.IsTrue(!string.IsNullOrEmpty(feed.Title));
        Assert.IsTrue(feed.Items.Count > 0);
    }

    [TestMethod]
    public async Task TestReadRssScottHanselmanWeb()
    {
        var feed = await FeedReader.ReadAsync("http://feeds.hanselman.com/ScottHanselman");
        Assert.IsTrue(!string.IsNullOrEmpty(feed.Title));
        Assert.IsTrue(feed.Items.Count > 0);
    }

    [TestMethod]
    public async Task TestReadNoticiasCatolicas()
    {
        var feed = await FeedReader.ReadAsync("feeds.feedburner.com/NoticiasCatolicasAleteia");
        Assert.AreEqual("Noticias Catolicas", feed.Title);
        Assert.IsTrue(feed.Items.Count > 0);
    }

    [TestMethod]
    public async Task TestReadTimeDoctor()
    {
        var feed = await FeedReader.ReadAsync("https://www.timedoctor.com/blog/feed/");
        Assert.AreEqual("Time Doctor Blog", feed.Title);
        Assert.IsTrue(feed.Items.Count > 0);
    }

    [TestMethod]
    public async Task TestReadMikeC()
    {
        var feed = await FeedReader.ReadAsync("https://mikeclayton.wordpress.com/feed/");
        Assert.AreEqual("Shift Happens!", feed.Title);
        Assert.IsTrue(feed.Items.Count > 0);
    }

    [TestMethod]
    public async Task TestReadTheLPM()
    {
        var feed = await FeedReader.ReadAsync("https://thelazyprojectmanager.wordpress.com/feed/");
        Assert.AreEqual("The Lazy Project Manager's Blog", feed.Title);
        Assert.IsTrue(feed.Items.Count > 0);
    }

    [TestMethod]
    public async Task TestReadTechRep()
    {
        var feed = await FeedReader.ReadAsync("http://www.techrepublic.com/rssfeeds/topic/project-management/");
        Assert.AreEqual("Project Management Articles & Tutorials | TechRepublic", feed.Title);
        Assert.IsTrue(feed.Items.Count > 0);
    }

    [TestMethod]
    public async Task TestReadAPOD()
    {
        var feed = await FeedReader.ReadAsync("https://apod.nasa.gov/apod.rss");
        Assert.AreEqual("APOD", feed.Title);
        Assert.IsTrue(feed.Items.Count > 0);
    }

    [TestMethod]
    public async Task TestReadThaqafnafsak()
    {
        var feed = await FeedReader.ReadAsync("http://www.thaqafnafsak.com/feed");
        Assert.AreEqual("ثقف نفسك", feed.Title);
        Assert.IsTrue(feed.Items.Count > 0);
    }

    [TestMethod]
    public async Task TestReadLiveBold()
    {
        var feed = await FeedReader.ReadAsync("http://feeds.feedburner.com/LiveBoldAndBloom");
        Assert.AreEqual("Live Bold and Bloom", feed.Title);
        Assert.IsTrue(feed.Items.Count > 0);
    }

    [TestMethod]
    public async Task TestSwedish_ISO8859_1()
    {
        var feed = await FeedReader.ReadAsync("https://www.retriever-info.com/feed/2004645/intranet30/index.xml");
        Assert.AreEqual("intranet30", feed.Title);
    }

    [TestMethod]
    public async Task TestStadtfeuerwehrWeiz_ISO8859_1()
    {
        var feed = await FeedReader.ReadAsync("http://www.stadtfeuerwehr-weiz.at/rss/einsaetze.xml");
        Assert.AreEqual("Stadtfeuerwehr Weiz - Einsätze", feed.Title);
    }
}
