using System.Collections.Generic;
using System.Linq;
using Cadmus.Core;
using Cadmus.Core.Config;
using Fusi.Tools.Config;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// Categories part. This is just a collection of any number of categories.
    /// Usually the categories correspond to the IDs of a <see cref="Thesaurus"/>.
    /// Tag: <c>net.fusisoft.categories</c>.
    /// </summary>
    [Tag("net.fusisoft.categories")]
    public sealed class CategoriesPart : PartBase
    {
        /// <summary>
        /// Categories.
        /// </summary>
        public HashSet<string> Categories { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CategoriesPart()
        {
            Categories = new HashSet<string>();
        }

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// Each category is a pin, with name=<c>category</c> and value=category.
        /// Pins are sorted by their value.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <returns>pins: <c>tot-count</c> and a collection of pins with
        /// keys: <c>category</c> (filtered, with digits).</returns>
        public override IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            DataPinBuilder builder = new DataPinBuilder(
                new StandardDataPinTextFilter());

            builder.Set("tot", Categories?.Count ?? 0, false);

            if (Categories?.Count > 0)
            {
                builder.AddValues("category", Categories,
                    filter: true, filterOptions: true);
            }

            return builder.Build(this);
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "[Categories] " + string.Join(", ", Categories?.OrderBy(s => s));
        }
    }
}
