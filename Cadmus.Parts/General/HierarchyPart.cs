using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Cadmus.Core;
using Fusi.Tools.Config;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// Hierarchy part. This defines a hierarchy among some items, by
    /// connecting a parent item to any number of children items. The part
    /// targets the parent, and can also specify its coordinates in the
    /// hierarchy tree, by setting its <see cref="Y"/> and <see cref="X"/>
    /// values. Finally, a <see cref="Tag"/> can be used to apply several
    /// different hierarchies to the same items.
    /// Tag: <c>net.fusisoft.hierarchy</c>.
    /// </summary>
    /// <seealso cref="PartBase" />
    [Tag("net.fusisoft.hierarchy")]
    public sealed class HierarchyPart : PartBase
    {
        /// <summary>
        /// Gets or sets the parent identifier.
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// Gets or sets the children IDs.
        /// </summary>
        public HashSet<string> ChildrenIds { get; set; }

        /// <summary>
        /// Gets or sets the depth level value represented by this part.
        /// Note that the Y-level number is not necessarily progressive,
        /// and its exact value depends on the consumer code. For instance,
        /// 1 might be the root item, 2 the children level, 3 the grandsons
        /// level, etc.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the sibling ordinal number (1-N). This is used
        /// when the targeted item is in turn a child of another item, and
        /// occupies a specific position in the set of its siblings.
        /// Note that the X-level number is not necessarily progressive,
        /// and its exact value depends on the consumer code. For instance,
        /// 1 might be the first sibling, 2 the second, etc.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the tag. This can be used for multiple hierarchies,
        /// so that all the parts with the same tag belong to the same hierarchy.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyPart"/> class.
        /// </summary>
        public HierarchyPart()
        {
            ChildrenIds = new HashSet<string>();
        }

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <remarks>
        /// <list type="bullet">
        /// <item>
        /// <term>y</term>
        /// <description><see cref="Y"/> value</description>
        /// <term>x</term>
        /// <description><see cref="X"/> value</description>
        /// </item>
        /// <item>
        /// <term>tag (optional)</term>
        /// <description><see cref="Tag"/> value</description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <returns>pins</returns>
        public override IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            List<DataPin> pins = new List<DataPin>
            {
                CreateDataPin("y", Y.ToString(CultureInfo.InvariantCulture)),
                CreateDataPin("x", X.ToString(CultureInfo.InvariantCulture))
            };

            if (Tag != null) pins.Add(CreateDataPin("tag", Tag));

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
            StringBuilder sb = new StringBuilder("[Hierarchy]");

            if (Tag != null) sb.Append(" (").Append(Tag).Append(')');

            sb.Append(' ').Append(Y).Append('.').Append(X);

            if (ParentId != null)
                sb.Append(" P=").Append(ParentId);
            if (ChildrenIds?.Count > 0)
                sb.Append(" C:").Append(ChildrenIds.Count);

            return sb.ToString();
        }
    }
}
