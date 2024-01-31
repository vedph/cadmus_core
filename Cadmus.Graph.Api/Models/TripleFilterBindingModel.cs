using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cadmus.Graph.Api.Models;

public class TripleFilterBindingModel : PagingBindingModel
{
    /// <summary>
    /// Gets or sets the subject node identifier to match.
    /// </summary>
    public int SubjectId { get; set; }

    /// <summary>
    /// Gets or sets the predicate node identifier to match.
    /// </summary>
    public HashSet<int>? PredicateIds { get; set; }

    /// <summary>
    /// Gets or sets the predicate node identifier not to match.
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
    [MaxLength(500)]
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
    [MaxLength(50)]
    public string? Tag { get; set; }

    [MaxLength(2)]
    public string? Sort { get; set; }

    /// <summary>
    /// Get a triple filter from this binding model.
    /// </summary>
    /// <returns>The filter.</returns>
    public TripleFilter ToTripleFilter()
    {
        return new TripleFilter
        {
            PageNumber = PageNumber,
            PageSize = PageSize,
            SubjectId = SubjectId,
            PredicateIds = PredicateIds,
            NotPredicateIds = NotPredicateIds,
            HasLiteralObject = HasLiteralObject,
            ObjectId = ObjectId,
            Sid = Sid,
            IsSidPrefix = IsSidPrefix,
            Tag = Tag
        };
    }
}
