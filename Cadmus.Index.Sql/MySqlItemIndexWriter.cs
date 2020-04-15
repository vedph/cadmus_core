using Cadmus.Core;
using Fusi.Tools.Config;
using System;
using System.Threading.Tasks;

namespace Cadmus.Index.Sql
{
    /// <summary>
    /// Item index writer for MySql.
    /// </summary>
    [Tag("item-index-writer.mysql")]
    public sealed class MySqlItemIndexWriter : IItemIndexWriter
    {
        /// <summary>
        /// Clears the whole index.
        /// </summary>
        public Task Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clears the whole index.
        /// </summary>
        /// <exception cref="ArgumentNullException">item</exception>
        public Task Write(IItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            throw new NotImplementedException();
        }
    }
}
