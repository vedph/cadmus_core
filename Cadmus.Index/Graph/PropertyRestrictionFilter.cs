using Fusi.Tools.Data;

namespace Cadmus.Index.Graph
{
    /// <summary>
    /// A filter for <see cref="PropertyRestriction"/>.
    /// </summary>
    public class PropertyRestrictionFilter : PagingOptions
    {
        /// <summary>
        /// Gets or sets the property identifier the restrictions refer to.
        /// </summary>
        public int PropertyId { get; set; }

        /// <summary>
        /// Gets or sets the restriction to match.
        /// </summary>
        public string Restriction { get; set; }

        /// <summary>
        /// Gets or sets the object identifier to match.
        /// </summary>
        public int ObjectId { get; set; }
    }
}
