namespace CodeHollow.FeedReader.Feeds.Itunes
{
    using Brackets;

    /// <summary>
    /// The itunes:owner xml element
    /// </summary>
    public record ItunesOwner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItunesOwner"/> class.
        /// </summary>
        /// <param name="ownerElement">the owner xml element</param>
        public ItunesOwner(ParentTag ownerElement)
        {
            this.Name = ownerElement.GetValue("itunes:name");
            this.Email = ownerElement.GetValue("itunes:email");
        }

        /// <summary>
        /// The itunes:email of the owner
        /// </summary>
        public string Email { get; }

        /// <summary>
        /// The itunes:name of the owner
        /// </summary>
        public string Name { get; }
    }
}
