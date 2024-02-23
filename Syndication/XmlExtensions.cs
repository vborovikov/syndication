namespace CodeHollow.FeedReader;

using System;
using System.Collections.Generic;
using System.Net;
using Brackets;
using Attribute = Brackets.Attribute;

static class XmlExtensions
{
    public static ParentTag Root(this Document document) => Find<ParentTag>(document.GetEnumerator(), _ => true);

    public static ParentTag Root(this Document document, string name) => Find<ParentTag>(document.GetEnumerator(), p => p.Name == name);

    public static ParentTag Root(this ParentTag parent, string name) => parent is null ? null : Find<ParentTag>(parent.GetEnumerator(), p => p.Name == name);

    public static Tag Tag(this Document document, string name) => Find<Tag>(document.GetEnumerator(), t => t.Name == name);

    public static Tag Tag(this ParentTag parent, string name) => parent is null ? null : Find<Tag>(parent.GetEnumerator(), t => t.Name == name);

    public static Attribute Attribute(this Tag tag, string name) => tag is null ? null : FindAttribute(tag.EnumerateAttributes(), name);

    public static string GetValue(this Element element)
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

    public static string GetValue(this ParentTag parent, string tagName) => parent.Tag(tagName)?.GetValue();

    public static string GetAttributeValue(this Tag tag, string attributeName) => tag.Attribute(attributeName)?.GetValue();

    public static ParentTag GetElement(this ParentTag parent, string name) => Root(parent, name);

    public static IEnumerable<Tag> GetElements(this Tag tag, string name) => tag is ParentTag parent ? GetTags(parent, name) : Array.Empty<Tag>();

    public static IEnumerable<Tag> GetTags(this ParentTag parent, string name) =>
        Enumerate<Tag>(parent.GetEnumerator(), t => t.Name == name);

    public static IEnumerable<ParentTag> GetRoots(this ParentTag parent, string name) =>
        Enumerate<ParentTag>(parent.GetEnumerator(), t => t.Name == name);

    private static Element GetSingleChild(this Element element)
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

    private static TElement Find<TElement>(Element.Enumerator elements, Predicate<TElement> match)
        where TElement : Element
    {
        while (elements.MoveNext())
        {
            if (elements.Current is TElement element && match(element))
            {
                return element;
            }
        }

        return null;
    }

    private static Attribute FindAttribute(Attribute.Enumerator attributes, string name)
    {
        while (attributes.MoveNext())
        {
            if (attributes.Current.Name == name)
            {
                return attributes.Current;
            }
        }

        return null;
    }
}
