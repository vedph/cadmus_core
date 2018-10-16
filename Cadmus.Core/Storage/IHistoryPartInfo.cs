using Cadmus.Core.Blocks;

namespace Cadmus.Core.Storage
{
    /// <summary>
    /// Essential information about a history part.
    /// </summary>
    /// <seealso cref="Cadmus.Core.Blocks.IHasVersion" />
    /// <seealso cref="Cadmus.Core.Storage.IHasHistory" />
    public interface IHistoryPartInfo : IHasVersion, IHasHistory
    {
        /// <summary>
        /// Gets or sets the history part identifier.
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
