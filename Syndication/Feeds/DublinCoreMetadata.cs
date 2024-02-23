namespace Syndication.Feeds
{
    using System;
    using Brackets;
    using Syndication;

    /// <summary>
    /// The parsed "dc:" elements in a feed
    /// </summary>
    public record DublinCoreMetadata
    {
        /// <summary>
        /// The "title" element
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// The "creator" element
        /// </summary>
        public string Creator { get; }

        /// <summary>
        /// The "subject" element
        /// </summary>
        public string Subject { get; }

        /// <summary>
        /// The "description" field
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The "publisher" element
        /// </summary>
        public string Publisher { get; }

        /// <summary>
        /// The "contributor" element
        /// </summary>
        public string Contributor { get; }

        /// <summary>
        /// The "date" element
        /// </summary>
        public string DateString { get; }

        /// <summary>
        /// The "date" element as datetime. Null if parsing failed or date is empty.
        /// </summary>
        public DateTime? Date { get; }

        /// <summary>
        /// The "type" element
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// The "format" element
        /// </summary>
        public string Format { get; }

        /// <summary>
        /// The "identifier" element
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// The "source" element
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// The "language" element
        /// </summary>
        public string Language { get; }

        /// <summary>
        /// The "rel" element
        /// </summary>
        public string Relation { get; }

        /// <summary>
        /// The "coverage" element
        /// </summary>
        public string Coverage { get; }

        /// <summary>
        /// The "rights" element
        /// </summary>
        public string Rights { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DublinCoreMetadata"/> class.
        /// Reads a dublincore (dc:) element based on the xml given in element
        /// </summary>
        /// <param name="item">item element as xml</param>
        public DublinCoreMetadata(ParentTag item)
        {
            this.Title = item.GetValue("dc:title");
            this.Creator = item.GetValue("dc:creator");
            this.Subject = item.GetValue("dc:subject");
            this.Description = item.GetValue("dc:description");
            this.Publisher = item.GetValue("dc:publisher");
            this.Contributor = item.GetValue("dc:contributor");
            this.DateString = item.GetValue("dc:date");
            this.Date = Helpers.TryParseDateTime(this.DateString);
            this.Type = item.GetValue("dc:type");
            this.Format = item.GetValue("dc:format");
            this.Identifier = item.GetValue("dc:identifier");
            this.Source = item.GetValue("dc:source");
            this.Language = item.GetValue("dc:language");
            this.Relation = item.GetValue("dc:relation");
            this.Coverage = item.GetValue("dc:coverage");
            this.Rights = item.GetValue("dc:rights");
        }
    }
}
