namespace Syndication.Tests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class FeedReaderTest
{
    [TestMethod]
    public async Task TestDownload400BadRequest()
    {
        // results in a 400 BadRequest if webclient is not initialized correctly
        await DownloadTestAsync("http://www.methode.at/blog?format=RSS");
    }

    [TestMethod]
    public async Task TestAcceptForbiddenUserAgent()
    {
        // results in 403 Forbidden if webclient does not have the accept header set
        await DownloadTestAsync("https://mikeclayton.wordpress.com/feed/");
    }

    [TestMethod]
    public async Task TestAcceptForbiddenUserAgentWrike()
    {
        // results in 403 Forbidden if webclient does not have the accept header set
        await DownloadTestAsync("https://www.wrike.com/blog");
    }

    [TestMethod]
    public async Task TestParseRssLinksCodehollow()
    {
        await TestParseRssLinksAsync("https://codehollow.com", 2);
    }

    [TestMethod]
    public async Task TestParseRssLinksHeise() { await TestParseRssLinksAsync("http://heise.de/", 2); }
    [TestMethod]
    public async Task TestParseRssLinksHeise2() { await TestParseRssLinksAsync("heise.de", 2); }
    [TestMethod]
    public async Task TestParseRssLinksHeise3() { await TestParseRssLinksAsync("www.heise.de", 2); }
    [TestMethod]
    public async Task TestParseRssLinksNYTimes() { await TestParseRssLinksAsync("nytimes.com", 1); }

    private static async Task TestParseRssLinksAsync(string url, int expectedNumberOfLinks)
    {
        string[] urls = await FeedReader.ParseFeedUrlsAsStringAsync(url);
        Assert.AreEqual(expectedNumberOfLinks, urls.Length);
    }

    [TestMethod]
    public async Task TestParseAndAbsoluteUrlDerStandard1()
    {
        string url = "derstandard.at";
        var links = await FeedReader.GetFeedUrlsFromUrlAsync(url);

        foreach (var link in links)
        {
            var absoluteUrl = FeedReader.GetAbsoluteFeedUrl(url, link);
            Assert.IsTrue(absoluteUrl.Url.StartsWith("http://"));
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
    public async Task TestReadBuildAzure()
    {
        await DownloadTestAsync("https://buildazure.com");
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

    private static async Task DownloadTestAsync(string url)
    {
        var content = await Helpers.DownloadAsync(url);
        Assert.IsTrue(content.Length > 200);
    }
}
