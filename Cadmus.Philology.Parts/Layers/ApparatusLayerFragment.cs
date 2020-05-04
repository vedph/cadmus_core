using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cadmus.Core;
using Cadmus.Core.Layers;
using Fusi.Tools.Config;

namespace Cadmus.Philology.Parts.Layers
{
    /// <summary>
    /// Critical apparatus layer fragment.
    /// Tag: <c>fr.net.fusisoft.apparatus</c>.
    /// </summary>
    /// <seealso cref="T:Cadmus.Core.Layers.ITextLayerFragment" />
    [Tag("fr.net.fusisoft.apparatus")]
    public sealed class ApparatusLayerFragment : ITextLayerFragment
    {
        #region Properties
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
        /// Gets or sets the tag, an optional arbitrary string representing a
        /// categorization of some sort for that fragment, e.g. "margin",
        /// "interlinear", etc. This can be overridden by variants tag.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the entries.
        /// </summary>
        public List<ApparatusEntry> Entries { get; set; }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ApparatusLayerFragment"/>
        /// class.
        /// </summary>
        public ApparatusLayerFragment()
        {
            Entries = new List<ApparatusEntry>();
        }

        private static void AddPinsFromSet(HashSet<string> set, string name,
            List<DataPin> pins)
        {
            foreach (string s in set.OrderBy(s => s))
            {
                pins.Add(new DataPin
                {
                    Name = PartBase.FR_PREFIX + name,
                    Value = s
                });
            }
        }

        /// <summary>
        /// Get all the pins exposed by the implementor.
        /// Pins are: <c>fr.variant</c>=variant (normalized if any, else just
        /// the variant), <c>fr.witness</c>=witness, <c>fr.author</c>=author;
        /// each distinct variant, witness, and author has a pin.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <returns>pins</returns>
        public IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            HashSet<string> witnesses = new HashSet<string>();
            HashSet<string> authors = new HashSet<string>();
            HashSet<string> variants = new HashSet<string>();

            foreach (ApparatusEntry entry in Entries)
            {
                if (entry.Type != ApparatusEntryType.Note)
                {
                    string variant = entry.NormValue ?? entry.Value;
                    if (!variants.Contains(variant))
                        variants.Add(variant);
                }

                if (entry.Witnesses != null)
                {
                    foreach (ApparatusAnnotatedValue w in entry.Witnesses
                        .OrderBy(w => w.Value))
                    {
                        if (!witnesses.Contains(w.Value))
                            witnesses.Add(w.Value);
                    }
                }

                if (entry.Authors != null)
                {
                    foreach (ApparatusAnnotatedValue a in entry.Authors
                        .OrderBy(a => a.Value))
                    {
                        if (!authors.Contains(a.Value))
                            authors.Add(a.Value);
                    }
                }
            }

            List<DataPin> pins = new List<DataPin>();
            AddPinsFromSet(variants, "variant", pins);
            AddPinsFromSet(witnesses, "witness", pins);
            AddPinsFromSet(authors, "author", pins);

            return pins;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder($"[Apparatus] {Location}");

            switch (Entries?.Count ?? 0)
            {
                case 1:
                    sb.Append(' ').Append(Entries[0].ToString());
                    break;
                case 0:
                    break;
                default:
                    sb.Append(' ').Append(Entries.Count);
                    break;
            }

            return sb.ToString();
        }
    }
}
