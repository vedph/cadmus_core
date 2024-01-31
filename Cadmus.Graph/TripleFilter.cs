using Fusi.Tools.Data;
using System.Collections.Generic;

namespace Cadmus.Graph;

/// <summary>
/// Filter for <see cref="Triple"/>.
/// </summary>
/// <seealso cref="PagingOptions" />
public class TripleFilter : LiteralFilter
{
    /// <summary>
    /// Gets or sets the subject node identifier to match.
    /// </summary>
    public int SubjectId { get; set; }

    /// <summary>
    /// Gets or sets the predicate node identifier which must be matched.
    /// At least 1 of these must match.
    /// </summary>
    public HashSet<int>? PredicateIds { get; set; }

    /// <summary>
    /// Gets or sets the predicate node identifier which must NOT be matched.
    /// None of these must match.
    /// </summary>
    public HashSet<int>? NotPredicateIds { get; set; }

    /// <summary>
    /// Gets or sets a value equal to true to match only triples having
    /// a literal object, false to match only triples having a non-literal
    /// object, or null to disable this filter.
    /// </summary>
    public bool? HasLiteralObject { get; set; }

    /// <summary>
    /// Gets or sets the object identifier to match.
    /// </summary>
    public int ObjectId { get; set; }

    /// <summary>
    /// Gets or sets the sid.
    /// </summary>
    public string? Sid { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="Sid"/> represents
    /// the initial portion of the SID being searched, rather than the
    /// full SID.
    /// </summary>
    public bool IsSidPrefix { get; set; }

    /// <summary>
    /// Gets or sets the tag filter to match. If null, no tag filtering
    /// is applied; if empty, only triples with a null tag are matched;
    /// otherwise, the triples with the same tag must be matched.
    /// </summary>
    public string? Tag { get; set; }
}
