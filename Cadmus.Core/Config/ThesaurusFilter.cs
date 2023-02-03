using Fusi.Tools.Data;

namespace Cadmus.Core.Config;

/// <summary>
/// A filter for browsing thesauri.
/// </summary>
/// <seealso cref="PagingOptions" />
public sealed class ThesaurusFilter : PagingOptions
{
    /// <summary>
    /// Gets or sets the tag's ID filter. This matches all the thesauri
    /// whose ID contains the specified text.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the is-alias filter. This is null to match all the
    /// thesauri, true to match only those with a
    /// <see cref="Thesaurus.TargetId"/>, false to match only those without
    /// it.
    /// </summary>
    public bool? IsAlias { get; set; }

    /// <summary>
    /// Gets or sets the language filter. This matches all the thesauri
    /// with the specified language (ISO639-2/3).
    /// </summary>
    public string? Language { get; set; }
}
