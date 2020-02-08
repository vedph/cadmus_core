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
    }
}
