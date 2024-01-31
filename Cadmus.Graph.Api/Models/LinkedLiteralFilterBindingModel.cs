using System.ComponentModel.DataAnnotations;

namespace Cadmus.Graph.Api.Models;

public class LinkedLiteralFilterBindingModel : PagingBindingModel
{
    /// <summary>
    /// Gets or sets the object literal regular expression to match.
    /// </summary>
    [MaxLength(100)]
    public string? LiteralPattern { get; set; }

    /// <summary>
    /// Gets or sets the type of the object literal. This corresponds to
    /// literal suffixes after <c>^^</c> in Turtle: e.g.
    /// <c>"12.3"^^xs:double</c>.
    /// </summary>
    [MaxLength(100)]
    public string? LiteralType { get; set; }

    /// <summary>
    /// Gets or sets the object literal language. This is meaningful only
    /// for string literals, and usually is an ISO639 code.
    /// </summary>
    [MaxLength(10)]
    public string? LiteralLanguage { get; set; }

    /// <summary>
    /// Gets or sets the minimum numeric value for a numeric object literal.
    /// </summary>
    public double? MinLiteralNumber { get; set; }

    /// <summary>
    /// Gets or sets the maximum numeric value for a numeric object literal.
    /// </summary>
    public double? MaxLiteralNumber { get; set; }

    /// <summary>
    /// Gets or sets the subject identifier in the triple including the
    /// literal to match.
    /// </summary>
    public int SubjectId { get; set; }

    /// <summary>
    /// Gets or sets the property identifier in the triple including the
    /// literal to match.
    /// </summary>
    public int PredicateId { get; set; }

    public LinkedLiteralFilter ToLinkedLiteralFilter()
    {
        return new LinkedLiteralFilter
        {
            PageNumber = PageNumber,
            PageSize = PageSize,
            LiteralPattern = LiteralPattern,
            LiteralType = LiteralType,
            LiteralLanguage = LiteralLanguage,
            MinLiteralNumber = MinLiteralNumber,
            MaxLiteralNumber = MaxLiteralNumber,
            SubjectId = SubjectId,
            PredicateId = PredicateId
        };
    }
}
