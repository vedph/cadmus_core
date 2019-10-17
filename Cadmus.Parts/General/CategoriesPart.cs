using System.Collections.Generic;
using System.Linq;
using Cadmus.Core;
using Fusi.Tools.Config;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// Categories part.
    /// Tag: <code>net.fusisoft.categories</code>.
    /// </summary>
    /// <remarks>
    /// <para>Search pins:</para>
    /// <list type="bullet">
    /// <item>
    /// <term>category name (multiple)</term>
    /// <description>category value</description>
    /// </item>
    /// </list>
    /// </remarks>
    [Tag("net.fusisoft.categories")]
    public sealed class CategoriesPart : PartBase
    {
        /// <summary>
        /// Categories.
        /// </summary>
        public List<string> Categories { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CategoriesPart()
        {
            Categories = new List<string>();
        }

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
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
        /// Textual representation of this part.
        /// </summary>
        /// <returns>date</returns>
        public override string ToString()
        {
            return $"{nameof(CategoriesPart)}: " + string.Join(", ", Categories);
        }
    }
}
