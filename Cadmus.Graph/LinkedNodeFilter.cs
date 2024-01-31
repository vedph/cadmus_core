namespace Cadmus.Graph;

/// <summary>
/// Filter for nodes linked to a triple either as subject or as object.
/// </summary>
/// <seealso cref="NodeFilterBase" />
public class LinkedNodeFilter : NodeFilterBase
{
    /// <summary>
    /// Gets or sets the other node identifier, which is the subject node
    /// ID when <see cref="IsObject"/> is true, otherwise the object node ID.
    /// </summary>
    public int OtherNodeId { get; set; }

    /// <summary>
    /// Gets or sets the property identifier in the triple including the
    /// node to match, either as a subject or as an object (according to
    /// <see cref="IsObject"/>).
    /// </summary>
    public int PredicateId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the node to match is the
    /// object (true) or the subject (false) of the triple having predicate
    /// <see cref="PredicateId"/>.
    /// </summary>
    public bool IsObject { get; set; }
}
