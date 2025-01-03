﻿namespace Syndication.Parser;

using System;
using System.Collections.Frozen;
using System.Diagnostics;
using System.Globalization;

static class InvalidDateTimeOffset
{
    private const DateTimeStyles ParsingStyle = 
        DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AssumeUniversal;

    // they better should provide correct offsets instead of abbreviations, we just assume the values here
    private static readonly FrozenDictionary<string, string> knownTimeZones = new KeyValuePair<string, string>[]
    {
        new("CST", "-06:00"),
        new("EDT", "-04:00"),
        new("EST", "-05:00"),
        new("GMT", "+00:00"),
        new("MDT", "-06:00"),
        new("MST", "-07:00"),
        new("PDT", "-07:00"),
        new("PST", "-08:00"),
        new("UT",  "+00:00"),
        new("UTC", "+00:00"),
    }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    public static bool TryParse(ReadOnlySpan<char> str, out DateTimeOffset dateTimeOffset)
    {
        Span<char> buffer = stackalloc char[32];
        buffer.Fill(' ');

        var components = new DateTimeOffsetEnumerator(str);
        while (components.MoveNext())
        {
            var cmp = components.Current;
            switch (cmp.Type)
            {
                case DateTimeOffsetComponentType.DayOfWeek:
                    cmp.Span[..3].CopyTo(buffer);
                    buffer[3] = ',';
                    break;

                case DateTimeOffsetComponentType.Day:
                    if (cmp.Span.Length < 2)
                    {
                        buffer[5] = '0';
                        buffer[6] = cmp.Span[0];
                    }
                    else
                    {
                        cmp.Span[..2].CopyTo(buffer[5..]);
                    }
                    break;

                case DateTimeOffsetComponentType.Month:
                    cmp.Span[..3].CopyTo(buffer[8..]);
                    break;

                case DateTimeOffsetComponentType.Year:
                    cmp.Span.CopyTo(buffer[12..]);
                    break;

                case DateTimeOffsetComponentType.Date:
                    if (!DateTime.TryParse(cmp.Span, CultureInfo.InvariantCulture, ParsingStyle, out var date) ||
                        !date.TryFormat(buffer, out _, "R", CultureInfo.InvariantCulture))
                    {
                        dateTimeOffset = default;
                        return false;
                    }
                    break;

                case DateTimeOffsetComponentType.Time:
                    cmp.Span.CopyTo(buffer[17..]);
                    break;

                case DateTimeOffsetComponentType.Offset:
                    cmp.Span[..3].CopyTo(buffer[26..]);
                    buffer[29] = ':';
                    cmp.Span[^2..].CopyTo(buffer[30..]);
                    
                    if (((cmp.Span[1] == '1' && cmp.Span[2] > '4') || cmp.Span[1] > '1') &&
                        DateTimeOffset.TryParse(buffer[5..25], CultureInfo.InvariantCulture, ParsingStyle, out var dateTime))
                    {
                        var offsetSpan = buffer[26..];
                        if (offsetSpan[0] == '+')
                            offsetSpan = offsetSpan[1..];

                        if (TimeSpan.TryParse(offsetSpan, out var offset))
                        {
                            dateTimeOffset = dateTime + offset;
                            return true;
                        }
                    }
                    break;

                case DateTimeOffsetComponentType.TimeZone:
                    var timeZone = cmp.Span[..Math.Min(3, cmp.Span.Length)];
                    foreach (var tz in knownTimeZones)
                    {
                        if (timeZone.Equals(tz.Key, StringComparison.OrdinalIgnoreCase))
                        {
                            tz.Value.CopyTo(buffer[26..]);
                            break;
                        }
                    }
                    break;
            }
        }

        // "ddd dd MMM yyyy HH:mm:ss K" but we remove day-of-week component
        return DateTimeOffset.TryParse(buffer[5..], CultureInfo.InvariantCulture, ParsingStyle, out dateTimeOffset);
    }

    private enum DateTimeOffsetComponentType
    {
        Unknown,
        DayOfWeek,
        Day,
        Month,
        Year,
        Date,
        Time,
        Offset,
        TimeZone
    }

    [DebuggerDisplay("{Span}: {Type}")]
    private readonly ref struct DateTimeOffsetComponent
    {
        public DateTimeOffsetComponent(ReadOnlySpan<char> span, DateTimeOffsetComponentType type)
        {
            this.Span = span;
            this.Type = type;
        }

        public ReadOnlySpan<char> Span { get; }
        public DateTimeOffsetComponentType Type { get; }
    }

    private ref struct DateTimeOffsetEnumerator
    {
        private ReadOnlySpan<char> span;
        private DateTimeOffsetComponent current;
        private DateTimeOffsetComponentType type;

        public DateTimeOffsetEnumerator(ReadOnlySpan<char> span)
        {
            this.span = span;
            this.current = default;
            this.type = DateTimeOffsetComponentType.Unknown;
        }

        public readonly DateTimeOffsetComponent Current => this.current;

        public readonly DateTimeOffsetEnumerator GetEnumerator() => this;

        public bool MoveNext()
        {
            var remaining = this.span;
            if (remaining.IsEmpty)
                return false;

            var start = remaining.IndexOfAnyExcept(' ');
            if (start >= 0)
            {
                ++this.type;

                remaining = remaining[start..];
                var end = remaining.IndexOf(' ');
                if (end > 0)
                {
                    var component = remaining[..end];

                    this.type = SpecifyType(component, this.type);
                    if (this.type == DateTimeOffsetComponentType.Unknown)
                        goto UnknownComponent;
                    
                    this.current = new(component, this.type);
                    this.span = remaining[(end + 1)..];
                    return true;
                }

                this.type = SpecifyType(remaining, this.type);
                if (this.type == DateTimeOffsetComponentType.Unknown)
                    goto UnknownComponent;
                
                this.current = new(remaining, this.type);
                this.span = default;
                return true;
            }

        UnknownComponent:
            this.span = default;
            return false;
        }

        private static DateTimeOffsetComponentType SpecifyType(ReadOnlySpan<char> span, DateTimeOffsetComponentType suggestedType)
        {
            return
                char.IsAsciiDigit(span[0]) ? span switch
                {
                    { Length: <= 2 } => DateTimeOffsetComponentType.Day,

                    { Length: 4 } => DateTimeOffsetComponentType.Year,

                    { Length: 8 } => DateTimeOffsetComponentType.Time,

                    { Length: 10 } => DateTimeOffsetComponentType.Date,

                    _ => suggestedType,
                } :
                char.IsAsciiLetter(span[0]) ? span switch
                {
                    { Length: 2 } => DateTimeOffsetComponentType.TimeZone,

                    { Length: 3 } =>
                        char.IsAsciiLetterUpper(span[0]) &&
                        char.IsAsciiLetterUpper(span[1]) &&
                        char.IsAsciiLetterUpper(span[2]) ? DateTimeOffsetComponentType.TimeZone :
                        suggestedType == DateTimeOffsetComponentType.Day ? DateTimeOffsetComponentType.Month :
                        suggestedType,

                    { Length: > 3 } =>
                        char.IsAsciiLetterUpper(span[0]) &&
                        char.IsAsciiLetterUpper(span[1]) &&
                        char.IsAsciiLetterUpper(span[2]) ? DateTimeOffsetComponentType.TimeZone :
                        span[^1] == ',' ? DateTimeOffsetComponentType.DayOfWeek :
                        DateTimeOffsetComponentType.Month,

                    _ => suggestedType,
                } :
                span[0] == '+' || span[0] == '-' ? DateTimeOffsetComponentType.Offset :
                DateTimeOffsetComponentType.Unknown;
        }
    }
}
