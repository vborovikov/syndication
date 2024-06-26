﻿namespace Syndication.Feeds;

using Brackets;

/// <summary>
/// Rss 0.92 feed according to specification: http://backend.userland.com/rss092
/// </summary>
public record Rss092Feed : Rss091Feed
{
    /// <summary>
    /// The "cloud" field
    /// </summary>
    public FeedCloud? Cloud { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Rss092Feed"/> class.
    /// Reads a rss 0.92 feed based on the xml given in channel
    /// </summary>
    /// <param name="feedXml">the entire feed xml as string</param>
    /// <param name="channel">the "channel" element in the xml as XElement</param>
    public Rss092Feed(string feedXml, ParentTag channel)
        : base(feedXml, channel)
    {
        if (channel.Tag("cloud") is Tag cloud)
        {
            this.Cloud = new FeedCloud(cloud);
        }
    }

    private protected override BaseFeedItem CreateItem(ParentTag item) => new Rss092FeedItem(item);

    /// <summary>
    /// Creates the base <see cref="Feed"/> element out of this feed.
    /// </summary>
    /// <returns>feed</returns>
    public override Feed ToFeed()
    {
        var feed = base.ToFeed();
        feed.Type = FeedType.Rss_0_92;
        return feed;
    }
}
