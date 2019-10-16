using Cadmus.Core;

namespace Cadmus.Core.Layers
{
    /// <summary>
    /// A fragment contained in a text layer part. Fragments are the portions
    /// of a text layer part; they are not "sub-parts", as they do NOT have any 
    /// independent existance, but require their container part. The part as a
    /// whole, with all its fragments, is the atomic storage unit.
    /// <para>For instance, a comments text layer part would include one text
    /// layer fragment for each comment, representing it; yet, it's the part
    /// as a whole which contains all the comments layered on the base text.
    /// </para>
    /// </summary>
    /// <seealso cref="IHasDataPins" />
    public interface ITextLayerFragment : IHasDataPins
    {
        /// <summary>
        /// Gets or sets the location of this fragment.
        /// </summary>
        /// <remarks>The location can be expressed in different ways according
        /// to the text coordinates system being adopted. For instance, it
        /// might be a simple token-based coordinates system (e.g. 1.2=second
        /// token of first block), or a more complex system like an XPath
        /// expression.</remarks>
        string Location { get; set; }
    }
}
