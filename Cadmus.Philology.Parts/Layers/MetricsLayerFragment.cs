using System.Collections.Generic;
using System.Linq;
using Cadmus.Core;
using Cadmus.Core.Layers;
using Fusi.Tools.Config;

namespace Cadmus.Philology.Parts.Layers
{
    /// <summary>
    /// Metrics layer fragment, used to mark the text as part of a metrical verse.
    /// Tag: <c>fr.it.vedph.metrics</c>.
    /// </summary>
    /// <seealso cref="ITextLayerFragment" />
    [Tag("fr.it.vepdh.metrics")]
    public sealed class MetricsLayerFragment : ITextLayerFragment
    {
        /// <summary>
        /// Gets or sets the location of this fragment.
        /// </summary>
        /// <remarks>
        /// The location can be expressed in different ways according to the
        /// text coordinates system being adopted. For instance, it might be a
        /// simple token-based coordinates system (e.g. 1.2=second token of
        /// first block), or a more complex system like an XPath expression.
        /// </remarks>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the metre (e.g. <c>3ia</c>=iambic trimeter.
        /// </summary>
        public string Metre { get; set; }

        /// <summary>
        /// Gets or sets the number for the portion of the base text this
        /// fragment refers to. This might be a simple verse number like <c>1</c>,
        /// or a more structured numbering like <c>11.3</c> (e.g. for strophe 11,
        /// line 3). Should the base text include several metrical compositions
        /// (e.g. a prose text including 2 epigrams), the first number might
        /// represent the composition number, and the others the structure of
        /// each composition.
        /// At any rate, you should adopt a convention which allows sorting the
        /// fragments by their number value, treated as a raw string.
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this verse is imperfectly
        /// or incompletely implemented.
        /// </summary>
        public bool IsImperfect { get; set; }

        /// <summary>
        /// Gets or sets an optional note about this verse.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Get all the pins exposed by the implementor.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <returns>The pins: <c>fr.metre</c>=metre, suffixed with <c>*</c>
        /// when imperfect.</returns>
        public IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            if (Metre == null) return Enumerable.Empty<DataPin>();
            return new[]
            {
                new DataPin
                {
                    Name = PartBase.FR_PREFIX + "metre",
                    Value = Metre + (IsImperfect? "*" : "")
                }
            };
        }

        /// <summary>
        /// Gets the definitions of data pins used by the implementor.
        /// </summary>
        /// <returns>Data pins definitions.</returns>
        public IList<DataPinDefinition> GetDataPinDefinitions()
        {
            return new List<DataPinDefinition>(new[]
            {
                new DataPinDefinition(DataPinValueType.String,
                    PartBase.FR_PREFIX + "metre",
                    "The metre, suffixed with * when imperfect.")
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
            return $"[Metrics] {Location} {Number} {Metre}" +
                   (IsImperfect
                       ? "*"
                       : "");
        }
    }
}
