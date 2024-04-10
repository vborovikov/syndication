namespace Syndication;

using System;
using System.Collections.Generic;
using System.Net;
using Brackets;

static class XmlExtensions
{
    public static Tag? Tag(this ParentTag root, string name) =>
        root?.FirstOrDefault<Tag>(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public static string? GetValue(this Element? element)
    {
        if (element is null)
            return null;

        var child = element.GetSingleChild();
        if (child != null)
        {
            element = child;
        }

        return element switch
        {
            Section section => section.ToString(), // don't decode section
            _ => element.TryGetValue<string>(out var str) ? WebUtility.HtmlDecode(str) : null,
        };
    }

    public static string? GetValue(this ParentTag parent, string tagName) => parent.Tag(tagName)?.GetValue();

    public static string? GetAttributeValue(this Tag? tag, string attributeName) => tag?.Attributes[attributeName].ToString();

    public static ParentTag? GetElement(this ParentTag parent, string name) => parent.FirstOrDefault<ParentTag>(r => r.Name == name);

    public static IEnumerable<Tag> GetElements(this Tag tag, string name) => tag is ParentTag parent ? GetTags(parent, name) : [];

    public static IEnumerable<Tag> GetTags(this ParentTag parent, string name) =>
        Enumerate<Tag>(parent.GetEnumerator(), t => t.Name == name);

    public static IEnumerable<ParentTag> GetRoots(this ParentTag parent, string name) =>
        Enumerate<ParentTag>(parent.GetEnumerator(), t => t.Name == name);

    private static Element? GetSingleChild(this Element element)
    {
        if (element is not ParentTag parent)
            return null;
        var elements = parent.GetEnumerator();
        if (!elements.MoveNext())
            return null;

        var child = elements.Current;
        if (elements.MoveNext())
            return null;

        return child;
    }

    private static IEnumerable<TElement> Enumerate<TElement>(Element.Enumerator elements, Predicate<TElement> match)
    {
        while (elements.MoveNext())
        {
            if (elements.Current is TElement element && match(element))
            {
                yield return element;
            }
        }
    }
}
