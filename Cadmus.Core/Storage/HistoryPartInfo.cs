using System;

namespace Cadmus.Core.Storage
{
    /// <summary>
    /// Information about a history part record.
    /// </summary>
    public class HistoryPartInfo : IHasHistory
    {
        /// <summary>
        /// Gets or sets the history part identifier.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the item the parts belongs to.
        /// </summary>
        public string? ItemId { get; set; }

        /// <summary>
        /// Gets or sets the part's type identifier.
        /// </summary>
        public string? TypeId { get; set; }

        /// <summary>
        /// Gets or sets the role identifier.
        /// </summary>
        /// <remarks>Parts of the same type in the same item can have different
        /// roles. For instance, two date parts may refer to the date of the
        /// original text and to that of its later copy. In this case, a role
        /// ID helps selecting the desired part from an item.</remarks>
        public string? RoleId { get; set; }

        /// <summary>
        /// Creation date and time (UTC).
        /// </summary>
        public DateTime TimeCreated { get; set; }

        /// <summary>
        /// ID of the user who created the resource.
        /// </summary>
        public string CreatorId { get; set; }

        /// <summary>
        /// Last saved date and time (UTC).
        /// </summary>
        public DateTime TimeModified { get; set; }

        /// <summary>
        /// ID of the user who last saved the resource.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the reference identifier.
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public EditStatus Status { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryPartInfo"/> class.
        /// </summary>
        public HistoryPartInfo()
        {
            CreatorId =  UserId = ReferenceId = "";
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{ItemId}.{Id} {TypeId}" +
                (RoleId != null ? $" ({RoleId})" : "");
        }
    }
}
