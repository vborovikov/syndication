using System;
using Brackets;

namespace Syndication.Feeds.Itunes
{
    /// <summary>
    /// The itunes:... elements of an rss 2.0 item
    /// </summary>
    public record ItunesItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItunesItem"/> class.
        /// </summary>
        /// <param name="itemElement"></param>
        public ItunesItem(ParentTag itemElement)
        {
            this.Author = itemElement.GetValue("itunes:author");
            this.Block = itemElement.GetValue("itunes:block").EqualsIgnoreCase("yes");
            var imageElement = itemElement.Tag("itunes:image");

            if (imageElement != null)
            {
                this.Image = new ItunesImage(imageElement);
            }

            var duration = itemElement.GetValue("itunes:duration");
            this.Duration = ParseDuration(duration);

            var explicitValue = itemElement.GetValue("itunes:explicit");
            this.Explicit = explicitValue.EqualsIgnoreCase("yes", "explicit", "true");

            this.IsClosedCaptioned = itemElement.GetValue("itunes:isClosedCaptioned").EqualsIgnoreCase("yes");

            if (int.TryParse(itemElement.GetValue("itunes:order"), out var order))
            {
                this.Order = order;
            }

            this.Subtitle = itemElement.GetValue("itunes:subtitle");
            this.Summary = itemElement.GetValue("itunes:summary");
        }

        private static TimeSpan? ParseDuration(string duration)
        {
            if (String.IsNullOrWhiteSpace(duration))
                return null;

            var durationArray = duration.Split(':');

            if (durationArray.Length == 1 && long.TryParse(durationArray[0], out long result))
            {
                return TimeSpan.FromSeconds(result);
            }

            if (durationArray.Length == 2 && int.TryParse(durationArray[0], out int minutes) &&
                    int.TryParse(durationArray[1], out int seconds))
            {
                return new TimeSpan(0, minutes, seconds);
            }

            if (durationArray.Length == 3 && int.TryParse(durationArray[0], out int hours) &&
                    int.TryParse(durationArray[1], out int min) &&
                    int.TryParse(durationArray[2], out int sec))
            {
                return new TimeSpan(hours, min, sec);
            }

            return null;
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
        /// The itunes:image element
        /// </summary>
        public ItunesImage Image { get; }

        /// <summary>
        /// The itunes:duration element
        /// </summary>
        public TimeSpan? Duration { get; }

        /// <summary>
        /// The itunes:explicit element
        /// </summary>
        public bool Explicit { get; }

        /// <summary>
        /// The itunes:isClosedCaptioned element
        /// </summary>
        public bool IsClosedCaptioned { get; }

        /// <summary>
        /// The itunes:order element
        /// </summary>
        public int Order { get; }

        /// <summary>
        /// The itunes:subtitle element
        /// </summary>
        public string Subtitle { get; }

        /// <summary>
        /// The itunes:summary element
        /// </summary>
        public string Summary { get; }
    }
}
