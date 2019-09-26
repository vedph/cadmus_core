using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cadmus.Core.Blocks;
using Cadmus.Core.Layers;
using Fusi.Tools.Config;

namespace Cadmus.Parts.Layers
{
    /// <summary>
    /// Text layer part class, based on token-referenced text.
    /// </summary>
    /// <remarks>This class represents any text layer part. The text layer
    /// item part is just a wrapper for a collection of such text layer items,
    /// and adds no other piece of data to the part itself.
    /// <para>A text layer part is like any other ordinary part, and derives
    /// from the same base class; its only peculiarity is that it only contains
    /// a collection of <see cref="ITextLayerFragment"/>-derived fragments,
    /// and exposes some utility methods to deal with them (e.g. adding a
    /// fragment, or getting all the fragments at the specified location).</para>
    /// </remarks>
    [Tag("token-text-layer")]
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
        /// type has a <see cref="TagAttribute"/> value equal to <c>fr-comment</c>
        /// will have its <see cref="PartBase.RoleId"/> property equal
        /// to <c>fr-comment</c>. This effectively is the role played by this
        /// generic layer part in an item, as determined by the type of its
        /// fragments.
        /// </summary>
        public TokenTextLayerPart()
        {
            Fragments = new List<TFragment>();
            RoleId = typeof(TFragment).GetTypeInfo().GetCustomAttribute<TagAttribute>()?.Tag;
        }

        /// <summary>
        /// Add the specified layer fragment to this part. Any other existing fragment 
        /// (of the same type) eventually overlapping the new one will be removed.
        /// </summary>
        /// <param name="fragment">The new fragment.</param>
        /// <exception cref="ArgumentNullException">null item</exception>
        public void AddFragment(TFragment fragment)
        {
            if (fragment == null) throw new ArgumentNullException(nameof(fragment));

            if (Fragments == null) Fragments = new List<TFragment>();
            Type t = fragment.GetType();
            TokenTextLocation newLoc = TokenTextLocation.Parse(fragment.Location);

            for (int i = Fragments.Count - 1; i > -1; i--)
            {
                if (Fragments[i].GetType() != t) continue;
                TokenTextLocation loc = TokenTextLocation.Parse(Fragments[i].Location);
                if (newLoc.Overlaps(loc)) Fragments.RemoveAt(i);
            }

            Fragments.Add(fragment);
        }

        /// <summary>
        /// Gets all the fragments touched by the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>fragments</returns>
        /// <exception cref="ArgumentNullException">null location</exception>
        public IList<ITextLayerFragment> GetFragmentsAt(string location)
        {
            if (location == null) throw new ArgumentNullException(nameof(location));

            TokenTextLocation requestedLoc = TokenTextLocation.Parse(location);

            return (IList<ITextLayerFragment>) (from fr in Fragments
                where TokenTextLocation.Parse(fr.Location).Overlaps(requestedLoc)
                select fr).ToList();
        }

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// </summary>
        /// <returns>pins</returns>
        public override IEnumerable<DataPin> GetDataPins()
        {
            List<DataPin> pins = new List<DataPin>();
            foreach (TFragment fr in Fragments) pins.AddRange(fr.GetDataPins());

            foreach (DataPin pin in pins)
            {
                pin.ItemId = ItemId;
                pin.PartId = Id;
            }
            return pins;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{GetType().Name}: {Fragments.Count}";
        }
    }
}
