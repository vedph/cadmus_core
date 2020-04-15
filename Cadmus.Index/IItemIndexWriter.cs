using Cadmus.Core;
using System.Threading.Tasks;

namespace Cadmus.Index
{
    /// <summary>
    /// Items index writer.
    /// </summary>
    public interface IItemIndexWriter
    {
        /// <summary>
        /// Writes the specified item to the index.
        /// If the index does not exist, it is created.
        /// </summary>
        /// <param name="item">The item.</param>
        Task Write(IItem item);

        /// <summary>
        /// Clears the whole index.
        /// </summary>
        Task Clear();
    }
}
