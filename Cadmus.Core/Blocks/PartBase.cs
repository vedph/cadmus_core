using Fusi.Tools.Config;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cadmus.Core.Blocks
{
    /// <summary>
    /// Base class for parts implementations.
    /// </summary>
    /// <seealso cref="IPart" />
    public abstract class PartBase : IPart
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
        /// Gets or sets the part's type identifier. This is usually derived
        /// from the part's class <see cref="TagAttribute"/> attribute.
        /// </summary>
        public string TypeId { get; set; }

        /// <summary>
        /// Gets or sets the role identifier.
        /// </summary>
        /// <remarks>
        /// Parts of the same type in the same item can have different
        /// roles. For instance, two date parts may refer to the date of the original
        /// text and to that of its later copy. In this case, a role ID helps
        /// selecting the desired part from an item.
        /// </remarks>
        public string RoleId { get; set; }

        /// <summary>
        /// Last saved date and time (UTC).
        /// </summary>
        public DateTime TimeModified { get; set; }

        /// <summary>
        /// User ID.
        /// </summary>
        /// <remarks>This is the ID of the user who last modified the object.</remarks>
        public string UserId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartBase"/> class.
        /// </summary>
        protected PartBase()
        {
            Id = Guid.NewGuid().ToString("N");
            TagAttribute attr = GetType().GetTypeInfo().GetCustomAttribute<TagAttribute>();
            TypeId = attr != null ? attr.Tag : GetType().FullName;
            TimeModified = DateTime.UtcNow;
        }

        /// <summary>
        /// Creates a data pin from the specified key=value pair.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>data pin</returns>
        /// <exception cref="ArgumentNullException">null name</exception>
        protected DataPin CreateDataPin(string name, string value)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return new DataPin
            {
                ItemId = ItemId,
                PartId = Id,
                Name = name,
                Value = value
            };
        }

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// </summary>
        /// <returns>pins</returns>
        public abstract IEnumerable<DataPin> GetDataPins();
    }
}
