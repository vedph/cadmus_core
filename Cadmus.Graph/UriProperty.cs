namespace Cadmus.Graph;

/// <summary>
/// A property with a URI.
/// </summary>
/// <seealso cref="Property" />
public class UriProperty : Property
{
    /// <summary>
    /// Gets or sets the property URI.
    /// </summary>
    public string? Uri { get; set; }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return Uri ?? base.ToString()!;
    }
}
