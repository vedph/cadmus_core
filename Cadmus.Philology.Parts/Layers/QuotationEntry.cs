using System.Text;

namespace Cadmus.Philology.Parts.Layers
{
    /// <summary>
    /// A quotation entry used in <see cref="QuotationsLayerFragment"/>.
    /// </summary>
    public sealed class QuotationEntry
    {
        /// <summary>
        /// Gets or sets the author (e.g. <c>verg</c>=Vergilius).
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the work (e.g. <c>ecl</c>=eclogae).
        /// </summary>
        public string Work { get; set; }

        /// <summary>
        /// Gets or sets the work's passage citation (e.g. <c>3.24</c>).
        /// </summary>
        public string Citation { get; set; }

        /// <summary>
        /// Gets or sets an optional URI used to identify the quotation's
        /// citation in an external reference citational system.
        /// </summary>
        public string CitationUri { get; set; }

        /// <summary>
        /// Gets or sets the modified quotation text, when the text this layer
        /// fragment refers to is different from the quoted text.
        /// </summary>
        public string Variant { get; set; }

        /// <summary>
        /// Gets or sets an optional tag to group quotations in any meaningful
        /// way.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets an optional note.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            // author
            if (!string.IsNullOrEmpty(Author)) sb.Append(Author);
            // , work
            if (!string.IsNullOrEmpty(Work))
            {
                if (sb.Length > 0) sb.Append(", ");
                sb.Append(Work);
            }
            // citation
            if (!string.IsNullOrEmpty(Citation))
            {
                if (sb.Length > 0) sb.Append(' ');
                sb.Append(Citation);
            }
            // "variant"
            if (!string.IsNullOrEmpty(Variant))
            {
                if (sb.Length > 0) sb.Append(' ');
                sb.Append('"').Append(Variant).Append('"');
            }
            // [tag]
            if (!string.IsNullOrEmpty(Tag))
            {
                if (sb.Length > 0) sb.Append(' ');
                sb.Append('[').Append(Tag).Append(']');
            }

            return sb.ToString();
        }
    }
}
