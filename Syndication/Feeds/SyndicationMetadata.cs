namespace Syndication.Feeds
{
    using Brackets;
    using Syndication;

    /// <summary>
    /// The parsed syndication elements. Those are all elements that start with "sy:"
    /// </summary>
    public record SyndicationMetadata
    {
        /// <summary>
        /// The "updatePeriod" element
        /// </summary>
        public string UpdatePeriod { get; }

        /// <summary>
        /// The "updateFrequency" element
        /// </summary>
        public string UpdateFrequency { get; }

        /// <summary>
        /// The "updateBase" element
        /// </summary>
        public string UpdateBase { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyndicationMetadata"/> class.
        /// Creates the object based on the xml in the given XElement
        /// </summary>
        /// <param name="xelement">syndication element as xml</param>
        public SyndicationMetadata(ParentTag xelement)
        {
            this.UpdateBase = xelement.GetValue("sy:updateBase");
            this.UpdateFrequency = xelement.GetValue("sy:updateFrequency");
            this.UpdatePeriod = xelement.GetValue("sy:updatePeriod");
        }
    }
}
