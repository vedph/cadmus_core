namespace Cadmus.Graph;

/// <summary>
/// A group of triples sharing the same predicate. This is used when
/// interactively browsing a graph node by node.
/// </summary>
public class TripleGroup
{
    /// <summary>
    /// Gets or sets the predicate identifier.
    /// </summary>
    public int PredicateId { get; set; }

    /// <summary>
    /// Gets or sets the predicate URI.
    /// </summary>
    public string? PredicateUri { get; set; }

    /// <summary>
    /// Gets or sets the count.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return $"#{PredicateId} {PredicateUri}={Count}";
    }
}
