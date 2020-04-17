﻿using Cadmus.Core;
using System;
using System.Threading.Tasks;

namespace Cadmus.Index
{
    /// <summary>
    /// Items index writer. The item index writer is used to add an item with
    /// all its parts to the index, but also has the ability of creating its
    /// target database if not present on he first write, and to remove all
    /// data from it on demand.
    /// </summary>
    public interface IItemIndexWriter : IDisposable
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
