using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Cadmus.Core;
using Cadmus.Core.Layers;
using Fusi.Tools.Config;

namespace Cadmus.Philology.Parts.Layers
{
    /// <summary>
    /// Orthography layer fragment, used to mark deviations from the
    /// orthographical norm.
    /// <para>Tag: <c>fr.net.fusisoft.orthography</c>.</para>
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
        /// Initializes a new instance of the <see cref="OrthographyLayerFragment"/>
        /// class.
        /// </summary>
        public OrthographyLayerFragment()
        {
            Operations = new List<string>();
        }

        /// <summary>
        /// Get all the pins exposed by the implementor.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <remarks>If operations have tags, the operations with tags are
        /// grouped by them, and a pin is returned for each group, with its name
        /// equal to <c>fr.msp</c> + the grouped operations tag, and its value
        /// equal to the count of such operations. These pins are sorted
        /// by their name.
        /// <para>Also, if <paramref name="item"/> is received and it has
        /// a base text part and an orthography layer part, two additional pins
        /// are returned: <c>fr.orthography-txt</c> with the original orthography
        /// got from the base text, and <c>fr.orthography.std</c> with the
        /// <see cref="Standard"/> orthography from this fragment.</para>
        /// </remarks>
        /// <returns>pins</returns>
        public IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            List<DataPin> pins = new List<DataPin>();

            if (Operations?.Count > 0)
            {
                var groups = from s in Operations
                             let o = MspOperation.Parse(s)
                             where o?.Tag != null
                             group o by o.Tag
                    into g
                             select g;

                pins.AddRange(
                    from g in groups
                    orderby g.Key
                    select new DataPin
                    {
                        Name = PartBase.FR_PREFIX + $"msp.{g.Key}",
                        Value = g.Count().ToString(CultureInfo.InvariantCulture)
                    });
            }

            if (item != null)
            {
                // get the base text part
                IPart textPart = item.Parts
                    .Find(p => p.RoleId == PartBase.BASE_TEXT_ROLE_ID);
                if (textPart == null) return pins;

                // get the orthography layer
                TagAttribute attr = GetType().GetTypeInfo()
                    .GetCustomAttribute<TagAttribute>();
                Regex roleIdRegex = new Regex("^" + attr.Tag + "(?::.+)?$");

                IHasFragments<OrthographyLayerFragment> layerPart =
                    item.Parts.Find(p => p.RoleId != null
                        && roleIdRegex.IsMatch(p.RoleId))
                    as IHasFragments<OrthographyLayerFragment>;
                if (layerPart == null) return pins;

                string baseText = layerPart.GetTextAt(textPart, Location);
                if (baseText != null)
                {
                    pins.Add(new DataPin
                    {
                        Name = PartBase.FR_PREFIX + "orthography-txt",
                        Value = baseText
                    });
                    pins.Add(new DataPin
                    {
                        Name = PartBase.FR_PREFIX + "orthography-std",
                        Value = Standard
                    });
                }
            }

            return pins;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"[Orthography] {Location} {Standard} " +
                   (Operations != null ?
                   Operations.Count.ToString(CultureInfo.InvariantCulture) : "");
        }
    }
}
