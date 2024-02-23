using Brackets;

namespace CodeHollow.FeedReader.Feeds.MediaRSS
{
    /// <summary>
    /// Allows particular images to be used as representative images for the media object. If multiple thumbnails are included, and time coding is not at play, it is assumed that the images are in order of importance. 
    /// </summary>
    public record Thumbnail
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Thumbnail"/> class.
        /// Reads a rss feed item enclosure based on the xml given in element
        /// </summary>
        /// <param name="element">enclosure element as xml</param>
        public Thumbnail(Tag element)
        {
            this.Url = element.GetAttributeValue("url");
            this.Height = Helpers.TryParseInt(element.GetAttributeValue("height"));
            this.Width = Helpers.TryParseInt(element.GetAttributeValue("width"));
            this.Time = element.GetAttributeValue("time");
        }

        /// <summary>
        /// The url of the thumbnail. Required attribute
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// Height of the object media. Optional attribute
        /// </summary>
        public int? Height { get; }

        /// <summary>
        /// Width of the object media. Optional attribute
        /// </summary>
        public int? Width { get; }

        /// <summary>
        /// Specifies the time offset in relation to the media object
        /// </summary>
        public string Time { get; }
    }
}
