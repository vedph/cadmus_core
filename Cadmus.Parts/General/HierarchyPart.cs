using System.Collections.Generic;
using System.Globalization;
using Cadmus.Core.Blocks;
using Fusi.Tools.Config;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// Hierarchy part.
    /// </summary>
    /// <remarks>
    /// <para>Search pins:</para>
    /// <list type="bullet">
    /// 	<item>
    /// 		<term>y</term>
    /// 		<description><see cref="Y"/> value</description>
    /// 		<term>x</term>
    /// 		<description><see cref="X"/> value</description>
    /// 	</item>
    /// 	<item>
    /// 		<term>tag (optional)</term>
    /// 		<description><see cref="Tag"/> value</description>
    /// 	</item>
    /// </list>
    /// </remarks>
    /// <seealso cref="PartBase" />
    [Tag("hierarchy")]
    public sealed class HierarchyPart : PartBase
    {
        /// <summary>
        /// Gets or sets the parent identifier.
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// Gets or sets the children IDs.
        /// </summary>
        public List<string> ChildrenIds { get; set; }

        /// <summary>
        /// Gets or sets the depth level value represented by this part. Note that the 
        /// y-level number is not necessarily progressive.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the sibling ordinal number (1-N).
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyPart"/> class.
        /// </summary>
        public HierarchyPart()
        {
            ChildrenIds = new List<string>();
        }

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// </summary>
        /// <returns>pins</returns>
        public override IEnumerable<DataPin> GetDataPins()
        {
            return Tag != null?
                new[]
                {
                    CreateDataPin("tag", Tag),
                    CreateDataPin("y", Y.ToString(CultureInfo.InvariantCulture)),
                    CreateDataPin("x", X.ToString(CultureInfo.InvariantCulture))
                } :
                new[]
                {
                    CreateDataPin("y", Y.ToString(CultureInfo.InvariantCulture)),
                    CreateDataPin("x", X.ToString(CultureInfo.InvariantCulture))
                };
        }
    }
}
