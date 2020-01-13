using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cadmus.Core;
using Cadmus.Core.Layers;
using Fusi.Tools.Config;

namespace Cadmus.Parts.Layers
{
    /// <summary>
    /// Text layer part class, based on token-referenced text.
    /// Tag: <c>net.fusisoft.token-text-layer</c>.
    /// </summary>
    /// <remarks>This class represents any text layer part using token-based
    /// coordinates. The text layer item part is just a wrapper for a collection
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
    [Tag("net.fusisoft.token-text-layer")]
    public sealed class TokenTextLayerPart<TFragment> : PartBase
        where TFragment : ITextLayerFragment, new()
    {
        /// <summary>
        /// Gets or sets the part's fragments.
        /// </summary>
        public List<TFragment> Fragments { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenTextLayerPart{TFragment}"/>
        /// class.
        /// Note that the <see cref="PartBase.RoleId"/> property is set here
        /// to the tag value of the fragment type. For instance, a 
        /// <see cref="TokenTextLayerPart{TFragment}"/> with fragments whose
        /// type has a <see cref="TagAttribute"/> value equal to <c>fr.comment</c>
        /// will have its <see cref="PartBase.RoleId"/> property equal
        /// to <c>fr.comment</c>. This effectively is the role played by this
        /// generic layer part in an item, as determined by the type of its
        /// fragments.
        /// </summary>
        public TokenTextLayerPart()
        {
            Fragments = new List<TFragment>();
            RoleId = typeof(TFragment).GetTypeInfo()
                .GetCustomAttribute<TagAttribute>()?.Tag;
        }

        /// <summary>
        /// Add the specified layer fragment to this part. Any other existing
        /// fragment eventually overlapping the new one will be removed.
        /// </summary>
        /// <param name="fragment">The new fragment.</param>
        /// <exception cref="ArgumentNullException">null item</exception>
        public void AddFragment(TFragment fragment)
        {
            if (fragment == null) throw new ArgumentNullException(nameof(fragment));

            if (Fragments == null) Fragments = new List<TFragment>();

            // remove all the existing overlapping fragments
            TokenTextLocation newLoc = TokenTextLocation.Parse(fragment.Location);

            for (int i = Fragments.Count - 1; i > -1; i--)
            {
                TokenTextLocation loc =
                    TokenTextLocation.Parse(Fragments[i].Location);
                if (newLoc.Overlaps(loc)) Fragments.RemoveAt(i);
            }

            Fragments.Add(fragment);
        }

        /// <summary>
        /// Gets all the fragments ovlerapping the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>The fragments</returns>
        /// <exception cref="ArgumentNullException">location</exception>
        public IList<TFragment> GetFragmentsAt(string location)
        {
            if (location == null) throw new ArgumentNullException(nameof(location));

            if (Fragments == null) return new List<TFragment>();

            TokenTextLocation requestedLoc = TokenTextLocation.Parse(location);

            return (from fr in Fragments
                where TokenTextLocation.Parse(fr.Location).Overlaps(requestedLoc)
                select fr).ToList();
        }

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
