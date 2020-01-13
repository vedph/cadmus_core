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
        /// <returns>pins</returns>
        public override IEnumerable<DataPin> GetDataPins()
        {
            if (Categories == null || Categories.Count == 0)
                return Enumerable.Empty<DataPin>();

            return from category in Categories.OrderBy(s => s)
                select CreateDataPin("category", category);
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
