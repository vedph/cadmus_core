using Cadmus.Core;
using Cadmus.Core.Layers;
using Fusi.Tools.Config;
using System.Collections.Generic;
using System.Linq;

namespace Cadmus.Philology.Parts.Layers
{
    /// <summary>
    /// Witnesses layer fragment. This collects 1 or more witnesses which
    /// represent the source for the base text. Each witness has an ID
    /// which uniquely identifies the source (e.g. <c>Fest.</c> for
    /// <c>Festus grammaticus</c>), a citation (e.g. <c>p.236</c>), a Markdown
    /// text, and an optional short note.
    /// <para>Tag: <c>fr.net.fusisoft.witnesses</c>.</para>
    /// </summary>
    /// <seealso cref="ITextLayerFragment" />
    [Tag("fr.net.fusisoft.witnesses")]
    public sealed class WitnessesLayerFragment : ITextLayerFragment
    {
        /// <summary>
        /// Gets or sets the location of this fragment.
        /// </summary>
        /// <remarks>
        /// The location can be expressed in different ways according
        /// to the text coordinates system being adopted. For instance, it
        /// might be a simple token-based coordinates system (e.g. 1.2=second
        /// token of first block), or a more complex system like an XPath
        /// expression.
        /// </remarks>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the witnesses.
        /// </summary>
        public List<Witness> Witnesses { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WitnessesLayerFragment"/>
        /// class.
        /// </summary>
        public WitnessesLayerFragment()
        {
            Witnesses = new List<Witness>();
        }

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// For each unique witness source ID, a pin with name=<c>fr.witness.id</c>
        /// and value=ID is returned.
        /// </summary>
        /// <returns>Pins.</returns>
        public IEnumerable<DataPin> GetDataPins()
        {
            if (Witnesses == null || Witnesses.Count == 0)
                return Enumerable.Empty<DataPin>();

            return (from w in Witnesses
                    orderby w.Id
                    select w.Id)
                    .Distinct()
                    .Select(id => new DataPin
                    {
                        Name = PartBase.FR_PREFIX + "witness.id",
                        Value = id
                    });
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"[Witnesses] {Witnesses?.Count}";
        }
    }

    /// <summary>
    /// A witness used in a <see cref="WitnessesLayerFragment"/>.
    /// </summary>
    public sealed class Witness
    {
        /// <summary>
        /// Gets or sets the source identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the source citation.
        /// </summary>
        public string Citation { get; set; }

        /// <summary>
        /// Gets or sets the (usually Markdown) text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets an optional short note.
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
            return $"{Id}, {Citation}: " +
                   (Text?.Length > 50 ? Text.Substring(50) : Text);
        }
    }
}
