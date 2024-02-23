namespace Syndication.Feeds.Itunes
{
    using System;
    using System.Linq;
    using Brackets;

    /// <summary>
    /// The basic itunes: elements that are part of the channel xml element of an rss2.0 feed
    /// </summary>
    public record ItunesChannel
    {
        internal const string NAMESPACEPREFIX = "itunes";
        /// <summary>
        /// Initializes a new instance of the <see cref="ItunesChannel"/> class.
        /// </summary>
        /// <param name="channelElement"></param>
        public ItunesChannel(ParentTag channelElement)
        {
            Author = channelElement.GetValue("itunes:author");
            Block = channelElement.GetValue("itunes:block").EqualsIgnoreCase("yes");
            Categories = GetItunesCategories(channelElement);

            var imageElement = channelElement.Tag("itunes:image");
            if (imageElement != null)
            {
                Image = new ItunesImage(imageElement);
            }

            var explicitValue = channelElement.GetValue("itunes:explicit");
            Explicit = explicitValue.EqualsIgnoreCase("yes", "explicit", "true");
            
            Complete = channelElement.GetValue("itunes:complete").EqualsIgnoreCase("yes");

            if (Uri.TryCreate(channelElement.GetValue("itunes:new-feed-url"), UriKind.Absolute, out var newFeedUrl))
            {
                NewFeedUrl = newFeedUrl;
            }

            var ownerElement = channelElement.GetElement("itunes:owner");

            if (ownerElement != null)
            {
                Owner = new ItunesOwner(ownerElement);
            }

            Subtitle = channelElement.GetValue("itunes:subtitle");
            Summary = channelElement.GetValue("itunes:summary");
        }

        /// <summary>
        /// The itunes:author element
        /// </summary>
        public string Author { get; }

        /// <summary>
        /// The itunes:block element
        /// </summary>
        public bool Block { get; }

        /// <summary>
        /// The itunes:category elements
        /// </summary>
        public ItunesCategory[] Categories { get; }

        /// <summary>
        /// The itunes:image element
        /// </summary>
        public ItunesImage Image { get; }

        /// <summary>
        /// The itunes:explicit element
        /// </summary>
        public bool Explicit { get; }

        /// <summary>
        /// The itunes:complete element
        /// </summary>
        public bool Complete { get; }

        /// <summary>
        /// The itunes:new-feed-url element
        /// </summary>
        public Uri NewFeedUrl { get; }

        /// <summary>
        /// The itunes:owner element
        /// </summary>
        public ItunesOwner Owner { get; }

        /// <summary>
        /// The itunes:subtitle element
        /// </summary>
        public string Subtitle { get; }

        /// <summary>
        /// The itunes:summary element
        /// </summary>
        public string Summary { get; }

        /// <summary>
        /// Sets the itunes categories
        /// </summary>
        /// <param name="element">the channel element</param>
        /// <returns>the itunes:categries</returns>
        private ItunesCategory[] GetItunesCategories(Tag element)
        {
            if (element is not ParentTag parent)
                return Array.Empty<ItunesCategory>();

            var categoryElements = parent.GetTags("itunes:category");
            var query = from categoryElement in categoryElements
                let children = GetItunesCategories(categoryElement)
                select new ItunesCategory(categoryElement.GetAttributeValue("text"), children);

            return query.ToArray();
        }
    }
}
