using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cadmus.Core;
using Cadmus.Core.Layers;
using Fusi.Tools.Config;

namespace Cadmus.Philology.Parts.Layers
{
    /// <summary>
    /// Orthography layer fragment, used to mark deviations from the
    /// orthographical norm.
    /// Tag: <c>fr.net.fusisoft.orthography</c>.
    /// </summary>
    /// <seealso cref="ITextLayerFragment" />
    [Tag("fr.net.fusisoft.orthography")]
    public sealed class OrthographyLayerFragment : ITextLayerFragment
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
        /// Gets or sets the standard orthography form for the word linked to
        /// this fragment.
        /// </summary>
        public string Standard { get; set; }

        /// <summary>
        /// Gets or sets the operations describing the relationship between the
        /// <see cref="Standard"/> form and the orthographically deviated form.
        /// Each operation is a text representing a <see cref="MspOperation"/>,
        /// to be parsed by <see cref="MspOperation.Parse(string)"/>.
        /// </summary>
        public List<string> Operations { get; set; }

        /// <summary>
        /// Get all the pins exposed by the implementor.
        /// </summary>
        /// <remarks>If operations have tags, the operations with tags are
        /// grouped by them and a pin is returned for each group, with name
        /// equal to the operations tag, and value equal to the count of
        /// such operations.</remarks>
        /// <returns>pins</returns>
        public IEnumerable<DataPin> GetDataPins()
        {
            if (Operations?.Count == 0)
                return Enumerable.Empty<DataPin>();

            var groups = from s in Operations
                         let o = MspOperation.Parse(s)
                where o?.Tag != null
                group o by o.Tag
                into g
                select g;

            return from g in groups
                orderby g.Key
                select new DataPin
                {
                    Name = PartBase.FR_PREFIX + $"msp.{g.Key}",
                    Value = g.Count().ToString(CultureInfo.InvariantCulture)
                };
        }

        /// <summary>
        /// Returns a <see cref="String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"[Orthography] {Location} {Standard} " +
                   (Operations != null ?
                   Operations.Count.ToString(CultureInfo.InvariantCulture) : "");
        }
    }
}
