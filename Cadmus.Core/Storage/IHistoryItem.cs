using Cadmus.Core.Blocks;

namespace Cadmus.Core.Storage
{
    /// <summary>
    /// History item interface.
    /// </summary>
    /// <seealso cref="Cadmus.Core.Blocks.IItem" />
    /// <seealso cref="Cadmus.Core.Storage.IHasHistory" />
    public interface IHistoryItem : IItem, IHasHistory
    {
    }
}
