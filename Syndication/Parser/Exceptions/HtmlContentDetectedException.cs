namespace CodeHollow.FeedReader.Parser;

using System;
using System.Collections.Generic;

public class HtmlContentDetectedException : Exception
{
    public HtmlContentDetectedException()
        : this("") { }

    public HtmlContentDetectedException(string message)
        : this(message, innerException: null) { }

    public HtmlContentDetectedException(string message, IEnumerable<HtmlFeedLink> feedLinks)
        : this(message, null, feedLinks) { }

    public HtmlContentDetectedException(string message, Exception innerException)
        : this(message, innerException, Array.Empty<HtmlFeedLink>()) { }

    public HtmlContentDetectedException(string message, Exception innerException, IEnumerable<HtmlFeedLink> feedLinks)
        : base(message, innerException)
    {
        this.FeedLinks = feedLinks;
        foreach (var link in feedLinks)
        {
            this.Data.Add(link.Title, link.Url);
        }
    }

    public IEnumerable<HtmlFeedLink> FeedLinks { get; }
}