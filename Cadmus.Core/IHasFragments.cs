using Cadmus.Core.Layers;
using System.Collections.Generic;

namespace Cadmus.Core
{
    /// <summary>
    /// Interface implemented by layer parts, which include a collection
    /// of <see cref="ITextLayerFragment" />'s.
    /// </summary>
    /// <typeparam name="TFragment">The type of the fragment.</typeparam>
    public interface IHasFragments<TFragment>
        where TFragment : ITextLayerFragment, new()
    {
        /// <summary>
        /// Gets the fragments.
        /// </summary>
        List<TFragment> Fragments { get; }

        /// <summary>
        /// Add the specified layer fragment to this part. Any other existing
        /// fragment eventually overlapping the new one will be removed.
        /// </summary>
        /// <param name="fragment">The new fragment.</param>
        void AddFragment(TFragment fragment);

        /// <summary>
        /// Deletes all the non-range fragments at the integral location
        /// specified by the given coordinates. Fragments including this
        /// location but with a larger extent (ranges) are not deleted;
        /// fragments included by this location with a smaller extent 
        /// are deleted.
        /// </summary>
        /// <param name="location">The location. This must represent a
        /// single point, not a range.</param>
        void DeleteFragmentsAtIntegral(string location);

        /// <summary>
        /// Gets the non-range fragments whose extent is equal to or less than
        /// that specified by the given coordinates.
        /// </summary>
        /// <param name="location">The location. This must represent a single
        /// point, not a range.</param>
        /// <returns>Fragments list, empty if none matches.</returns>
        IList<TFragment> GetFragmentsAtIntegral(string location);

        /// <summary>
        /// Gets all the fragments ovlerapping the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>The fragments.</returns>
        IList<TFragment> GetFragmentsAt(string location);

        /// <summary>
        /// Gets the text at the specified location from the specified
        /// base text part.
        /// </summary>
        /// <param name="baseTextPart">The base text part to get text from.</param>
        /// <param name="location">The location.</param>
        /// <returns>The text, or null if location is invalid.</returns>
        string GetTextAt(IPart baseTextPart, string location);
    }
}
