using System.Collections.Generic;
using System.Linq;
using Cadmus.Core.Blocks;
using Cadmus.Core.Layers;
using Fusi.Tools.Config;

namespace Cadmus.Philology.Parts.Layers
{
    /// <summary>
    /// Quotation layer fragment, used to mark literary quotations in the text.
    /// </summary>
    /// <seealso cref="ITextLayerFragment" />
    [Tag("fr-quotation")]
    public sealed class QuotationLayerFragment : ITextLayerFragment
    {
        /// <summary>
        /// Gets or sets the location of this fragment.
        /// </summary>
        /// <remarks>
        /// The location can be expressed in different ways according to the
        /// text coordinates system being adopted. For instance, it might be a simple
        /// token-based coordinates system (e.g. 1.2=second token of first block), or
        /// a more complex system like an XPath expression.
        /// </remarks>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the author (e.g. <c>Hom.</c>).
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the work (e.g. <c>Il.</c>).
        /// </summary>
        public string Work { get; set; }

        /// <summary>
        /// Gets or sets the work location (e.g. 3,24).
        /// </summary>
        public string WorkLoc { get; set; }

        /// <summary>
        /// Gets or sets the original quotation text, when the text this layer fragment
        /// refers to  is a variant of it.
        /// </summary>
        public string VariantOf { get; set; }

        /// <summary>
        /// Gets or sets an optional note.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Get all the pins exposed by the implementor.
        /// </summary>
        /// <returns>pins</returns>
        public IEnumerable<DataPin> GetDataPins()
        {
            if (Author == null || Work == null)
                return Enumerable.Empty<DataPin>();

            return new[]
            {
                new DataPin
                {
                    Name = "quote.author",
                    Value = Author
                },
                new DataPin
                {
                    Name = "quote.work",
                    Value = Work
                }
            };
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"Quotation: {Location} {Author} {Work} {WorkLoc}".TrimEnd();
        }
    }
}
