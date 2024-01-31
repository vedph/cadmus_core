using Fusi.Tools.Data;

namespace Cadmus.Graph;

/// <summary>
/// Filter for <see cref="NamespaceEntry"/>.
/// </summary>
public class NamespaceFilter : PagingOptions
{
    /// <summary>
    /// Gets or sets any portion of the prefix to match.
    /// </summary>
    public string? Prefix { get; set; }

    /// <summary>
    /// Gets or sets any portion of the URI to match.
    /// </summary>
    public string? Uri { get; set; }
}
