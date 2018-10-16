namespace Cadmus.Core.Blocks
{
    /// <summary>
    /// Item's part.
    /// </summary>
    public interface IPart : IHasVersion, IHasDataPins
    {
        /// <summary>
        /// Gets or sets the part identifier.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the item the parts belongs to.
        /// </summary>
        string ItemId { get; set; }

        /// <summary>
        /// Gets or sets the part's type identifier.
        /// </summary>
        string TypeId { get; set; }

        /// <summary>
        /// Gets or sets the role identifier.
        /// </summary>
        /// <remarks>Parts of the same type in the same item can have different
        /// roles. For instance, two date parts may refer to the date of the original
        /// text and to that of its later copy. In this case, a role ID helps
        /// selecting the desired part from an item.</remarks>
        string RoleId { get; set; }
    }
}
