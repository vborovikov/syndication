namespace Syndication;

using System;
using System.Collections.Generic;
using System.Net;
using Brackets;

static class XmlExtensions
{
    private const StringComparison NameComparison = StringComparison.OrdinalIgnoreCase;

    public static Tag? Tag(this ParentTag root, string name) =>
        root.FirstOrDefault<Tag>(tag => tag.Name.Equals(name, NameComparison));

    public static string? GetValue(this ParentTag root, string elementName) =>
        root.FirstOrDefault<Tag>(tag => tag.Name.Equals(elementName, NameComparison))?.GetValue();

    public static string GetRequiredValue(this ParentTag root, string elementName) =>
        root.First<Tag>(tag => tag.Name.Equals(elementName, NameComparison)).GetRequiredValue();

    public static string GetRequiredValue(this Element element)
    {
        return element.GetValue() ?? throw new RequiredElementValueMissingException();
    }

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

    public static T[] GetArray<T>(this ParentTag root, string elementName, Func<Tag, T> factory)
    {
        var items = new List<T>();

        foreach (var element in root)
        {
            if (element is Tag tag && tag.Name.Equals(elementName, NameComparison))
            {
                items.Add(factory(tag));
            }
        }

        return items.ToArray();
    }

    public static string GetAttributeValue(this Tag element, string attributeName) => element.Attributes[attributeName].ToString();

    public static string GetAttributeValue(this ParentTag root, string elementName, string attributeName) =>
        root.FirstOrDefault<Tag>(tag => tag.Name.Equals(elementName, NameComparison))?.Attributes[attributeName].ToString() ?? string.Empty;

    public static ParentTag? GetElement(this ParentTag parent, string name) => 
        parent.FirstOrDefault<ParentTag>(r => r.Name.Equals(name, NameComparison));

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

[Serializable]
public class RequiredElementValueMissingException : Exception
{
    public RequiredElementValueMissingException()
    {
    }

    public RequiredElementValueMissingException(string? message) : base(message)
    {
    }

    public RequiredElementValueMissingException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}