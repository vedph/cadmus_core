namespace Cadmus.Index.Graph
{
    /// <summary>
    /// An entry in the namespaces lookup set.
    /// This set is used in the <see cref="IGraphRepository"/> to lookup
    /// namespaces from their prefixes.
    /// </summary>
    public class NamespaceEntry
    {
        /// <summary>
        /// Gets or sets the prefix.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Gets or sets the URI.
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{Prefix}={Uri}";
        }
    }
}
