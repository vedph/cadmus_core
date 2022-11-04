using System;

namespace Cadmus.Index
{
    /// <summary>
    /// A data pin stored in the items index.
    /// </summary>
    public sealed class IndexPin
    {
        /// <summary>
        /// Gets or sets the item identifier.
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// Gets or sets the part identifier.
        /// </summary>
        public string PartId { get; set; }

        /// <summary>
        /// Gets or sets the part type identifier.
        /// </summary>
        public string PartTypeId { get; set; }

        /// <summary>
        /// Gets or sets the optional role identifier.
        /// </summary>
        public string? RoleId { get; set; }

        /// <summary>
        /// Gets or sets the pin name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time at which this pin was added
        /// to the index.
        /// </summary>
        public DateTime TimeIndexed { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexPin"/> class.
        /// </summary>
        public IndexPin()
        {
            ItemId = PartId = PartTypeId = Name = Value = "";
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{Name}={Value} {PartTypeId} @{ItemId}.{PartId}" +
                (string.IsNullOrEmpty(RoleId) ? "" : " " + RoleId);
        }
    }
}
