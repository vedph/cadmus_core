namespace Cadmus.Index.Graph
{
    /// <summary>
    /// A variable extracted from a <see cref="NodeMapping"/>. This can derive
    /// from either a placeholder in a template, or from a macro instruction
    /// as the full value of some parameter. In both cases, the variable has a
    /// unique ID, a name extracted from the ID (=ID minus argument if any),
    /// and an optional, single numeric argument extracted from the ID.
    /// </summary>
    public class NodeMappingVariable
    {
        /// <summary>
        /// Gets or sets the variable identifier. For macros, this is the variable
        /// name minus its <c>$</c> prefix; for placeholders, it's the placeholder
        /// name minus its wrapping braces. The optional argument is preserved
        /// as part of the ID, while being extracted in <see cref="Argument"/>.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name. This is equal to <see cref="Id"/> minus
        /// the optional <see cref="Argument"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the optional numeric value argument used in some of
        /// the macros.
        /// </summary>
        public int Argument { get; set; }

        /// <summary>
        /// Gets or sets the variable's value, when set.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{Id}={Value}";
        }
    }
}
