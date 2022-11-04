using System.Text;

namespace Cadmus.Core.Layers
{
    /// <summary>
    /// A general-purpose edit operation generated from diffing, and based
    /// on Y/X coordinates. This is used for token-based and tiles-based
    /// text parts, as well as for any other text part whose coordinates can
    /// be expressed with Y and X.
    /// </summary>
    /// <remarks>This operation is used by adapters implementing
    /// <see cref="IEditOperationDiffAdapter{TOperation}"/>.</remarks>
    public class YXEditOperation
    {
        /// <summary>The equals operator (<c>equ</c>).</summary>
        public const string EQU = "equ";

        /// <summary>The delete operator (<c>del</c>).</summary>
        public const string DEL = "del";

        /// <summary>The insert operator (<c>ins</c>).</summary>
        public const string INS = "ins";

        /// <summary>The replace operator (<c>rep</c>).</summary>
        public const string REP = "rep";

        /// <summary>The move-delete operator (<c>mvd</c>).</summary>
        public const string MVD = "mvd";

        /// <summary>The move-insert operator (<c>mvi</c>).</summary>
        public const string MVI = "mvi";

        /// <summary>
        /// Gets or sets the old location.
        /// </summary>
        public string? OldLocation { get; set; }

        /// <summary>
        /// Gets or sets the location. In a delete operation, this is meaningless
        /// and you can refer only to the <see cref="OldLocation"/>.
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// Gets or sets the operator.
        /// </summary>
        public string? Operator { get; set; }

        /// <summary>
        /// Gets or sets the text value involved in the operation.
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// Gets or sets the old text value. This is used for replacements or
        /// deletions only, which require to store two different values
        /// instead of just one.
        /// </summary>
        public string? OldValue { get; set; }

        /// <summary>
        /// Gets or sets the group identifier. This is used for movements only.
        /// The two delete and insert operations involved in a move operation
        /// are grouped under the same ID.
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// Filters the text for displaying CR, LF and space with symbols.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The filtered text</returns>
        public static string? FilterTextForDisplay(string? text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            // https://superuser.com/questions/382163/how-do-i-visualize-cr-lf-in-word
            StringBuilder sb = new(text);
            sb.Replace('\r', '\u21a9');
            sb.Replace('\n', '\u240d');
            sb.Replace(' ', '\u00b7');
            return sb.ToString();
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append(OldLocation).Append("->")
                .Append(Location).Append(' ')
                .Append(Operator).Append(' ');

            if (OldValue != null)
                sb.Append(OldValue).Append("->").Append(FilterTextForDisplay(Value));
            else
                sb.Append(FilterTextForDisplay(Value));

            sb.Append(GroupId > 0 ? $" ({GroupId})" : "");
            return sb.ToString();
        }
    }
}
