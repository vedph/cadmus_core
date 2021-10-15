using Cadmus.Core;
using Cadmus.Refs.Bricks;
using Fusi.Tools.Config;
using System.Collections.Generic;
using System.Text;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// A set of general purpose documental references.
    /// Tag: <c>it.vedph.doc-references</c>.
    /// </summary>
    [Tag("it.vedph.doc-references")]
    public sealed class DocReferencesPart : PartBase
    {
        /// <summary>
        /// Gets or sets the references.
        /// </summary>
        public List<DocReference> References { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocReferencesPart"/>
        /// class.
        /// </summary>
        public DocReferencesPart()
        {
            References = new List<DocReference>();
        }

        /// <summary>
        /// Get all the key=value pairs (pins) exposed by the implementor.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <returns>The pins: <c>tot-count</c> and a collections of pins with
        /// keys <c>citation</c>, <c>tag</c>.</returns>
        public override IEnumerable<DataPin> GetDataPins(IItem item)
        {
            DataPinBuilder builder = new DataPinBuilder();

            builder.Set("tot", References?.Count ?? 0, false);

            if (References?.Count > 0)
            {
                foreach (DocReference reference in References)
                {
                    builder.AddValue("citation", reference.Citation);
                    builder.AddValue("tag", reference.Tag);
                }
            }

            return builder.Build(this);
        }

        /// <summary>
        /// Gets the definitions of data pins used by the implementor.
        /// </summary>
        /// <returns>Data pins definitions.</returns>
        public override IList<DataPinDefinition> GetDataPinDefinitions()
        {
            return new List<DataPinDefinition>(new[]
            {
                new DataPinDefinition(DataPinValueType.Integer,
                    "tot-count",
                    "The total count of references."),
                new DataPinDefinition(DataPinValueType.String,
                    "citation",
                    "The list of citations.",
                    "M"),
                new DataPinDefinition(DataPinValueType.String,
                    "tag",
                    "The list of tags.",
                    "M")
            });
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[DocReferences]");

            if (References?.Count > 0)
            {
                sb.Append(' ');
                int n = 0;
                foreach (DocReference citation in References)
                {
                    if (++n > 5)
                    {
                        sb.Append("[...").Append(References.Count).Append(']');
                        break;
                    }
                    if (n > 1) sb.Append("; ");
                    sb.Append(citation);
                }
            }

            return sb.ToString();
        }
    }
}
