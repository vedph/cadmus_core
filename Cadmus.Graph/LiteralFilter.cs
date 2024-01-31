using Fusi.Tools.Data;

namespace Cadmus.Graph;

public class LiteralFilter : PagingOptions
{
    /// <summary>
    /// Gets or sets the object literal regular expression to match.
    /// </summary>
    public string? LiteralPattern { get; set; }

    /// <summary>
    /// Gets or sets the type of the object literal. This corresponds to
    /// literal suffixes after <c>^^</c> in Turtle: e.g.
    /// <c>"12.3"^^xs:double</c>.
    /// </summary>
    public string? LiteralType { get; set; }

    /// <summary>
    /// Gets or sets the object literal language. This is meaningful only
    /// for string literals, and usually is an ISO639 code.
    /// </summary>
    public string? LiteralLanguage { get; set; }

    /// <summary>
    /// Gets or sets the minimum numeric value for a numeric object literal.
    /// </summary>
    public double? MinLiteralNumber { get; set; }

    /// <summary>
    /// Gets or sets the maximum numeric value for a numeric object literal.
    /// </summary>
    public double? MaxLiteralNumber { get; set; }
}
