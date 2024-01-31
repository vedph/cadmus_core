namespace Cadmus.Graph;

public class LinkedLiteralFilter : LiteralFilter
{
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
}
