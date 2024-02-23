namespace CodeHollow.FeedReader
{
    using System;

    /// <summary>
    /// Extension methods
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Determines whether this string and another string object have the same value.
        /// </summary>
        /// <param name="text">the string</param>
        /// <param name="compareTo">the string to compare to</param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this ReadOnlySpan<char> text, ReadOnlySpan<char> compareTo)
        {
            if (text.IsEmpty)
                return compareTo.IsEmpty;
            return text.Equals(compareTo, StringComparison.OrdinalIgnoreCase);
        } 

        /// <summary>
        /// Determines whether this string equals one of the given strings.
        /// </summary>
        /// <param name="text">the string</param>
        /// <param name="compareTo">all strings to compare to</param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this ReadOnlySpan<char> text, params string[] compareTo)
        {
            foreach(string value in compareTo)
            {
                if (text.EqualsIgnoreCase(value.AsSpan()))
                    return true;
            }
            return false;
        }

        public static bool EqualsIgnoreCase(this string text, params string[] compareTo) => EqualsIgnoreCase(text.AsSpan(), compareTo);
    }
}
