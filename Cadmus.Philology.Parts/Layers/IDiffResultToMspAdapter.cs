using System.Collections.Generic;

namespace Cadmus.Philology.Parts.Layers
{
    /// <summary>
    /// Interface representing an adapter which gets the result of a
    /// diff operation and rewrites it in terms of <see cref="MspOperation" />'s.
    /// </summary>
    /// <typeparam name="TResult">The type of the diffing result object, which
    /// depends on the diffing library used.</typeparam>
    /// <remarks>Implementors of this interface are used to pre-populate the
    /// misspelling operations given a start and an end form. In most cases,
    /// the result will be ok; in some cases, when the user wants to convey
    /// a specific linguistic meaning with the misspelling operations, the
    /// result will require to be manually edited. In both cases, this speeds
    /// up the data entry process, as users just have to select a word form,
    /// enter its standard-orthography counterpart, and get the corresponding
    /// misspelling operations. For instance, the user might select the word
    /// <c>bixit</c> from an epigraphical Latin text, and type the
    /// orthographically correct form <c>vixit</c>; a differ would then detect
    /// the differences between these two forms, and produce a single misspelling
    /// operation: <c>"b"@1×1="v"</c>, i.e. "replace the initial b- with v-".
    /// </remarks>
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
