﻿namespace Syndication.Feeds
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Brackets;

    /// <summary>
    /// Media RSS 2.0 feed according to specification: http://www.rssboard.org/media-rss
    /// </summary>
    public record MediaRssFeed : BaseFeed
    {
        /// <summary>
        /// The "description" element
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The "language" element
        /// </summary>
        public string Language { get; }

        /// <summary>
        /// The "copyright" element
        /// </summary>
        public string Copyright { get; }

        /// <summary>
        /// The "docs" element
        /// </summary>
        public string Docs { get; }

        /// <summary>
        /// The "image" element
        /// </summary>
        public FeedImage Image { get; }

        /// <summary>
        /// The "lastBuildDate" element as string
        /// </summary>
        public string LastBuildDateString { get; }

        /// <summary>
        /// The "lastBuildDate" element as DateTime. Null if parsing failed of lastBuildDate is empty.
        /// </summary>
        public DateTimeOffset? LastBuildDate { get; private set; }

        /// <summary>
        /// The "managingEditor" element
        /// </summary>
        public string ManagingEditor { get; }

        /// <summary>
        /// The "pubDate" field
        /// </summary>
        public string PublishingDateString { get; }

        /// <summary>
        /// The "pubDate" field as DateTime. Null if parsing failed or pubDate is empty.
        /// </summary>
        public DateTimeOffset? PublishingDate { get; private set; }

        /// <summary>
        /// The "webMaster" field
        /// </summary>
        public string WebMaster { get; }

        /// <summary>
        /// All "category" elements
        /// </summary>
        public IReadOnlyCollection<string> Categories { get; } = []; // category

        /// <summary>
        /// The "generator" element
        /// </summary>
        public string Generator { get; }

        /// <summary>
        /// The "cloud" element
        /// </summary>
        public FeedCloud Cloud { get; }

        /// <summary>
        /// The time to life "ttl" element
        /// </summary>
        public string TTL { get; }

        /// <summary>
        /// All "day" elements in "skipDays"
        /// </summary>
        public IReadOnlyCollection<string> SkipDays { get; } = [];

        /// <summary>
        /// All "hour" elements in "skipHours"
        /// </summary>
        public IReadOnlyCollection<string> SkipHours { get; } = [];

        /// <summary>
        /// The "textInput" element
        /// </summary>
        public FeedTextInput TextInput { get; }

        /// <summary>
        /// All elements starting with "sy:"
        /// </summary>
        public SyndicationMetadata Sy { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaRssFeed"/> class.
        /// Reads a Media Rss feed based on the xml given in channel
        /// </summary>
        /// <param name="feedXml">the entire feed xml as string</param>
        /// <param name="channel">the "channel" element in the xml as XElement</param>
        public MediaRssFeed(string feedXml, ParentTag channel)
            : base(feedXml, channel)
        {
            this.Description = channel.GetValue("description");
            this.Language = channel.GetValue("language");
            this.Copyright = channel.GetValue("copyright");
            this.ManagingEditor = channel.GetValue("managingEditor");
            this.WebMaster = channel.GetValue("webMaster");
            this.Docs = channel.GetValue("docs");
            this.PublishingDateString = channel.GetValue("pubDate");
            this.LastBuildDateString = channel.GetValue("lastBuildDate");
            this.ParseDates(this.Language, this.PublishingDateString, this.LastBuildDateString);

            this.Categories = channel.GetArray("category", x => x.GetRequiredValue());

            this.Sy = new SyndicationMetadata(channel);
            this.Generator = channel.GetValue("generator");
            this.TTL = channel.GetValue("ttl");
            this.Image = new MediaRssFeedImage(channel.GetElement("image"));
            this.Cloud = new FeedCloud(channel.Tag("cloud"));
            this.TextInput = new FeedTextInput(channel.GetElement("textinput"));

            if (channel.GetElement("skipHours") is ParentTag skipHours)
            {
                this.SkipHours = skipHours.GetArray("hour", x => x.GetRequiredValue());
            }
            if (channel.GetElement("skipDays") is ParentTag skipDays)
            {
                this.SkipDays = skipDays.GetArray("day", x => x.GetRequiredValue());
            }

            this.Items = channel.GetArray("item", item => new MediaRssFeedItem((ParentTag)item));
        }

        /// <summary>
        /// Creates the base <see cref="Feed"/> element out of this feed.
        /// </summary>
        /// <returns>feed</returns>
        public override Feed ToFeed()
        {
            Feed f = new Feed(this)
            {
                Copyright = this.Copyright,
                Description = this.Description,
                ImageUrl = this.Image?.Url,
                Language = this.Language,
                LastUpdatedDate = this.LastBuildDate,
                LastUpdatedDateString = this.LastBuildDateString,
                Type = FeedType.MediaRss,
            };
            return f;
        }

        /// <summary>
        /// Sets the PublishingDate and LastBuildDate. If parsing fails, then it checks if the language
        /// is set and tries to parse the date based on the culture of the language.
        /// </summary>
        /// <param name="language">language of the feed</param>
        /// <param name="publishingDate">publishing date as string</param>
        /// <param name="lastBuildDate">last build date as string</param>
        private void ParseDates(string language, string publishingDate, string lastBuildDate)
        {
            this.PublishingDate = Helpers.TryParseDateTime(publishingDate);
            this.LastBuildDate = Helpers.TryParseDateTime(lastBuildDate);

            // check if language is set - if so, check if dates could be parsed or try to parse it with culture of the language
            if (string.IsNullOrWhiteSpace(language))
                return;

            // if publishingDateString is set but PublishingDate is null - try to parse with culture of "Language" property
            bool parseLocalizedPublishingDate = this.PublishingDate == null && !string.IsNullOrWhiteSpace(this.PublishingDateString);

            // if LastBuildDateString is set but LastBuildDate is null - try to parse with culture of "Language" property
            bool parseLocalizedLastBuildDate = this.LastBuildDate == null && !string.IsNullOrWhiteSpace(this.LastBuildDateString);

            // if both dates are set - return
            if (!parseLocalizedPublishingDate && !parseLocalizedLastBuildDate)
                return;

            // dates are set, but one of them couldn't be parsed - so try again with the culture set to the language
            CultureInfo culture;
            try
            {
                culture = new CultureInfo(this.Language);
            }
            catch (CultureNotFoundException)
            {
                // should be replace by a try parse or by getting all cultures and selecting the culture
                // out of the collection. That's unfortunately not available in .net standard 1.3 for now
                // this case should never happen, but in some rare cases it does - so catching the exception
                // is acceptable in that case.
                return; // culture couldn't be found, return as it was already tried to parse with default values
            }

            if (parseLocalizedPublishingDate)
            {
                this.PublishingDate = Helpers.TryParseDateTime(this.PublishingDateString, culture);
            }

            if (parseLocalizedLastBuildDate)
            {
                this.LastBuildDate = Helpers.TryParseDateTime(this.LastBuildDateString, culture);
            }
        }
    }
}
