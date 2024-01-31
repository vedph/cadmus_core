using Fusi.Tools.Data;

namespace Cadmus.Graph;

/// <summary>
/// A filter for <see cref="NodeMapping"/>.
/// </summary>
public class NodeMappingFilter : PagingOptions
{
    /// <summary>
    /// Gets or sets the parent mapping's identifier. Set to 0 to get
    /// only top level mappings; set to null to avoid filtering at all.
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// Gets or sets the source type to match.
    /// </summary>
    public int? SourceType { get; set; }

    /// <summary>
    /// Gets or sets any portion of the mapping's name to match.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the facet ID to match.
    /// </summary>
    public string? Facet { get; set; }

    /// <summary>
    /// Gets or sets the regular expression to match for the group ID.
    /// </summary>
    public string? Group { get; set; }

    /// <summary>
    /// Gets or sets the flags to match.
    /// </summary>
    public int? Flags { get; set; }

    /// <summary>
    /// Gets or sets the regular expression to match for the item's title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the type ID of the source part to match.
    /// </summary>
    public string? PartType { get; set; }

    /// <summary>
    /// Gets or sets the role ID of the source part to match.
    /// </summary>
    public string? PartRole { get; set; }
}
