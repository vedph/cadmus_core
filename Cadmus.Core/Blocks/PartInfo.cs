using System;

namespace Cadmus.Core.Blocks
{
    /// <summary>
    /// Essential information about any type of part.
    /// </summary>
    public class PartInfo : IPartInfo
    {
        /// <summary>
        /// Gets or sets the part identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the item the parts belongs to.
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// Gets or sets the part's type identifier.
        /// </summary>
        public string TypeId { get; set; }

        /// <summary>
        /// Gets or sets the role identifier.
        /// </summary>
        /// <remarks>Parts of the same type in the same item can have different
        /// roles. For instance, two date parts may refer to the date of the original
        /// text and to that of its later copy. In this case, a role ID helps
        /// selecting the desired part from an item.</remarks>
        public string RoleId { get; set; }

        /// <summary>
        /// Gets or sets ID of the last user who modified the part.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the time the part was last modified.
        /// </summary>
        public DateTime TimeModified { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartInfo"/> class.
        /// </summary>
        public PartInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartInfo"/> class.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <exception cref="ArgumentNullException">null part</exception>
        public PartInfo(IPart part)
        {
            if (part == null) throw new ArgumentNullException(nameof(part));

            Id = part.Id;
            ItemId = part.ItemId;
            TypeId = part.TypeId;
            RoleId = part.RoleId;
            UserId = part.UserId;
            TimeModified = part.TimeModified;
        }
    }
}
