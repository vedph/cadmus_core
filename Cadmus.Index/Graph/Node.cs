namespace Cadmus.Index.Graph
{
    /// <summary>
    /// A graph's node.
    /// </summary>
    public class Node
    {
        /// <summary>
        /// Gets or sets the node's identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the optional node's label. Most nodes have a label
        /// to ease their editing.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the type of the source for this node.
        /// </summary>
        public NodeSourceType SourceType { get; set; }

        /// <summary>
        /// Gets or sets the source ID for this node.
        /// </summary>
        public string Sid { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"#{Id} {Label} [{NodeMapping.SOURCE_TYPES[(int)SourceType]}] {Sid}";
        }
    }
}
