namespace Cadmus.Graph;

/// <summary>
/// A filter for <see cref="Node"/>.
/// </summary>
public class NodeFilter : NodeFilterBase
{
    /// <summary>
    /// Gets or sets the identifier of a node which is directly linked
    /// to the nodes being searched.
    /// </summary>
    public int LinkedNodeId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating the role of the node identified by
    /// <see cref="LinkedNodeId"/>: <c>S</c>=subject, <c>O</c>=object,
    /// else no role filtering.
    /// </summary>
    public char LinkedNodeRole { get; set; }
}
