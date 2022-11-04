using Fusi.Tools.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cadmus.Core.Layers
{
    /// <summary>
    /// Base class for Y/X coordinates-based layer parts.
    /// </summary>
    /// <typeparam name="TFragment">The type of the fragment.</typeparam>
    /// <seealso cref="PartBase" />
    /// <seealso cref="IHasFragments{TFragment}" />
    public abstract class YXLayerPartBase<TFragment> : PartBase,
        IHasFragments<TFragment>
        where TFragment : ITextLayerFragment, new()
    {
        /// <summary>
        /// Gets or sets the part's fragments.
        /// </summary>
        public List<TFragment> Fragments { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="YXLayerPartBase{TFragment}"/>
        /// class.
        /// Note that the <see cref="PartBase.RoleId"/> property is set here
        /// to the tag value of the fragment type. For instance, a
        /// layer part with fragments whose type has a <see cref="TagAttribute"/>
        /// value equal to <c>fr.comment</c> will have its
        /// <see cref="PartBase.RoleId"/> property equal to <c>fr.comment</c>.
        /// This effectively is the role played by this generic layer part
        /// in an item, as determined by the type of its fragments.
        /// </summary>
        protected YXLayerPartBase()
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

            Fragments ??= new List<TFragment>();

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
        /// Deletes all the non-range fragments at the integral location
        /// specified by the given Y,X coordinates. Fragments including this
        /// location but with a larger extent (ranges) are not deleted;
        /// fragments included by this location with a smaller extent (with
        /// at/run) are deleted.
        /// </summary>
        /// <param name="location">The location with Y and X coordinates.
        /// This must represent a single point, not a range.</param>
        /// <exception cref="ArgumentNullException">null location</exception>
        public void DeleteFragmentsAtIntegral(string location)
        {
            if (location is null)
                throw new ArgumentNullException(nameof(location));

            if (Fragments == null) return;
            TokenTextLocation refLoc = TokenTextLocation.Parse(location);

            for (int i = Fragments.Count - 1; i > -1; i--)
            {
                TokenTextLocation loc =
                    TokenTextLocation.Parse(Fragments[i].Location);

                if (!loc.IsRange && loc.A.Y == refLoc.A.Y && loc.A.X == refLoc.A.X)
                    Fragments.RemoveAt(i);
            }
        }

        /// <summary>
        /// Gets the non-range fragments whose extent is equal to or less than
        /// that specified by the given Y/X coordinates.
        /// </summary>
        /// <param name="location">The location with Y and X coordinates.
        /// This must represent a single point, not a range.</param>
        /// <exception cref="ArgumentNullException">null location</exception>
        /// <returns>Fragments list, empty if none matches.</returns>
        public IList<TFragment> GetFragmentsAtIntegral(string location)
        {
            if (location is null)
                throw new ArgumentNullException(nameof(location));

            List<TFragment> frr = new();
            TokenTextLocation refLoc = TokenTextLocation.Parse(location);

            if (Fragments != null)
            {
                for (int i = Fragments.Count - 1; i > -1; i--)
                {
                    TokenTextLocation loc =
                        TokenTextLocation.Parse(Fragments[i].Location);
                    if (!loc.IsRange && loc.A.Y == refLoc.A.Y && loc.A.X == refLoc.A.X)
                        frr.Add(Fragments[i]);
                }
            }

            return frr;
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
        /// Gets the text at the specified location from the specified
        /// base text part.
        /// </summary>
        /// <param name="baseTextPart">The base text part to get text from.</param>
        /// <param name="location">The location.</param>
        /// <returns>The text, or null if location is invalid.</returns>
        public abstract string? GetTextAt(IPart baseTextPart, string location);
    }
}
