using System.Collections.Generic;
using System.Linq;
using Cadmus.Core;
using Cadmus.Core.Layers;
using Fusi.Tools.Config;

namespace Cadmus.Philology.Parts.Layers
{
    /// <summary>
    /// Quotations layer fragment, used to mark one or more literary quotations
    /// corresponding to a specific portion of the text.
    /// <para>Tag: <c>fr.net.fusisoft.quotations</c>.</para>
    /// </summary>
    /// <seealso cref="ITextLayerFragment" />
    [Tag("fr.net.fusisoft.quotations")]
    public sealed class QuotationsLayerFragment : ITextLayerFragment
    {
        /// <summary>
        /// Gets or sets the location of this fragment.
        /// </summary>
        /// <remarks>
        /// The location can be expressed in different ways according to the
        /// text coordinates system being adopted. For instance, it might be a
        /// simple token-based coordinates system (e.g. 1.2=second token of
        /// first block), or a more complex system like an XPath expression.
        /// </remarks>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the quotation entries.
        /// </summary>
        public List<QuotationEntry> Entries { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuotationsLayerFragment"/>
        /// class.
        /// </summary>
        public QuotationsLayerFragment()
        {
            Entries = new List<QuotationEntry>();
        }

        /// <summary>
        /// Get all the pins exposed by the implementor.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <returns>Pins: <c>fr.tot-count</c>, and a collection of pins with
        /// keys: <c>fr.author</c> (filtered, with digits), <c>fr.work</c>
        /// (filtered, with digits), <c>fr.cit-uri</c>, <c>fr.tag-X-count</c>.</returns>
        public IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            DataPinBuilder builder = new DataPinBuilder(
                new StandardDataPinTextFilter());

            builder.Set(PartBase.FR_PREFIX + "tot", Entries?.Count ?? 0, false);

            if (Entries?.Count > 0)
            {
                foreach (QuotationEntry entry in Entries)
                {
                    if (!string.IsNullOrEmpty(entry.Author))
                    {
                        builder.AddValue(PartBase.FR_PREFIX + "author",
                            entry.Author, filter: true, filterOptions: true);
                    }
                    if (!string.IsNullOrEmpty(entry.Work))
                    {
                        builder.AddValue(PartBase.FR_PREFIX + "work",
                            entry.Work, filter: true, filterOptions: true);
                    }

                    if (!string.IsNullOrEmpty(entry.CitationUri))
                    {
                        builder.AddValue(PartBase.FR_PREFIX + "cit-uri",
                            entry.CitationUri);
                    }

                    builder.Increase(entry.Tag, false, PartBase.FR_PREFIX + "tag-");
                }
            }

            return builder.Build(null);
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"[Quotation] {Entries?.Count ?? 0}";
        }
    }
}
