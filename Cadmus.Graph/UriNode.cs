namespace Cadmus.Graph;

/// <summary>
/// The result of searching a node.
/// </summary>
/// <seealso cref="Node" />
public class UriNode : Node
{
    /// <summary>
    /// Gets or sets the node URI.
    /// </summary>
    public string? Uri { get; set; }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return $"{Uri} {Label} [{SourceType}] {Sid}";
    }
}
