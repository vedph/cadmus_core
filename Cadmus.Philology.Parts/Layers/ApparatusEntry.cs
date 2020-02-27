using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cadmus.Philology.Parts.Layers
{
    /// <summary>
    /// Apparatus entry in an <see cref="ApparatusLayerFragment"/>.
    /// </summary>
    public sealed class ApparatusEntry
    {
        /// <summary>
        /// Variant type.
        /// </summary>
        public ApparatusEntryType Type { get; set; }

        /// <summary>
        /// Gets or sets the tag, an optional arbitrary string representing a
        /// categorization of some sort for that fragment, e.g. "margin",
        /// "interlinear", etc. This can be overridden by variants tag.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Lemma variant text value; can be zero (null or empty) for a
        /// deletion. When <see cref="Type"/> is <see cref="ApparatusEntryType.Note"/>
        /// this property has no meaning, as it's not applicable.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// An optional normalization of <see cref="Value"/>.
        /// </summary>
        public string NormValue { get; set; }

        /// <summary>
        /// True if this variant has been accepted.
        /// </summary>
        /// <remarks>Only 1 variant can be accepted, and represents the lemma.
        /// </remarks>
        public bool IsAccepted { get; set; }

        /// <summary>
        /// Gets or sets the group identifier, an optional arbitrary string used
        /// for grouping two or more fragments in the layer together.
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// Witnesses of this variant.
        /// </summary>
        public List<ApparatusAnnotatedValue> Witnesses { get; set; }

        /// <summary>
        /// Authors of this variant.
        /// </summary>
        public List<ApparatusAnnotatedValue> Authors { get; set; }

        /// <summary>
        /// An optional short note.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApparatusEntry"/> class.
        /// </summary>
        public ApparatusEntry()
        {
            Witnesses = new List<ApparatusAnnotatedValue>();
            Authors = new List<ApparatusAnnotatedValue>();
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Enum.GetName(typeof(ApparatusEntryType), Type)
                .ToLowerInvariant());

            if (Type != ApparatusEntryType.Note)
            {
                sb.Append(": ").Append(Value);
                if (IsAccepted) sb.Append('*');
                if (!string.IsNullOrEmpty(Note))
                    sb.Append(" (").Append(Note).Append(')');
            }
            else sb.Append(": ").Append(Note);

            if (Witnesses?.Count > 0)
            {
                sb.Append(' ')
                  .Append(string.Join(" ", from w in Witnesses
                                           select w.ToString()));
            }
            if (Authors?.Count > 0)
            {
                if (Witnesses?.Count > 0) sb.Append(", ");
                sb.Append(' ')
                  .Append(string.Join(" ", from a in Authors
                                           select a.ToString()));
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// An optionally annotated value used in an <see cref="ApparatusEntry"/>.
    /// </summary>
    public sealed class ApparatusAnnotatedValue
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the optional note.
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
            return string.IsNullOrEmpty(Note)? Value : $"{Value} ({Note})";
        }
    }

    /// <summary>
    /// Apparatus entry type.
    /// </summary>
    public enum ApparatusEntryType
    {
        /// <summary>variant should replace lemma</summary>
        Replacement = 0,

        /// <summary>variant should be added before lemma</summary>
        AdditionBefore,

        /// <summary>variant should be added after lemma</summary>
        AdditionAfter,

        /// <summary>any note to the text</summary>
        Note
    }
}
