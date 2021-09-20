using Fusi.Tools.Data;

namespace Cadmus.Index.Graph
{
    /// <summary>
    /// A filter used by <see cref="IGraphRepository"/>.
    /// </summary>
    public class NamespaceFilter : PagingOptions
    {
        /// <summary>
        /// Gets or sets any portion of the prefix to match.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Gets or sets any portion of the URI to match.
        /// </summary>
        public string Uri { get; set; }
    }
}
