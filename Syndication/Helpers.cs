namespace Syndication
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Brackets;
    using Feeds.MediaRSS;
    using Syndication.Parser;

    /// <summary>
    /// static class with helper functions
    /// </summary>
    public static class Helpers
    {
        private const string ACCEPT_HEADER_NAME = "Accept";
        private const string ACCEPT_HEADER_VALUE = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
        private const string USER_AGENT_NAME = "User-Agent";
        private const string USER_AGENT_VALUE = "Mozilla/5.0 (Windows NT 6.3; rv:36.0) Gecko/20100101 Firefox/36.0";

        // The HttpClient instance must be a static field
        // https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
        private static readonly HttpClient _httpClient = new HttpClient(
            new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            }
        );

        /// <summary>
        /// Download the content from an url
        /// </summary>
        /// <param name="url">correct url</param>
        /// <returns>Content as string</returns>
        [Obsolete("Use the DownloadAsync method")]
        public static string Download(string url)
        {
            return DownloadAsync(url).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Download the content from an url
        /// </summary>
        /// <param name="url">correct url</param>
        /// <param name="cancellationToken">token to cancel operation</param>
        /// <param name="autoRedirect">autoredirect if page is moved permanently</param>
        /// <param name="userAgent">override built-in user-agent header</param>
        /// <returns>Content as byte array</returns>
        public static async Task<byte[]> DownloadBytesAsync(string url, CancellationToken cancellationToken, bool autoRedirect = true, string userAgent = USER_AGENT_VALUE)
        {
            url = System.Net.WebUtility.UrlDecode(url);
            HttpResponseMessage response;
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                request.Headers.TryAddWithoutValidation(ACCEPT_HEADER_NAME, ACCEPT_HEADER_VALUE);
                request.Headers.TryAddWithoutValidation(USER_AGENT_NAME, userAgent);

                response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false);
            }
            if (!response.IsSuccessStatusCode)
            {
                var statusCode = (int)response.StatusCode;
                // redirect if statuscode = 301 - Moved Permanently, 302 - Moved temporarly 308 - Permanent redirect
                if (autoRedirect && (statusCode == 301 || statusCode == 302 || statusCode == 308))
                {
                    url = response.Headers?.Location?.AbsoluteUri ?? url;
                }

                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false);
                }
            }
            var content = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            return content;
        }

        /// <summary>
        /// Download the content from an url
        /// </summary>
        /// <param name="url">correct url</param>
        /// <param name="autoRedirect">autoredirect if page is moved permanently</param>
        /// <returns>Content as byte array</returns>
        public static Task<byte[]> DownloadBytesAsync(string url, bool autoRedirect = true)
        {
            return DownloadBytesAsync(url, CancellationToken.None, autoRedirect);
        }

        /// <summary>
        /// Download the content from an url and returns it as utf8 encoded string.
        /// Preferred way is to use <see cref="DownloadBytesAsync(string, bool)"/> because it works
        /// better with encoding.
        /// </summary>
        /// <param name="url">correct url</param>
        /// <param name="cancellationToken">token to cancel operation</param>
        /// <param name="autoRedirect">autoredirect if page is moved permanently</param>
        /// <returns>Content as string</returns>
        public static async Task<string> DownloadAsync(string url, CancellationToken cancellationToken, bool autoRedirect = true)
        {
            var content = await DownloadBytesAsync(url, cancellationToken, autoRedirect).ConfigureAwait(false);
            return Encoding.UTF8.GetString(content);
        }

        /// <summary>
        /// Download the content from an url and returns it as utf8 encoded string.
        /// Preferred way is to use <see cref="DownloadBytesAsync(string, bool)"/> because it works
        /// better with encoding.
        /// </summary>
        /// <param name="url">correct url</param>
        /// <param name="autoRedirect">autoredirect if page is moved permanently</param>
        /// <returns>Content as string</returns>
        public static Task<string> DownloadAsync(string url, bool autoRedirect = true)
        {
            return DownloadAsync(url, CancellationToken.None, autoRedirect);
        }

        /// <summary>
        /// Tries to parse the string as datetime and returns null if it fails
        /// </summary>
        /// <param name="dateTimeSpan">datetime as string</param>
        /// <param name="cultureInfo">The cultureInfo for parsing</param>
        /// <returns>datetime or null</returns>
        public static DateTime? TryParseDateTime(ReadOnlySpan<char> dateTimeSpan, CultureInfo? cultureInfo = null)
        {
            if (dateTimeSpan.IsEmpty)
                return null;

            var dateTimeFormat = cultureInfo?.DateTimeFormat ?? DateTimeFormatInfo.CurrentInfo;
            var parseSuccess = 
                DateTimeOffset.TryParse(dateTimeSpan, dateTimeFormat, DateTimeStyles.None, out var dt) ||
                InvalidDateTimeOffset.TryParse(dateTimeSpan, out dt);

            if (!parseSuccess)
                return null;

            return dt.UtcDateTime;
        }

        /// <summary>
        /// Tries to parse the string as int and returns null if it fails
        /// </summary>
        /// <param name="input">int as string</param>
        /// <returns>integer or null</returns>
        public static int? TryParseInt(string input)
        {
            if (!int.TryParse(input, out int tmp))
                return null;
            return tmp;
        }

        /// <summary>
        /// Tries to parse a string and returns the media type
        /// </summary>
        /// <param name="medium">media type as string</param>
        /// <returns><see cref="Medium"/></returns>
        public static Medium TryParseMedium(string medium)
        {
            if (string.IsNullOrEmpty(medium))
            {
                return Medium.Unknown;
            }

            switch (medium.ToLower())
            {
                case "image":
                    return Medium.Image;
                case "audio":
                    return Medium.Audio;
                case "video":
                    return Medium.Video;
                case "document":
                    return Medium.Document;
                case "executable":
                    return Medium.Executable;
                default:
                    return Medium.Unknown;
            }
        }

        /// <summary>
        /// Tries to parse the string as int and returns null if it fails
        /// </summary>
        /// <param name="input">int as string</param>
        /// <returns>integer or null</returns>
        public static bool? TryParseBool(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                input = input.ToLower();

                if (input == "true")
                {
                    return true;
                }
                else if (input == "false")
                {
                    return false;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a HtmlFeedLink object from a linktag (link href="" type="")
        /// only support application/rss and application/atom as type
        /// if type is not supported, null is returned
        /// </summary>
        /// <param name="input">link tag, e.g. &lt;link rel="alternate" type="application/rss+xml" title="codehollow &gt; Feed" href="https://codehollow.com/feed/" /&gt;</param>
        /// <returns>Parsed HtmlFeedLink</returns>
        public static HtmlFeedLink GetFeedLinkFromLinkTag(string input)
        {
            var html = Document.Html.Parse(input);
            var tag = html.Find<Tag>(t => t.Name.Equals("link", StringComparison.OrdinalIgnoreCase));
            if (tag is null)
                return null;

            return GetFeedLinkFromLinkTag(tag);
        }

        /// <summary>
        /// Parses RSS links from html page and returns all links
        /// </summary>
        /// <param name="htmlContent">the content of the html page</param>
        /// <returns>all RSS/feed links</returns>
        public static IEnumerable<HtmlFeedLink> ParseFeedUrlsFromHtml(string htmlContent)
        {
            // sample link:
            // <link rel="alternate" type="application/rss+xml" title="Microsoft Bot Framework Blog" href="http://blog.botframework.com/feed.xml">
            // <link rel="alternate" type="application/atom+xml" title="Aktuelle News von heise online" href="https://www.heise.de/newsticker/heise-atom.xml">

            var html = Document.Html.Parse(htmlContent);
            return ParseFeedUrls(html);
        }

        internal static HtmlFeedLink[] ParseFeedUrls(Document html)
        {
            var links = html.Find("/html/head/link[@rel='alternate']");

            var result = new List<HtmlFeedLink>();
            foreach (var link in links)
            {
                if (link is Tag tag)
                {
                    var hfl = GetFeedLinkFromLinkTag(tag);
                    if (hfl is not null)
                    {
                        result.Add(hfl);
                    }
                }
            }
            return result.ToArray();
        }

        private static HtmlFeedLink? GetFeedLinkFromLinkTag(Tag link)
        {
            const StringComparison cmp = StringComparison.OrdinalIgnoreCase;

            var type = string.Empty;
            var title = string.Empty;
            var href = string.Empty;

            foreach (var attr in link.EnumerateAttributes())
            {
                if (attr.Name.Equals("type", cmp))
                {
                    type = attr.ToString();
                }
                else if (attr.Name.Equals("title", cmp))
                {
                    title = WebUtility.HtmlDecode(attr.ToString());
                }
                else if (attr.Name.Equals("href", cmp))
                {
                    href = attr.ToString();
                }
            }

            var feedType =
                type.Contains("application/rss", cmp) ? FeedType.Rss :
                type.Contains("application/atom", cmp) ? FeedType.Atom :
                FeedType.Unknown;
            if (feedType == FeedType.Unknown || string.IsNullOrWhiteSpace(href))
                return null;

            return new(title, href, feedType);
        }
    }
}
