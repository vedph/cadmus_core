using System.Text;

namespace Cadmus.Philology.Parts
{
    /// <summary>
    /// An <see cref="AnnotatedValue"/> with an additional tag and location.
    /// </summary>
    /// <seealso cref="AnnotatedValue" />
    public class LocAnnotatedValue : AnnotatedValue
    {
        /// <summary>
        /// Gets or sets the tag, used to classify this value in any meaningful
        /// way.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(Tag))
                sb.Append('[').Append(Tag).Append(']');

            if (sb.Length > 0) sb.Append(' ');
            sb.Append(Value);

            if (!string.IsNullOrEmpty(Location))
                sb.Append(' ').Append(Location);
            if (!string.IsNullOrEmpty(Note))
                sb.Append(" (").Append(Note).Append(')');

            return sb.ToString();
        }
    }
}
