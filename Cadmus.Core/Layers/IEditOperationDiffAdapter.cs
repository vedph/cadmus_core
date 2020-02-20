using DiffMatchPatch;
using System.Collections.Generic;

namespace Cadmus.Core.Layers
{
    /// <summary>
    /// Interface implemented by <see cref="Diff"/> to edit operations
    /// adapters.
    /// </summary>
    /// <typeparam name="TOperation">The type of the operation.</typeparam>
    /// <remarks>Such adapters are used to allow hints and automatic patches
    /// when reconciliating a layer part with its base text, which has been
    /// changed after the layer part itself. This reconciliation is based on
    /// diffing the two versions of the raw base text before and after changes
    /// in it, and combining the resulting diff operations with the coordinates
    /// system underlying each layer part. The final outcome is a set of
    /// edit operations which describe how each targeted text atom was affected
    /// by text editing. This atom varies according to the text part: e.g. it may
    /// be a token in token-based text parts, or a tile in tiled-text parts.
    /// </remarks>
    public interface IEditOperationDiffAdapter<TOperation> where TOperation : class
    {
        /// <summary>
        /// Adapts the specified diffs list into a list of edit operations.
        /// </summary>
        /// <param name="diffs">The diffs.</param>
        /// <returns>The edit operations.</returns>
        IList<TOperation> Adapt(IList<Diff> diffs);
    }
}
