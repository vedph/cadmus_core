using System;

namespace Cadmus.Core.Config;

/// <summary>
/// Hierarchical entry for <see cref="Thesaurus"/>, used when processing
/// its entries in a hierarchical structure is required.
/// </summary>
/// <seealso cref="ThesaurusEntry" />
public class ThesaurusTreeEntry : ThesaurusEntry
{
    /// <summary>
    /// Gets or sets the parent node.
    /// </summary>
    public ThesaurusTreeEntry? Parent { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThesaurusTreeEntry"/>
    /// class.
    /// </summary>
    public ThesaurusTreeEntry()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThesaurusTreeEntry"/>
    /// class.
    /// </summary>
    /// <param name="entry">The entry.</param>
    /// <exception cref="ArgumentNullException">entry</exception>
    public ThesaurusTreeEntry(ThesaurusEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        Id = entry.Id;
        Value = entry.Value;
    }
}
