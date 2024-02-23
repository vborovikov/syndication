namespace Syndication.Feeds.Itunes
{
    using Brackets;

    /// <summary>
    /// The itunes:image xml element
    /// </summary>
    public record ItunesImage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItunesImage"/> class.
        /// </summary>
        /// <param name="image">The itunes:image element</param>
        public ItunesImage(Tag image)
        {
            this.Href = image.GetAttributeValue("href");
        }

        /// <summary>
        /// The value of the href attribute
        /// </summary>
        public string Href { get; }
    }
}
