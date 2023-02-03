namespace Cadmus.Core.Storage;

/// <summary>
/// Filter for history items.
/// </summary>
/// <seealso cref="ItemFilter" />
/// <seealso cref="HistoryFilter" />
public class HistoryItemFilter : HistoryFilter
{
    /// <summary>
    /// Gets or sets the title filter.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the description filter.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the facet ID filter.
    /// </summary>
    public string? FacetId { get; set; }

    /// <summary>
    /// Gets or sets the group identifier.
    /// </summary>
    public string? GroupId { get; set; }

    /// <summary>
    /// Gets or sets the flags filter.
    /// </summary>
    public int? Flags { get; set; }

    /// <summary>
    /// Gets or sets the flag matching mode to use for <see cref="Flags"/>
    /// when it is not null.
    /// </summary>
    public FlagMatching FlagMatching { get; set; }
}
