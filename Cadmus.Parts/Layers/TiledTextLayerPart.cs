using Cadmus.Core;
using Cadmus.Core.Layers;
using Fusi.Tools.Config;
using System.Collections.Generic;

namespace Cadmus.Parts.Layers
{
    /// <summary>
    /// Text layer part class, based on tiles-referenced text. Note that
    /// currently tiles-referenced text uses the same coordinates system as
    /// token-referenced text: for tiles, y=row number and x=tile number.
    /// Tag: <c>net.fusisoft.tiled-text-layer</c>.
    /// </summary>
    /// <remarks>This class represents any text layer part using tiles-based
    /// coordinates. The tiles layer item part is just a wrapper for a collection
    /// of text layer fragments, and adds no other piece of data to the part itself.
    /// <para>
    /// A text layer part is like any other ordinary part, and derives
    /// from the same base class; its only peculiarity is that it just contains
    /// a collection of <see cref="ITextLayerFragment"/>-derived fragments,
    /// and exposes some utility methods to deal with them (e.g. adding a
    /// fragment, or getting all the fragments at the specified location).
    /// </para>
    /// <para>
    /// As a consequence, the pins exposed by this part is just the collection
    /// of all the pins exposed by its fragments. Also, layer parts always have
    /// their role ID equal to their fragments type ID; this effectively is the
    /// role played by this generic layer part in an item, as determined by the
    /// type of its fragments.
    /// </para>
    /// </remarks>
    /// <typeparam name="TFragment">The type of the fragment.</typeparam>
    /// <seealso cref="PartBase" />
    [Tag("net.fusisoft.tiled-text-layer")]
    public sealed class TiledTextLayerPart<TFragment> : YXLayerPartBase<TFragment>
        where TFragment : ITextLayerFragment, new()
    {
        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// Pins: all the pins from the part's fragments, sorted first in their
        /// order, and then by the criterion used by the fragment's type.
        /// By convention, fragment-generated pins should all start with
        /// prefix <see cref="PartBase.FR_PREFIX"/>.
        /// </summary>
        /// <returns>Pins.</returns>
        public override IEnumerable<DataPin> GetDataPins()
        {
            List<DataPin> pins = new List<DataPin>();
            if (Fragments == null) return pins;

            // add pins from fragments
            foreach (TFragment fr in Fragments)
            {
                foreach (DataPin frPin in fr.GetDataPins())
                {
                    DataPin pin = CreateDataPin(frPin.Name, frPin.Value);
                    pins.Add(pin);
                }
            }

            return pins;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{TypeId}.{RoleId}: {Fragments?.Count}";
        }
    }
}
