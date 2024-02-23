namespace Syndication.Feeds
{
    using Brackets;

    /// <summary>
    /// Rss 1.0 Feed textinput according to specification: http://web.resource.org/rss/1.0/spec
    /// </summary>
    public record Rss10FeedTextInput : FeedTextInput
    {
        /// <summary>
        /// The "about" attribute of the element
        /// </summary>
        public string About { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rss10FeedTextInput"/> class.
        /// Reads a rss 1.0 textInput element based on the xml given in item
        /// </summary>
        /// <param name="element">about element as xml</param>
        public Rss10FeedTextInput(ParentTag element)
            : base(element)
        {
            this.About = element.GetAttributeValue("rdf:about");
        }
    }
}
