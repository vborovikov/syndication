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
    public async Task TestReadSimpleFeed()
    {
        var feed = await Feed.FromStreamAsync(Samples.GetStream("Web/arminreiter_com.xml"), default);
        string title = feed.Title;
        Assert.AreEqual("arminreiter.com", title);
        Assert.AreEqual(10, feed.Items.Count());
    }

    [TestMethod]
    public async Task TestReadRss20GermanFeed()
    {
        var feed = await Feed.FromStreamAsync(Samples.GetStream("Web/guidnew_com.xml"), default);
        string title = feed.Title;
        Assert.AreEqual("Guid.New", title);
        Assert.IsTrue(feed.Items.Count > 0);
    }

    [TestMethod]
    public async Task TestReadRss10GermanFeed()
    {
        var feed = await Feed.FromStreamAsync(Samples.GetStream("Web/rss_orf_at.xml"), default);
        string title = feed.Title;
        Assert.AreEqual("news.ORF.at", title);
        Assert.IsTrue(feed.Items.Count > 10);
    }

    [TestMethod]
    public async Task TestReadAtomFeedHeise()
    {
        var feed = await Feed.FromStreamAsync(Samples.GetStream("Web/heise-atom.xml"), default);
        Assert.IsTrue(!string.IsNullOrEmpty(feed.Title));
        Assert.IsTrue(feed.Items.Count > 1);
    }

    [TestMethod]
    public async Task TestReadAtomFeedGitHub()
    {
        try
        {
            var feed = await Feed.FromStreamAsync(Samples.GetStream("Web/AzureBillingRateCardSample.xml"), default);
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
        var feed = await Feed.FromStreamAsync(Samples.GetStream("Web/powershell_co_at.xml"), default);
        Assert.IsTrue(!string.IsNullOrEmpty(feed.Title));
        Assert.IsTrue(feed.Items.Count > 0);
    }

    [TestMethod]
    public async Task TestReadRssScottHanselmanWeb()
    {
        var feed = await Feed.FromStreamAsync(Samples.GetStream("Web/ScottHanselman.xml"), default);
        Assert.IsTrue(!string.IsNullOrEmpty(feed.Title));
        Assert.IsTrue(feed.Items.Count > 0);
    }

    [TestMethod]
    public async Task TestReadNoticiasCatolicas()
    {
        var feed = await Feed.FromStreamAsync(Samples.GetStream("Web/NoticiasCatolicasAleteia.xml"), default);
        Assert.AreEqual("Noticias Catolicas", feed.Title);
        Assert.IsTrue(feed.Items.Count > 0);
    }

    [TestMethod]
    public async Task TestReadTimeDoctor()
    {
        var feed = await Feed.FromStreamAsync(Samples.GetStream("Web/timedoctor_com.xml"), default);
        Assert.AreEqual("Time Doctor Blog", feed.Title);
        Assert.IsTrue(feed.Items.Count > 0);
    }

    [TestMethod]
    public async Task TestReadMikeC()
    {
        var feed = await Feed.FromStreamAsync(Samples.GetStream("Web/mikeclayton.xml"), default);
        Assert.AreEqual("Shift Happens!", feed.Title);
        Assert.IsTrue(feed.Items.Count > 0);
    }

    [TestMethod]
    public async Task TestReadTheLPM()
    {
        var feed = await Feed.FromStreamAsync(Samples.GetStream("Web/thelazyprojectmanager.xml"), default);
        Assert.AreEqual("The Lazy Project Manager's Blog", feed.Title);
        Assert.IsTrue(feed.Items.Count > 0);
    }

    [TestMethod]
    public async Task TestReadTechRep()
    {
        var feed = await Feed.FromStreamAsync(Samples.GetStream("Web/techrepublic_com.xml"), default);
        Assert.AreEqual("Project Management Articles & Tutorials | TechRepublic", feed.Title);
        Assert.IsTrue(feed.Items.Count > 0);
    }

    [TestMethod]
    public async Task TestReadAPOD()
    {
        var feed = await Feed.FromStreamAsync(Samples.GetStream("Web/apod_nasa_gov.xml"), default);
        Assert.AreEqual("APOD", feed.Title);
        Assert.IsTrue(feed.Items.Count > 0);
    }

    [TestMethod]
    public async Task TestReadThaqafnafsak()
    {
        var feed = await Feed.FromStreamAsync(Samples.GetStream("Web/thaqafnafsak_com.xml"), default);
        Assert.AreEqual("ثقف نفسك", feed.Title);
        Assert.IsTrue(feed.Items.Count > 0);
    }

    [TestMethod]
    public async Task TestReadLiveBold()
    {
        var feed = await Feed.FromStreamAsync(Samples.GetStream("Web/LiveBoldAndBloom.xml"), default);
        Assert.AreEqual("Live Bold and Bloom", feed.Title);
        Assert.IsTrue(feed.Items.Count > 0);
    }

    [TestMethod]
    public async Task TestSwedish_ISO8859_1()
    {
        var feed = await Feed.FromStreamAsync(Samples.GetStream("Web/retriever-info_com.xml"), default);
        Assert.AreEqual("intranet30", feed.Title);
    }

    [TestMethod]
    public async Task TestStadtfeuerwehrWeiz_ISO8859_1()
    {
        var feed = await Feed.FromStreamAsync(Samples.GetStream("Web/stadtfeuerwehr-weiz_at.xml"), default);
        Assert.AreEqual("Stadtfeuerwehr Weiz - Einsätze", feed.Title);
    }
}
