using System.Collections.Generic;

namespace Cadmus.Philology.Parts.Layers
{
    /// <summary>
    /// Interface representing an adapter which gets the result of a DiffPlex
    /// diff operation and rewrites it in terms of <see cref="MspOperation" />'s.
    /// </summary>
    /// <typeparam name="TResult">The type of the diffing result, which
    /// depends on the diffing library used.</typeparam>
    public interface IDiffResultToMspAdapter<TResult>
    {
        /// <summary>
        /// Adapt the result into a set of misspelling operations.
        /// </summary>
        /// <param name="result">result</param>
        /// <returns>zero or more misspelling operations</returns>
        IList<MspOperation> Adapt(TResult result);
    }
}
