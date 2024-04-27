namespace Syndication.Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Syndication.Feeds;
using Syndication.Feeds.Itunes;

[TestClass]
public class FullParseTest
{
    [TestMethod]
    public void TestAtomParseAdobe()
    {
        var feed = (AtomFeed)Feed.FromString(Samples.GetContent("Feeds/AtomAdobe.xml")).SpecificFeed;

        Assert.AreEqual("Adobe Blog", feed.Title);
        Assert.AreEqual(null, feed.Icon);
        Assert.AreEqual("https://blog.adobe.com/", feed.Link);
        Assert.AreEqual("2021-07-19T00:00:00.000Z", feed.UpdatedAsString);
        Assert.AreEqual("https://blog.adobe.com/", feed.Id);

        var item = (AtomFeedItem)feed.Items.First();
        Assert.AreEqual(null, item.Link); // The post href is store in the id element
    }

    [TestMethod]
    public void TestAtomParseTheVerge()
    {
        var feed = (AtomFeed)Feed.FromString(Samples.GetContent("Feeds/AtomTheVerge.xml")).SpecificFeed;

        Assert.AreEqual("The Verge -  Front Pages", feed.Title);
        Assert.AreEqual("https://cdn2.vox-cdn.com/community_logos/34086/verge-fv.png", feed.Icon);
        Assert.AreEqual("2017-01-07T09:00:01-05:00", feed.UpdatedAsString);
        Assert.AreEqual(new DateTimeOffset(2017, 1, 7, 9, 0, 1, TimeSpan.FromHours(-5)), feed.Updated);
        Assert.AreEqual("http://www.theverge.com/rss/group/front-page/index.xml", feed.Id);

        var item = (AtomFeedItem)feed.Items.First();

        Assert.AreEqual("2017-01-07T09:00:01-05:00", item.UpdatedAsString);
        Assert.AreEqual(new DateTimeOffset(2017, 1, 7, 9, 0, 1, TimeSpan.FromHours(-5)), item.Updated);
        Assert.AreEqual("2017-01-07T09:00:01-05:00", item.PublishedAsString);
        Assert.AreEqual(new DateTimeOffset(2017, 1, 7, 9, 0, 1, TimeSpan.FromHours(-5)), item.Published);
        Assert.AreEqual("This is the new Hulu experience with live TV", item.Title);
        Assert.AreEqual("http://www.theverge.com/ces/2017/1/7/14195588/hulu-live-tv-streaming-internet-ces-2017", item.Id);
        Assert.AreEqual("http://www.theverge.com/ces/2017/1/7/14195588/hulu-live-tv-streaming-internet-ces-2017", item.Link);

        Assert.IsTrue(item.Content.Trim().StartsWith("<img alt=\"\""));

        Assert.AreEqual("Chris Welch", item.Author.Name);
    }

    [TestMethod]
    public void TestAtomParseBattleNet()
    {
        var feed = (AtomFeed)Feed.FromString(Samples.GetContent("Feeds/AtomBattleNet.xml")).SpecificFeed;

        Assert.AreEqual("StarCraft® II", feed.Title);
        Assert.AreEqual(null, feed.Icon);
        Assert.AreEqual(null, feed.Link);
        Assert.AreEqual("2018-11-20T19:59:19.147Z", feed.UpdatedAsString);
        Assert.AreEqual("3", feed.Id);
    }

    [TestMethod]
    public void TestAtomYouTubeInvestmentPunk()
    {
        var feed = (AtomFeed)Feed.FromString(Samples.GetContent("Feeds/AtomYoutubeInvestmentPunk.xml")).SpecificFeed;

        Assert.AreEqual("http://www.youtube.com/feeds/videos.xml?channel_id=UCmEN5ZnsHUXIxgpLitRTmWw", feed.Links.First().Href);
        Assert.AreEqual("yt:channel:UCmEN5ZnsHUXIxgpLitRTmWw", feed.Id);
        Assert.AreEqual("Investment Punk Academy by Gerald Hörhan", feed.Title);
        Assert.AreEqual("http://www.youtube.com/channel/UCmEN5ZnsHUXIxgpLitRTmWw", feed.Links.ElementAt(1).Href);
        Assert.AreEqual("Investment Punk Academy by Gerald Hörhan", feed.Author.Name);
        Assert.AreEqual("http://www.youtube.com/channel/UCmEN5ZnsHUXIxgpLitRTmWw", feed.Author.Uri);
        var item = (AtomFeedItem)feed.Items.First();
        Assert.AreEqual("yt:video:AFA8ZtMwrvc", item.Id);
        Assert.AreEqual("Zukunft von Vertretern I Kernfusion I Musikgeschäft #ASKTHEPUNK 71", item.Title);
        Assert.AreEqual("alternate", item.Links.First().Relation);
        Assert.AreEqual("2017-01-23T18:14:49+00:00", item.UpdatedAsString);
        Assert.AreEqual("2017-01-20T16:00:00+00:00", item.PublishedAsString);
    }


    [TestMethod]
    public void TestAtomWiganwarriors()
    {
        var feed = (AtomFeed)Feed.FromString(Samples.GetContent("Feeds/AtomWiganwarriors.xml")).SpecificFeed;

        Assert.AreEqual("Wigan Warriors Blog", feed.Title);
        Assert.AreEqual("Wigan Warriors Official Website", feed.Subtitle);
        var item = (AtomFeedItem)feed.Items.First();
        Assert.AreEqual(4, item.Categories.Count);
        Assert.AreEqual("Community & Education", item.Categories.First());
    }

    [TestMethod]
    public void TestRss091ParseStadtFWeiz()
    {
        var feed = (Rss091Feed)Feed.FromSpan(Samples.GetBuffer("Feeds/Rss091Stadtfeuerwehr.xml")).SpecificFeed;

        Assert.AreEqual("Stadtfeuerwehr Weiz - Einsätze", feed.Title);
        Assert.AreEqual("http://www.stadtfeuerwehr-weiz.at", feed.Link);
        Assert.AreEqual("Die letzten 15 Einsätze der Stadtfeuerwehr Weiz.", feed.Description);
        Assert.AreEqual("de-de", feed.Language);
        Assert.AreEqual("Stadtfeuerwehr Weiz / Markus Horwath", feed.Copyright);

        var item = (Rss091FeedItem)feed.Items.First();

        Assert.AreEqual(@"[19.08.2018 - 07:08 Uhr] Brandmeldeanlagenalarm", item.Title.Trim());
        Assert.IsTrue(item.Description.Contains("Weitere Informationen"));
        Assert.AreEqual("http://www.stadtfeuerwehr-weiz.at/einsaetze/einsatz-detail/5220/", item.Link);
        Assert.AreEqual("Sun, 19 Aug 2018 07:08:00 +0100", item.PublishingDateString);
        Assert.AreEqual(new DateTimeOffset(2018, 8, 19, 7, 08, 0, TimeSpan.FromHours(1)), item.PublishingDate);

        Assert.AreEqual(15, feed.Items.Count);
    }

    [TestMethod]
    public void TestRss091ParseFullSample()
    {
        var feed = (Rss091Feed)Feed.FromString(Samples.GetContent("Feeds/Rss091FullSample.xml")).SpecificFeed;
        Assert.AreEqual("Copyright 1997-1999 UserLand Software, Inc.", feed.Copyright);
        Assert.AreEqual("Thu, 08 Jul 1999 07:00:00 GMT", feed.PublishingDateString);
        Assert.AreEqual("Thu, 08 Jul 1999 16:20:26 GMT", feed.LastBuildDateString);
        Assert.AreEqual("http://my.userland.com/stories/storyReader$11", feed.Docs);
        Assert.AreEqual("News and commentary from the cross-platform scripting community.", feed.Description);
        Assert.AreEqual("http://www.scripting.com/", feed.Link);
        Assert.AreEqual("Scripting News", feed.Title);
        Assert.AreEqual("http://www.scripting.com/", feed.Image.Link);
        Assert.AreEqual("Scripting News", feed.Image.Title);
        Assert.AreEqual("http://www.scripting.com/gifs/tinyScriptingNews.gif", feed.Image.Url);
        Assert.AreEqual(40, ((Rss091FeedImage)feed.Image).Height);
        Assert.AreEqual(78, ((Rss091FeedImage)feed.Image).Width);
        Assert.AreEqual("What is this used for?", ((Rss091FeedImage)feed.Image).Description);
        Assert.AreEqual("dave@userland.com (Dave Winer)", feed.ManagingEditor);
        Assert.AreEqual("dave@userland.com (Dave Winer)", feed.WebMaster);
        Assert.AreEqual("en-us", feed.Language);
        Assert.IsTrue(feed.SkipHours.Contains("6"));
        Assert.IsTrue(feed.SkipHours.Contains("7"));
        Assert.IsTrue(feed.SkipHours.Contains("8"));
        Assert.IsTrue(feed.SkipHours.Contains("9"));
        Assert.IsTrue(feed.SkipHours.Contains("10"));
        Assert.IsTrue(feed.SkipHours.Contains("11"));
        Assert.IsTrue(feed.SkipDays.Contains("Sunday"));
        Assert.AreEqual("(PICS-1.1 \"http://www.rsac.org/ratingsv01.html\" l gen true comment \"RSACi North America Server\" for \"http://www.rsac.org\" on \"1996.04.16T08:15-0500\" r (n 0 s 0 v 0 l 0))", feed.Rating);

        Assert.AreEqual(1, feed.Items.Count);
        var item = (Rss091FeedItem)feed.Items.First();
        Assert.AreEqual("stuff", item.Title);
        Assert.AreEqual("http://bar", item.Link);
        Assert.AreEqual("This is an article about some stuff", item.Description);

        Assert.AreEqual("Search Now!", feed.TextInput.Title);
        Assert.AreEqual("Enter your search terms", feed.TextInput.Description);
        Assert.AreEqual("find", feed.TextInput.Name);
        Assert.AreEqual("http://my.site.com/search.cgi", feed.TextInput.Link);
    }

    [TestMethod]
    public void TestRss092ParseFullSample()
    {
        var feed = (Rss092Feed)Feed.FromString(Samples.GetContent("Feeds/Rss092FullSample.xml")).SpecificFeed;

        Assert.AreEqual("Dave Winer: Grateful Dead", feed.Title);
        Assert.AreEqual("http://www.scripting.com/blog/categories/gratefulDead.html", feed.Link);
        Assert.AreEqual("A high-fidelity Grateful Dead song every day. This is where we're experimenting with enclosures on RSS news items that download when you're not using your computer. If it works (it will) it will be the end of the Click-And-Wait multimedia experience on the Internet. ", feed.Description);
        Assert.AreEqual("Fri, 13 Apr 2001 19:23:02 GMT", feed.LastBuildDateString);
        Assert.AreEqual("http://backend.userland.com/rss092", feed.Docs);
        Assert.AreEqual("dave@userland.com (Dave Winer)", feed.ManagingEditor);
        Assert.AreEqual("dave@userland.com (Dave Winer)", feed.WebMaster);
        Assert.AreEqual("data.ourfavoritesongs.com", feed.Cloud.Domain);
        Assert.AreEqual("80", feed.Cloud.Port);
        Assert.AreEqual("/RPC2", feed.Cloud.Path);
        Assert.AreEqual("ourFavoriteSongs.rssPleaseNotify", feed.Cloud.RegisterProcedure);
        Assert.AreEqual("xml-rpc", feed.Cloud.Protocol);

        Assert.AreEqual(22, feed.Items.Count);
        var item = (Rss092FeedItem)feed.Items.ElementAt(20);
        Assert.AreEqual("A touch of gray, kinda suits you anyway..", item.Description);
        Assert.AreEqual("http://www.scripting.com/mp3s/touchOfGrey.mp3", item.Enclosure.Url);
        Assert.AreEqual(5588242, item.Enclosure.Length);
        Assert.AreEqual("audio/mpeg", item.Enclosure.MediaType);

        var secondItem = (Rss092FeedItem)feed.Items.ElementAt(1);
        Assert.AreEqual("http://scriptingnews.userland.com/xml/scriptingNews2.xml", secondItem.Source.Url);
        Assert.AreEqual("Scripting News", secondItem.Source.Value);
    }

    [TestMethod]
    public void TestRss10ParseFullSample()
    {
        var feed = (Rss10Feed)Feed.FromString(Samples.GetContent("Feeds/Rss10FeedWebResourceSample.xml")).SpecificFeed;

        Assert.AreEqual("XML.com", feed.Title);
        Assert.AreEqual("http://xml.com/pub", feed.Link);
        Assert.AreEqual("\n      XML.com features a rich mix of information and services\n      for the XML community.\n    ", feed.Description);
        var image = (Rss10FeedImage)feed.Image;
        Assert.AreEqual("http://xml.com/universal/images/xml_tiny.gif", image.About);
        Assert.AreEqual("XML.com", image.Title);
        Assert.AreEqual("http://www.xml.com", image.Link);
        Assert.AreEqual("http://xml.com/universal/images/xml_tiny.gif", image.Url);
        Assert.AreEqual("Search XML.com", feed.TextInput.Title);
        Assert.AreEqual("Search XML.com's XML collection", feed.TextInput.Description);
        Assert.AreEqual("s", feed.TextInput.Name);
        Assert.AreEqual("http://search.xml.com", ((Rss10FeedTextInput)feed.TextInput).About);
        Assert.AreEqual("http://search.xml.com", feed.TextInput.Link);

        var item = (Rss10FeedItem)feed.Items.Last();

        Assert.AreEqual("http://xml.com/pub/2000/08/09/rdfdb/index.html", item.About);
        Assert.AreEqual("Putting RDF to Work", item.Title);
        Assert.AreEqual("http://xml.com/pub/2000/08/09/rdfdb/index.html", item.Link);
        Assert.AreEqual(186, item.Description.Length);
    }

    [TestMethod]
    public void TestRss10ParseOrfAt()
    {
        var feed = (Rss10Feed)Feed.FromString(Samples.GetContent("Feeds/Rss10OrfAt.xml")).SpecificFeed;
        Assert.AreEqual("news.ORF.at", feed.Title);
        Assert.AreEqual("http://orf.at/", feed.Link);
        Assert.AreEqual("2017-01-23T21:54:55+01:00", feed.DC.DateString);
        Assert.AreEqual("Die aktuellsten Nachrichten auf einen Blick - aus Österreich und der ganzen Welt. In Text, Bild und Video.", feed.Description);
        Assert.AreEqual("ORF Österreichischer Rundfunk, Wien", feed.DC.Publisher);
        Assert.AreEqual("ORF Online und Teletext GmbH & Co KG", feed.DC.Creator);
        Assert.AreEqual("de", feed.DC.Language);
        Assert.AreEqual("Copyright © 2017 ORF Online und Teletext GmbH & Co KG", feed.DC.Rights);
        Assert.AreEqual("hourly", feed.Sy.UpdatePeriod);
        Assert.AreEqual("2", feed.Sy.UpdateFrequency);
        Assert.AreEqual("2000-01-01T00:00:00Z", feed.Sy.UpdateBase);
        Assert.AreEqual(50, feed.Items.Count);

        var item = (Rss10FeedItem)feed.Items.ElementAt(4);

        Assert.AreEqual("Feldsperling erstmals häufigster Vogel", item.Title);
        Assert.AreEqual("http://orf.at/stories/2376365/", item.Link);
        Assert.AreEqual("Chronik", item.DC.Subject);
        Assert.AreEqual("2017-01-23T20:51:06+01:00", item.DC.DateString);
    }

    [TestMethod]
    public void TestRss20ParseWebResourceSampleFull()
    {
        var feed = (Rss20Feed)Feed.FromString(Samples.GetContent("Feeds/Rss20FeedWebResourceSample.xml")).SpecificFeed;

        Assert.AreEqual("Scripting News", feed.Title);
        Assert.AreEqual("http://www.scripting.com/", feed.Link);
        Assert.AreEqual("A weblog about scripting and stuff like that.", feed.Description);
        Assert.AreEqual("en-us", feed.Language);
        Assert.AreEqual("Copyright 1997-2002 Dave Winer", feed.Copyright);
        Assert.AreEqual("Mon, 30 Sep 2002 11:00:00 GMT", feed.LastBuildDateString);
        Assert.AreEqual("http://backend.userland.com/rss", feed.Docs);
        Assert.AreEqual("Radio UserLand v8.0.5", feed.Generator);
        Assert.AreEqual("1765", feed.Categories.First());
        Assert.AreEqual("dave@userland.com", feed.ManagingEditor);
        Assert.AreEqual("dave@userland.com", feed.WebMaster);
        Assert.AreEqual("40", feed.TTL);
        Assert.AreEqual(9, feed.Items.Count);

        var item = (Rss20FeedItem)feed.Items.Last();
        Assert.AreEqual("Really early morning no-coffee notes", item.Title);
        Assert.AreEqual("http://scriptingnews.userland.com/backissues/2002/09/29#reallyEarlyMorningNocoffeeNotes", item.Link);
        Assert.IsTrue(item.Description.Contains("<p>One of the lessons I've learned"));
        Assert.AreEqual("Sun, 29 Sep 2002 11:13:10 GMT", item.PublishingDateString);
        Assert.AreEqual(new DateTimeOffset(2002, 09, 29, 11, 13, 10, TimeSpan.Zero), item.PublishingDate);
        Assert.AreEqual("http://scriptingnews.userland.com/backissues/2002/09/29#reallyEarlyMorningNocoffeeNotes", item.Guid);
    }

    [TestMethod]
    public void TestRss20ParseCodeHollow()
    {
        var feed = (Rss20Feed)Feed.FromString(Samples.GetContent("Feeds/Rss20CodeHollowCom.xml")).SpecificFeed;

        Assert.AreEqual("codehollow", feed.Title);
        Assert.AreEqual("https://codehollow.com", feed.Link);
        Assert.AreEqual("Azure, software engineering/architecture, Scrum, SharePoint, VSTS/TFS, .NET and other funny things", feed.Description);
        Assert.AreEqual("Fri, 23 Dec 2016 09:01:55 +0000", feed.LastBuildDateString);
        Assert.AreEqual(new DateTimeOffset(2016, 12, 23, 09, 01, 55, TimeSpan.Zero), feed.LastBuildDate);
        Assert.AreEqual("en-US", feed.Language);
        Assert.AreEqual("hourly", feed.Sy.UpdatePeriod);
        Assert.AreEqual("1", feed.Sy.UpdateFrequency);
        Assert.AreEqual("https://wordpress.org/?v=4.7", feed.Generator);

        var item = (Rss20FeedItem)feed.Items.First();

        Assert.AreEqual("Export Azure RateCard data to CSV with C# and Billing API", item.Title);
        Assert.AreEqual("https://codehollow.com/2016/12/export-azure-ratecard-data-csv-csharp-billing-api/", item.Link);
        Assert.AreEqual("https://codehollow.com/2016/12/export-azure-ratecard-data-csv-csharp-billing-api/#respond", item.Comments);
        Assert.AreEqual("Thu, 22 Dec 2016 07:00:28 +0000", item.PublishingDateString);
        Assert.AreEqual(new DateTimeOffset(2016, 12, 22, 7, 0, 28, TimeSpan.Zero), item.PublishingDate);
        Assert.AreEqual("Armin Reiter", item.DC.Creator);
        Assert.AreEqual(4, item.Categories.Count);
        Assert.IsTrue(item.Categories.Contains("BillingAPI"));
        Assert.AreEqual("https://codehollow.com/?p=749", item.Guid);
        Assert.IsTrue(item.Description.StartsWith("<p>The Azure Billing API allows to programmatically read Azure"));
        Assert.IsTrue(item.Content.Contains("&lt;add key=&quot;Tenant&quot; "));

    }

    [TestMethod]
    public void TestRss20ParseContentWindGerman()
    {
        var feed = (Rss20Feed)Feed.FromString(Samples.GetContent("Feeds/Rss20ContentWindCom.xml")).SpecificFeed;
        Assert.AreEqual("ContentWind", feed.Title);
        Assert.AreEqual("http://content-wind.com", feed.Link);
        Assert.AreEqual("Do, 22 Dez 2016 17:36:00 +0000", feed.LastBuildDateString);
        Assert.AreEqual(new DateTimeOffset(2016, 12, 22, 17, 36, 0, TimeSpan.Zero), feed.LastBuildDate);
        Assert.AreEqual("de-DE", feed.Language);
        Assert.AreEqual("hourly", feed.Sy.UpdatePeriod);
        Assert.AreEqual("1", feed.Sy.UpdateFrequency);
        Assert.AreEqual("https://wordpress.org/?v=4.7", feed.Generator);

        var item = (Rss20FeedItem)feed.Items.First();
        Assert.AreEqual("Wachstum Influencer Marketing", item.Title);
        Assert.AreEqual("http://content-wind.com/2016/12/22/wachstum-influencer-marketing/", item.Link);
        Assert.AreEqual("http://content-wind.com/2016/12/22/wachstum-influencer-marketing/#respond", item.Comments);
        Assert.AreEqual("Thu, 22 Dec 2016 13:09:51 +0000", item.PublishingDateString);
        Assert.AreEqual(new DateTimeOffset(2016, 12, 22, 13, 09, 51, TimeSpan.Zero), item.PublishingDate);
        Assert.AreEqual("Harald Schaffernak", item.DC.Creator);

    }

    [TestMethod]
    public void TestRss20ParseMoscowTimes()
    {
        var feed = (Rss20Feed)Feed.FromString(Samples.GetContent("Feeds/Rss20MoscowTimes.xml")).SpecificFeed;
        Assert.AreEqual("The Moscow Times - News, Business, Culture & Multimedia from Russia", feed.Title);
        Assert.AreEqual("https://themoscowtimes.com/", feed.Link);
        Assert.AreEqual("The Moscow Times offers everything you need to know about Russia: Breaking news, top stories, business, analysis, opinion, multimedia, upcoming cultural events", feed.Description);
        Assert.AreEqual("en-us", feed.Language);
        Assert.AreEqual("Mon, 23 Jan 2017 16:45:02 +0000", feed.LastBuildDateString);
        Assert.AreEqual("600", feed.TTL);

        var item = (Rss20FeedItem)feed.Items.First();
        Assert.AreEqual("Russian State TV Praises Trump for Avoiding ‘Democracy’ in Inauguration Speech", item.Title);
        Assert.AreEqual("https://themoscowtimes.com/articles/russian-state-tv-praises-trump-for-avoiding-democracy-in-inauguration-speech-56901", item.Link);
        Assert.AreEqual("Though he welcomed the end of Obama’s presidency as the start of a bright new era, the Kremlin’s “chief propagandist” quickly found himself struggling to find convincing scapegoats for the world’s problems this week.", item.Description);
        Assert.AreEqual("Mon, 23 Jan 2017 16:45:02 +0000", item.PublishingDateString);
        Assert.AreEqual("https://themoscowtimes.com/articles/russian-state-tv-praises-trump-for-avoiding-democracy-in-inauguration-speech-56901", item.Guid);

        item = (Rss20FeedItem)feed.Items.Last();
        Assert.AreEqual("Don’t Say It", item.Title);
        Assert.AreEqual("https://themoscowtimes.com/articles/dont-say-it-56774", item.Link);
        Assert.AreEqual("They say “sex sells,” but don't go peddling it near dinner tables in Russia, where families in an ostensibly conservative society say the subject is too taboo to discuss at home.", item.Description);
        Assert.AreEqual("Tue, 10 Jan 2017 19:58:13 +0000", item.PublishingDateString);
        Assert.AreEqual("https://themoscowtimes.com/articles/dont-say-it-56774", item.Guid);
    }

    [TestMethod]
    public void TestRss20ParseSwedishFeedWithIso8859_1()
    {
        var feed = (Rss20Feed)Feed.FromSpan(Samples.GetBuffer("Feeds/Rss20ISO88591Intranet30.xml")).SpecificFeed;
        Assert.AreEqual("intranet30", feed.Title);
        Assert.AreEqual("http://www.retriever-info.com", feed.Link);
        Assert.AreEqual("RSS 2.0 News feed from Retriever Norge AS", feed.Description);

        var item = (Rss20FeedItem)feed.Items.First();
        Assert.AreEqual("SVART MÅNAD - DÖDSOLYCKA i Vetlanda", item.Title);
        Assert.AreEqual("https://www.retriever-info.com/go/?a=30338&d=00201120180819281555686&p=200108&s=2011&sa=2016177&u=http%3A%2F%2Fwww.hoglandsnytt.se%2Fsvart-manad-dodsolycka-i-vetlanda%2F&x=33d88e677ce6481d9882de22c76e4234", item.Link);
        Assert.AreEqual("Under juli 2018 omkom 39 personer och 1 521 skadades i vägtrafiken. Det visar de preliminära uppgifter som inkommit till Transportstyrelsen fram till den 15 augusti 2018. Det är åtta fler omkomna jämfört med juli månad 2017.", item.Description);
        Assert.AreEqual("Sun, 19 Aug 2018 07:14:00 GMT", item.PublishingDateString);
        Assert.AreEqual("00201120180819281555686", item.Guid);
        Assert.AreEqual("Höglandsnytt", item.Author);

    }

    [TestMethod]
    public void TestRss20CityDogKyrillicNoEncodingDefined()
    {
        var feed = Feed.FromString(Samples.GetContent("Feeds/Rss20CityDog.xml"));
        Assert.AreEqual("Новости - citydog.by", feed.Title);
        Assert.AreEqual("Последние обновления - citydog.by", feed.Description);

        var item = feed.Items.First();
        Assert.AreEqual("Группа «Серебряная свадьба» ушла в бессрочный отпуск", item.Title);
        Assert.AreEqual("http://citydog.by/post/zaden-serebrianaya-svadba-v-otpuske/", item.Id);
    }

    [TestMethod]
    public void TestAllFilesForException()
    {
        var linkless = new[] { "AtomBattleNet.xml" };

        foreach (var (name, content) in Samples.EnumerateContents("Feeds"))
        {
            var feed = Feed.FromString(content);
            if (!linkless.Any(name.EndsWith))
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(feed.Link));
            }

            TestItunesParsingForException(feed);
        }
    }

    [TestMethod]
    public async Task TestAtomParseAdobe_Async()
    {
        var feed = (AtomFeed)(await Feed.FromStreamAsync(Samples.GetStream("Feeds/AtomAdobe.xml"), default)).SpecificFeed;

        Assert.AreEqual("Adobe Blog", feed.Title);
        Assert.AreEqual(null, feed.Icon);
        Assert.AreEqual("https://blog.adobe.com/", feed.Link);
        Assert.AreEqual("2021-07-19T00:00:00.000Z", feed.UpdatedAsString);
        Assert.AreEqual("https://blog.adobe.com/", feed.Id);

        var item = (AtomFeedItem)feed.Items.First();
        Assert.AreEqual(null, item.Link); // The post href is store in the id element
    }

    [TestMethod]
    public async Task TestAtomParseTheVerge_Async()
    {
        var feed = (AtomFeed)(await Feed.FromStreamAsync(Samples.GetStream("Feeds/AtomTheVerge.xml"), default)).SpecificFeed;

        Assert.AreEqual("The Verge -  Front Pages", feed.Title);
        Assert.AreEqual("https://cdn2.vox-cdn.com/community_logos/34086/verge-fv.png", feed.Icon);
        Assert.AreEqual("2017-01-07T09:00:01-05:00", feed.UpdatedAsString);
        Assert.AreEqual(new DateTimeOffset(2017, 1, 7, 9, 0, 1, TimeSpan.FromHours(-5)), feed.Updated);
        Assert.AreEqual("http://www.theverge.com/rss/group/front-page/index.xml", feed.Id);

        var item = (AtomFeedItem)feed.Items.First();

        Assert.AreEqual("2017-01-07T09:00:01-05:00", item.UpdatedAsString);
        Assert.AreEqual(new DateTimeOffset(2017, 1, 7, 9, 0, 1, TimeSpan.FromHours(-5)), item.Updated);
        Assert.AreEqual("2017-01-07T09:00:01-05:00", item.PublishedAsString);
        Assert.AreEqual(new DateTimeOffset(2017, 1, 7, 9, 0, 1, TimeSpan.FromHours(-5)), item.Published);
        Assert.AreEqual("This is the new Hulu experience with live TV", item.Title);
        Assert.AreEqual("http://www.theverge.com/ces/2017/1/7/14195588/hulu-live-tv-streaming-internet-ces-2017", item.Id);
        Assert.AreEqual("http://www.theverge.com/ces/2017/1/7/14195588/hulu-live-tv-streaming-internet-ces-2017", item.Link);

        Assert.IsTrue(item.Content.Trim().StartsWith("<img alt=\"\""));

        Assert.AreEqual("Chris Welch", item.Author.Name);
    }

    [TestMethod]
    public async Task TestAtomYouTubeInvestmentPunk_Async()
    {
        var feed = (AtomFeed)(await Feed.FromStreamAsync(Samples.GetStream("Feeds/AtomYoutubeInvestmentPunk.xml"), default)).SpecificFeed;

        Assert.AreEqual("http://www.youtube.com/feeds/videos.xml?channel_id=UCmEN5ZnsHUXIxgpLitRTmWw", feed.Links.First().Href);
        Assert.AreEqual("yt:channel:UCmEN5ZnsHUXIxgpLitRTmWw", feed.Id);
        Assert.AreEqual("Investment Punk Academy by Gerald Hörhan", feed.Title);
        Assert.AreEqual("http://www.youtube.com/channel/UCmEN5ZnsHUXIxgpLitRTmWw", feed.Links.ElementAt(1).Href);
        Assert.AreEqual("Investment Punk Academy by Gerald Hörhan", feed.Author.Name);
        Assert.AreEqual("http://www.youtube.com/channel/UCmEN5ZnsHUXIxgpLitRTmWw", feed.Author.Uri);
        var item = (AtomFeedItem)feed.Items.First();
        Assert.AreEqual("yt:video:AFA8ZtMwrvc", item.Id);
        Assert.AreEqual("Zukunft von Vertretern I Kernfusion I Musikgeschäft #ASKTHEPUNK 71", item.Title);
        Assert.AreEqual("alternate", item.Links.First().Relation);
        Assert.AreEqual("2017-01-23T18:14:49+00:00", item.UpdatedAsString);
        Assert.AreEqual("2017-01-20T16:00:00+00:00", item.PublishedAsString);
    }

    [TestMethod]
    public async Task TestRss091ParseStadtFWeiz_Async()
    {
        var feed = (Rss091Feed)(await Feed.FromStreamAsync(Samples.GetStream("Feeds/Rss091Stadtfeuerwehr.xml"), default)).SpecificFeed;

        Assert.AreEqual("Stadtfeuerwehr Weiz - Einsätze", feed.Title);
        Assert.AreEqual("http://www.stadtfeuerwehr-weiz.at", feed.Link);
        Assert.AreEqual("Die letzten 15 Einsätze der Stadtfeuerwehr Weiz.", feed.Description);
        Assert.AreEqual("de-de", feed.Language);
        Assert.AreEqual("Stadtfeuerwehr Weiz / Markus Horwath", feed.Copyright);

        var item = (Rss091FeedItem)feed.Items.First();

        Assert.AreEqual(@"[19.08.2018 - 07:08 Uhr] Brandmeldeanlagenalarm", item.Title.Trim());
        Assert.IsTrue(item.Description.Contains("Weitere Informationen"));
        Assert.AreEqual("http://www.stadtfeuerwehr-weiz.at/einsaetze/einsatz-detail/5220/", item.Link);
        Assert.AreEqual("Sun, 19 Aug 2018 07:08:00 +0100", item.PublishingDateString);
        Assert.AreEqual(new DateTimeOffset(2018, 8, 19, 7, 08, 0, TimeSpan.FromHours(1)), item.PublishingDate);

        Assert.AreEqual(15, feed.Items.Count);
    }

    [TestMethod]
    public async Task TestRss091ParseFullSample_Async()
    {
        var feed = (Rss091Feed)(await Feed.FromStreamAsync(Samples.GetStream("Feeds/Rss091FullSample.xml"), default)).SpecificFeed;
        Assert.AreEqual("Copyright 1997-1999 UserLand Software, Inc.", feed.Copyright);
        Assert.AreEqual("Thu, 08 Jul 1999 07:00:00 GMT", feed.PublishingDateString);
        Assert.AreEqual("Thu, 08 Jul 1999 16:20:26 GMT", feed.LastBuildDateString);
        Assert.AreEqual("http://my.userland.com/stories/storyReader$11", feed.Docs);
        Assert.AreEqual("News and commentary from the cross-platform scripting community.", feed.Description);
        Assert.AreEqual("http://www.scripting.com/", feed.Link);
        Assert.AreEqual("Scripting News", feed.Title);
        Assert.AreEqual("http://www.scripting.com/", feed.Image.Link);
        Assert.AreEqual("Scripting News", feed.Image.Title);
        Assert.AreEqual("http://www.scripting.com/gifs/tinyScriptingNews.gif", feed.Image.Url);
        Assert.AreEqual(40, ((Rss091FeedImage)feed.Image).Height);
        Assert.AreEqual(78, ((Rss091FeedImage)feed.Image).Width);
        Assert.AreEqual("What is this used for?", ((Rss091FeedImage)feed.Image).Description);
        Assert.AreEqual("dave@userland.com (Dave Winer)", feed.ManagingEditor);
        Assert.AreEqual("dave@userland.com (Dave Winer)", feed.WebMaster);
        Assert.AreEqual("en-us", feed.Language);
        Assert.IsTrue(feed.SkipHours.Contains("6"));
        Assert.IsTrue(feed.SkipHours.Contains("7"));
        Assert.IsTrue(feed.SkipHours.Contains("8"));
        Assert.IsTrue(feed.SkipHours.Contains("9"));
        Assert.IsTrue(feed.SkipHours.Contains("10"));
        Assert.IsTrue(feed.SkipHours.Contains("11"));
        Assert.IsTrue(feed.SkipDays.Contains("Sunday"));
        Assert.AreEqual("(PICS-1.1 \"http://www.rsac.org/ratingsv01.html\" l gen true comment \"RSACi North America Server\" for \"http://www.rsac.org\" on \"1996.04.16T08:15-0500\" r (n 0 s 0 v 0 l 0))", feed.Rating);

        Assert.AreEqual(1, feed.Items.Count);
        var item = (Rss091FeedItem)feed.Items.First();
        Assert.AreEqual("stuff", item.Title);
        Assert.AreEqual("http://bar", item.Link);
        Assert.AreEqual("This is an article about some stuff", item.Description);

        Assert.AreEqual("Search Now!", feed.TextInput.Title);
        Assert.AreEqual("Enter your search terms", feed.TextInput.Description);
        Assert.AreEqual("find", feed.TextInput.Name);
        Assert.AreEqual("http://my.site.com/search.cgi", feed.TextInput.Link);
    }

    [TestMethod]
    public async Task TestRss092ParseFullSample_Async()
    {
        var feed = (Rss092Feed)(await Feed.FromStreamAsync(Samples.GetStream("Feeds/Rss092FullSample.xml"), default)).SpecificFeed;

        Assert.AreEqual("Dave Winer: Grateful Dead", feed.Title);
        Assert.AreEqual("http://www.scripting.com/blog/categories/gratefulDead.html", feed.Link);
        Assert.AreEqual("A high-fidelity Grateful Dead song every day. This is where we're experimenting with enclosures on RSS news items that download when you're not using your computer. If it works (it will) it will be the end of the Click-And-Wait multimedia experience on the Internet. ", feed.Description);
        Assert.AreEqual("Fri, 13 Apr 2001 19:23:02 GMT", feed.LastBuildDateString);
        Assert.AreEqual("http://backend.userland.com/rss092", feed.Docs);
        Assert.AreEqual("dave@userland.com (Dave Winer)", feed.ManagingEditor);
        Assert.AreEqual("dave@userland.com (Dave Winer)", feed.WebMaster);
        Assert.AreEqual("data.ourfavoritesongs.com", feed.Cloud.Domain);
        Assert.AreEqual("80", feed.Cloud.Port);
        Assert.AreEqual("/RPC2", feed.Cloud.Path);
        Assert.AreEqual("ourFavoriteSongs.rssPleaseNotify", feed.Cloud.RegisterProcedure);
        Assert.AreEqual("xml-rpc", feed.Cloud.Protocol);

        Assert.AreEqual(22, feed.Items.Count);
        var item = (Rss092FeedItem)feed.Items.ElementAt(20);
        Assert.AreEqual("A touch of gray, kinda suits you anyway..", item.Description);
        Assert.AreEqual("http://www.scripting.com/mp3s/touchOfGrey.mp3", item.Enclosure.Url);
        Assert.AreEqual(5588242, item.Enclosure.Length);
        Assert.AreEqual("audio/mpeg", item.Enclosure.MediaType);

        var secondItem = (Rss092FeedItem)feed.Items.ElementAt(1);
        Assert.AreEqual("http://scriptingnews.userland.com/xml/scriptingNews2.xml", secondItem.Source.Url);
        Assert.AreEqual("Scripting News", secondItem.Source.Value);
    }

    [TestMethod]
    public async Task TestRss10ParseFullSample_Async()
    {
        var feed = (Rss10Feed)(await Feed.FromStreamAsync(Samples.GetStream("Feeds/Rss10FeedWebResourceSample.xml"), default)).SpecificFeed;

        Assert.AreEqual("XML.com", feed.Title);
        Assert.AreEqual("http://xml.com/pub", feed.Link);
        Assert.AreEqual("\n      XML.com features a rich mix of information and services\n      for the XML community.\n    ", feed.Description);
        var image = (Rss10FeedImage)feed.Image;
        Assert.AreEqual("http://xml.com/universal/images/xml_tiny.gif", image.About);
        Assert.AreEqual("XML.com", image.Title);
        Assert.AreEqual("http://www.xml.com", image.Link);
        Assert.AreEqual("http://xml.com/universal/images/xml_tiny.gif", image.Url);
        Assert.AreEqual("Search XML.com", feed.TextInput.Title);
        Assert.AreEqual("Search XML.com's XML collection", feed.TextInput.Description);
        Assert.AreEqual("s", feed.TextInput.Name);
        Assert.AreEqual("http://search.xml.com", ((Rss10FeedTextInput)feed.TextInput).About);
        Assert.AreEqual("http://search.xml.com", feed.TextInput.Link);

        var item = (Rss10FeedItem)feed.Items.Last();

        Assert.AreEqual("http://xml.com/pub/2000/08/09/rdfdb/index.html", item.About);
        Assert.AreEqual("Putting RDF to Work", item.Title);
        Assert.AreEqual("http://xml.com/pub/2000/08/09/rdfdb/index.html", item.Link);
        Assert.AreEqual(186, item.Description.Length);
    }

    [TestMethod]
    public async Task TestRss10ParseOrfAt_Async()
    {
        var feed = (Rss10Feed)(await Feed.FromStreamAsync(Samples.GetStream("Feeds/Rss10OrfAt.xml"), default)).SpecificFeed;
        Assert.AreEqual("news.ORF.at", feed.Title);
        Assert.AreEqual("http://orf.at/", feed.Link);
        Assert.AreEqual("2017-01-23T21:54:55+01:00", feed.DC.DateString);
        Assert.AreEqual("Die aktuellsten Nachrichten auf einen Blick - aus Österreich und der ganzen Welt. In Text, Bild und Video.", feed.Description);
        Assert.AreEqual("ORF Österreichischer Rundfunk, Wien", feed.DC.Publisher);
        Assert.AreEqual("ORF Online und Teletext GmbH & Co KG", feed.DC.Creator);
        Assert.AreEqual("de", feed.DC.Language);
        Assert.AreEqual("Copyright © 2017 ORF Online und Teletext GmbH & Co KG", feed.DC.Rights);
        Assert.AreEqual("hourly", feed.Sy.UpdatePeriod);
        Assert.AreEqual("2", feed.Sy.UpdateFrequency);
        Assert.AreEqual("2000-01-01T00:00:00Z", feed.Sy.UpdateBase);
        Assert.AreEqual(50, feed.Items.Count);

        var item = (Rss10FeedItem)feed.Items.ElementAt(4);

        Assert.AreEqual("Feldsperling erstmals häufigster Vogel", item.Title);
        Assert.AreEqual("http://orf.at/stories/2376365/", item.Link);
        Assert.AreEqual("Chronik", item.DC.Subject);
        Assert.AreEqual("2017-01-23T20:51:06+01:00", item.DC.DateString);
    }

    [TestMethod]
    public async Task TestRss20ParseWebResourceSampleFull_Async()
    {
        var feed = (Rss20Feed)(await Feed.FromStreamAsync(Samples.GetStream("Feeds/Rss20FeedWebResourceSample.xml"), default)).SpecificFeed;

        Assert.AreEqual("Scripting News", feed.Title);
        Assert.AreEqual("http://www.scripting.com/", feed.Link);
        Assert.AreEqual("A weblog about scripting and stuff like that.", feed.Description);
        Assert.AreEqual("en-us", feed.Language);
        Assert.AreEqual("Copyright 1997-2002 Dave Winer", feed.Copyright);
        Assert.AreEqual("Mon, 30 Sep 2002 11:00:00 GMT", feed.LastBuildDateString);
        Assert.AreEqual("http://backend.userland.com/rss", feed.Docs);
        Assert.AreEqual("Radio UserLand v8.0.5", feed.Generator);
        Assert.AreEqual("1765", feed.Categories.First());
        Assert.AreEqual("dave@userland.com", feed.ManagingEditor);
        Assert.AreEqual("dave@userland.com", feed.WebMaster);
        Assert.AreEqual("40", feed.TTL);
        Assert.AreEqual(9, feed.Items.Count);

        var item = (Rss20FeedItem)feed.Items.Last();
        Assert.AreEqual("Really early morning no-coffee notes", item.Title);
        Assert.AreEqual("http://scriptingnews.userland.com/backissues/2002/09/29#reallyEarlyMorningNocoffeeNotes", item.Link);
        Assert.IsTrue(item.Description.Contains("<p>One of the lessons I've learned"));
        Assert.AreEqual("Sun, 29 Sep 2002 11:13:10 GMT", item.PublishingDateString);
        Assert.AreEqual(new DateTimeOffset(2002, 09, 29, 11, 13, 10, TimeSpan.Zero), item.PublishingDate);
        Assert.AreEqual("http://scriptingnews.userland.com/backissues/2002/09/29#reallyEarlyMorningNocoffeeNotes", item.Guid);
    }

    [TestMethod]
    public async Task TestRss20ParseCodeHollow_Async()
    {
        var feed = (Rss20Feed)(await Feed.FromStreamAsync(Samples.GetStream("Feeds/Rss20CodeHollowCom.xml"), default)).SpecificFeed;

        Assert.AreEqual("codehollow", feed.Title);
        Assert.AreEqual("https://codehollow.com", feed.Link);
        Assert.AreEqual("Azure, software engineering/architecture, Scrum, SharePoint, VSTS/TFS, .NET and other funny things", feed.Description);
        Assert.AreEqual("Fri, 23 Dec 2016 09:01:55 +0000", feed.LastBuildDateString);
        Assert.AreEqual(new DateTimeOffset(2016, 12, 23, 09, 01, 55, TimeSpan.Zero), feed.LastBuildDate);
        Assert.AreEqual("en-US", feed.Language);
        Assert.AreEqual("hourly", feed.Sy.UpdatePeriod);
        Assert.AreEqual("1", feed.Sy.UpdateFrequency);
        Assert.AreEqual("https://wordpress.org/?v=4.7", feed.Generator);

        var item = (Rss20FeedItem)feed.Items.First();

        Assert.AreEqual("Export Azure RateCard data to CSV with C# and Billing API", item.Title);
        Assert.AreEqual("https://codehollow.com/2016/12/export-azure-ratecard-data-csv-csharp-billing-api/", item.Link);
        Assert.AreEqual("https://codehollow.com/2016/12/export-azure-ratecard-data-csv-csharp-billing-api/#respond", item.Comments);
        Assert.AreEqual("Thu, 22 Dec 2016 07:00:28 +0000", item.PublishingDateString);
        Assert.AreEqual(new DateTimeOffset(2016, 12, 22, 7, 0, 28, TimeSpan.Zero), item.PublishingDate);
        Assert.AreEqual("Armin Reiter", item.DC.Creator);
        Assert.AreEqual(4, item.Categories.Count);
        Assert.IsTrue(item.Categories.Contains("BillingAPI"));
        Assert.AreEqual("https://codehollow.com/?p=749", item.Guid);
        Assert.IsTrue(item.Description.StartsWith("<p>The Azure Billing API allows to programmatically read Azure"));
        Assert.IsTrue(item.Content.Contains("&lt;add key=&quot;Tenant&quot; "));

    }

    [TestMethod]
    public async Task TestRss20ParseContentWindGerman_Async()
    {
        var feed = (Rss20Feed)(await Feed.FromStreamAsync(Samples.GetStream("Feeds/Rss20ContentWindCom.xml"), default)).SpecificFeed;
        Assert.AreEqual("ContentWind", feed.Title);
        Assert.AreEqual("http://content-wind.com", feed.Link);
        Assert.AreEqual("Do, 22 Dez 2016 17:36:00 +0000", feed.LastBuildDateString);
        Assert.AreEqual(new DateTimeOffset(2016, 12, 22, 17, 36, 00, TimeSpan.Zero), feed.LastBuildDate);
        Assert.AreEqual("de-DE", feed.Language);
        Assert.AreEqual("hourly", feed.Sy.UpdatePeriod);
        Assert.AreEqual("1", feed.Sy.UpdateFrequency);
        Assert.AreEqual("https://wordpress.org/?v=4.7", feed.Generator);

        var item = (Rss20FeedItem)feed.Items.First();
        Assert.AreEqual("Wachstum Influencer Marketing", item.Title);
        Assert.AreEqual("http://content-wind.com/2016/12/22/wachstum-influencer-marketing/", item.Link);
        Assert.AreEqual("http://content-wind.com/2016/12/22/wachstum-influencer-marketing/#respond", item.Comments);
        Assert.AreEqual("Thu, 22 Dec 2016 13:09:51 +0000", item.PublishingDateString);
        Assert.AreEqual(new DateTimeOffset(2016, 12, 22, 13, 09, 51, TimeSpan.Zero), item.PublishingDate);
        Assert.AreEqual("Harald Schaffernak", item.DC.Creator);

    }

    [TestMethod]
    public async Task TestRss20ParseMoscowTimes_Async()
    {
        var feed = (Rss20Feed)(await Feed.FromStreamAsync(Samples.GetStream("Feeds/Rss20MoscowTimes.xml"), default)).SpecificFeed;
        Assert.AreEqual("The Moscow Times - News, Business, Culture & Multimedia from Russia", feed.Title);
        Assert.AreEqual("https://themoscowtimes.com/", feed.Link);
        Assert.AreEqual("The Moscow Times offers everything you need to know about Russia: Breaking news, top stories, business, analysis, opinion, multimedia, upcoming cultural events", feed.Description);
        Assert.AreEqual("en-us", feed.Language);
        Assert.AreEqual("Mon, 23 Jan 2017 16:45:02 +0000", feed.LastBuildDateString);
        Assert.AreEqual("600", feed.TTL);

        var item = (Rss20FeedItem)feed.Items.First();
        Assert.AreEqual("Russian State TV Praises Trump for Avoiding ‘Democracy’ in Inauguration Speech", item.Title);
        Assert.AreEqual("https://themoscowtimes.com/articles/russian-state-tv-praises-trump-for-avoiding-democracy-in-inauguration-speech-56901", item.Link);
        Assert.AreEqual("Though he welcomed the end of Obama’s presidency as the start of a bright new era, the Kremlin’s “chief propagandist” quickly found himself struggling to find convincing scapegoats for the world’s problems this week.", item.Description);
        Assert.AreEqual("Mon, 23 Jan 2017 16:45:02 +0000", item.PublishingDateString);
        Assert.AreEqual("https://themoscowtimes.com/articles/russian-state-tv-praises-trump-for-avoiding-democracy-in-inauguration-speech-56901", item.Guid);

        item = (Rss20FeedItem)feed.Items.Last();
        Assert.AreEqual("Don’t Say It", item.Title);
        Assert.AreEqual("https://themoscowtimes.com/articles/dont-say-it-56774", item.Link);
        Assert.AreEqual("They say “sex sells,” but don't go peddling it near dinner tables in Russia, where families in an ostensibly conservative society say the subject is too taboo to discuss at home.", item.Description);
        Assert.AreEqual("Tue, 10 Jan 2017 19:58:13 +0000", item.PublishingDateString);
        Assert.AreEqual("https://themoscowtimes.com/articles/dont-say-it-56774", item.Guid);
    }

    [TestMethod]
    public async Task TestRss20ParseSwedishFeedWithIso8859_1_Async()
    {
        var feed = (Rss20Feed)(await Feed.FromStreamAsync(Samples.GetStream("Feeds/Rss20ISO88591Intranet30.xml"), default)).SpecificFeed;
        Assert.AreEqual("intranet30", feed.Title);
        Assert.AreEqual("http://www.retriever-info.com", feed.Link);
        Assert.AreEqual("RSS 2.0 News feed from Retriever Norge AS", feed.Description);

        var item = (Rss20FeedItem)feed.Items.First();
        Assert.AreEqual("SVART MÅNAD - DÖDSOLYCKA i Vetlanda", item.Title);
        Assert.AreEqual("https://www.retriever-info.com/go/?a=30338&d=00201120180819281555686&p=200108&s=2011&sa=2016177&u=http%3A%2F%2Fwww.hoglandsnytt.se%2Fsvart-manad-dodsolycka-i-vetlanda%2F&x=33d88e677ce6481d9882de22c76e4234", item.Link);
        Assert.AreEqual("Under juli 2018 omkom 39 personer och 1 521 skadades i vägtrafiken. Det visar de preliminära uppgifter som inkommit till Transportstyrelsen fram till den 15 augusti 2018. Det är åtta fler omkomna jämfört med juli månad 2017.", item.Description);
        Assert.AreEqual("Sun, 19 Aug 2018 07:14:00 GMT", item.PublishingDateString);
        Assert.AreEqual("00201120180819281555686", item.Guid);
        Assert.AreEqual("Höglandsnytt", item.Author);

    }

    [TestMethod]
    public async Task TestRss20CityDogKyrillicNoEncodingDefined_Async()
    {
        var feed = await Feed.FromStreamAsync(Samples.GetStream("Feeds/Rss20CityDog.xml"), default);
        Assert.AreEqual("Новости - citydog.by", feed.Title);
        Assert.AreEqual("Последние обновления - citydog.by", feed.Description);

        var item = feed.Items.First();
        Assert.AreEqual("Группа «Серебряная свадьба» ушла в бессрочный отпуск", item.Title);
        Assert.AreEqual("http://citydog.by/post/zaden-serebrianaya-svadba-v-otpuske/", item.Id);
    }

    [TestMethod]
    public async Task TestAllFilesForException_Async()
    {
        var linkless = new[] { "AtomBattleNet.xml" };

        foreach (var (name, stream) in Samples.EnumerateStreams("Feeds"))
        {
            var feed = await Feed.FromStreamAsync(stream, default);
            if (!linkless.Any(name.EndsWith))
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(feed.Link));
            }

            TestItunesParsingForException(feed);
        }
    }

    private static void TestItunesParsingForException(Feed feed)
    {
        Assert.IsNotNull(feed.GetItunesChannel());

        foreach (var item in feed.Items)
        {
            Assert.IsNotNull(item.GetItunesItem());
        }
    }
}
