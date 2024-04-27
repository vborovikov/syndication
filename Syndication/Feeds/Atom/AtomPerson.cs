namespace Syndication.Feeds;

using Brackets;

/// <summary>
/// Atom 1.0 person element according to specification: https://validator.w3.org/feed/docs/atom.html
/// </summary>
public record AtomPerson
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AtomPerson"/> class.
    /// Reads an atom person based on the xml given in element
    /// </summary>
    /// <param name="element">person element as xml</param>
    public AtomPerson(ParentTag element)
    {
        this.Name = element.GetRequiredValue("name");
        this.Email = element.GetValue("email");
        this.Uri = element.GetValue("uri");
    }

    /// <summary>
    /// The "name" element
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The "email" element
    /// </summary>
    public string? Email { get; }

    /// <summary>
    /// The "uri" element
    /// </summary>
    public string? Uri { get; }

    /// <summary>
    /// returns the name of the author including the email if available
    /// </summary>
    /// <returns>name of the author with email if available</returns>
    public override string ToString()
    {
        if (string.IsNullOrEmpty(this.Email))
            return this.Name;

        return $"{this.Name} <{this.Email}>";
    }
}
