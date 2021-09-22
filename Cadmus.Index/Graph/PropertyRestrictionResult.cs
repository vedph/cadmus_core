namespace Cadmus.Index.Graph
{
    /// <summary>
    /// A result of searching a property restriction.
    /// </summary>
    public class PropertyRestrictionResult : PropertyRestriction
    {
        /// <summary>
        /// Gets or sets the property URI.
        /// </summary>
        public string PropertyUri { get; set; }

        /// <summary>
        /// Gets or sets the optional object URI.
        /// </summary>
        public string ObjectUri { get; set; }
    }
}
