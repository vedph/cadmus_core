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
    /// <para>Tag: <c>fr.it.vedph.witnesses</c>.</para>
    /// </summary>
    /// <seealso cref="ITextLayerFragment" />
    [Tag("fr.it.vedph.witnesses")]
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
        /// For each unique witness source ID a pin with name=<c>fr.id</c>
        /// and value=ID is returned.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <returns>Pins: <c>fr.tot-count</c>, and a list of pins with keys
        /// <c>fr.wid</c>=witness ID (filtered, with digits).</returns>
        public IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            DataPinBuilder builder = new DataPinBuilder(
                new StandardDataPinTextFilter());

            builder.Set(PartBase.FR_PREFIX + "tot", Witnesses?.Count ?? 0, false);

            if (Witnesses?.Count > 0)
            {
                builder.AddValues(PartBase.FR_PREFIX + "wid",
                    from w in Witnesses select w.Id,
                    filter: true, filterOptions: true);
            }

            return builder.Build(null);
        }

        /// <summary>
        /// Gets the definitions of data pins used by the implementor.
        /// </summary>
        /// <returns>Data pins definitions.</returns>
        public IList<DataPinDefinition> GetDataPinDefinitions()
        {
            return new List<DataPinDefinition>(new[]
            {
                new DataPinDefinition(DataPinValueType.Integer,
                    PartBase.FR_PREFIX + "tot-count",
                    "The total count of witnesses."),
                new DataPinDefinition(DataPinValueType.String,
                    PartBase.FR_PREFIX + "wid",
                    "The list of witnesses IDs.",
                    "Mf")
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
