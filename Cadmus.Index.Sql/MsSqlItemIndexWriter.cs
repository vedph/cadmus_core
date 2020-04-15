using Cadmus.Core;
using Fusi.Tools.Config;
using System;
using System.Threading.Tasks;

namespace Cadmus.Index.Sql
{
    /// <summary>
    /// Item index writer for SQL Server.
    /// </summary>
    /// <seealso cref="Cadmus.Index.IItemIndexWriter" />
    [Tag("item-index-writer.mssql")]
    public sealed class MsSqlItemIndexWriter : IItemIndexWriter
    {
        /// <summary>
        /// Clears the whole index.
        /// </summary>
        public Task Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the specified item to the index.
        /// If the index does not exist, it is created.
        /// </summary>
        /// <exception cref="ArgumentNullException">item</exception>
        public Task Write(IItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            throw new NotImplementedException();
        }
    }
}
