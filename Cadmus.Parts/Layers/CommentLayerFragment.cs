using Cadmus.Core;
using Cadmus.Core.Layers;
using Cadmus.Parts.General;
using Fusi.Tools.Config;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cadmus.Parts.Layers
{
    /// <summary>
    /// Generic comment fragment. This item contains a generic comment referred
    /// to a specific text portion. The text format depends on the implementor.
    /// Tag: <c>fr.it.vedph.comment</c>.
    /// </summary>
    /// <seealso cref="ITextLayerFragment" />
    /// <seealso cref="IHasText" />
    [Tag("fr.it.vedph.comment")]
    public sealed class CommentLayerFragment : Comment, ITextLayerFragment
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
        /// Get all the key=value pairs exposed by the implementor.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <returns>The pins: <c>fr.tag</c>=tag if any, plus these list of
        /// pins: <c>fr.ref</c>=references (built with author and work,
        /// both filtered), <c>fr.id</c>=external IDs, <c>fr.cat</c>=categories,
        /// <c>fr.key.{INDEXID}.{LANG}</c>=keywords.</returns>
        public IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            return GetDataPins(item, null, PartBase.FR_PREFIX);
        }

        /// <summary>
        /// Gets the definitions of data pins used by the implementor.
        /// </summary>
        /// <returns>Data pins definitions.</returns>
        public IList<DataPinDefinition> GetDataPinDefinitions()
        {
            return GetDataPinDefinitions(PartBase.FR_PREFIX);
        }
    }
}
