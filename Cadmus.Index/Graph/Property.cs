using System.Text;

namespace Cadmus.Index.Graph
{
    /// <summary>
    /// The definition of a property (=a node which can be used as a predicate).
    /// This is 1:1 with its parent <see cref="Node"/>.
    /// </summary>
    public class Property
    {
        /// <summary>
        /// Gets or sets the property identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the type of the data, when the property has a literal
        /// value.
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// Gets or sets the literal value editor ID. This specifies the
        /// special literal value editor to use (when available) in the editor.
        /// This can be used to offer a special editor when editing the
        /// property's literal value. For instance, if the property is a
        /// historical date, you might want to use a historical date editor
        /// to aid users in entering it. Editing value as a string always
        /// remains the default, but an option can be offered to edit in an
        /// easier way when the value editor is specified and available
        /// in the frontend.
        /// </summary>
        public string LiteralEditor { get; set; }

        /// <summary>
        /// Gets or sets an optional human-readable description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('#').Append(Id);
            if (!string.IsNullOrEmpty(DataType))
                sb.Append(" ^").Append(DataType);
            if (!string.IsNullOrEmpty(LiteralEditor))
                sb.Append(" [").Append(LiteralEditor).Append(']');

            return sb.ToString();
        }
    }
}
