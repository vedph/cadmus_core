namespace Cadmus.Index.Graph
{
    /// <summary>
    /// A <see cref="Node"/> with a URI. This is typically used when deserializing
    /// nodes before storing them into the graph database, thus receiving the
    /// numeric ID in exchange for the node's URI.
    /// </summary>
    /// <seealso cref="Node" />
    public class UriNode : Node
    {
        /// <summary>
        /// Gets or sets the node's URI.
        /// </summary>
        public string Uri { get; set; }
    }
}
