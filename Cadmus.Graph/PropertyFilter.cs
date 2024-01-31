using Fusi.Tools.Data;

namespace Cadmus.Graph;

/// <summary>
/// A filter for <see cref="Property"/>.
/// </summary>
public class PropertyFilter : PagingOptions
{
    /// <summary>
    /// Gets or sets any portion of the property's UID to match.
    /// </summary>
    public string? Uid { get; set; }

    /// <summary>
    /// Gets or sets the type of the data to match.
    /// </summary>
    public string? DataType { get; set; }

    /// <summary>
    /// Gets or sets the literal editor to match.
    /// </summary>
    public string? LiteralEditor { get; set; }
}
