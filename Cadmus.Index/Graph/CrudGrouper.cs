using System;
using System.Collections.Generic;
using System.Linq;

namespace Cadmus.Index.Graph
{
    /// <summary>
    /// CRUD operation grouper helper. This is used to distribute a set of
    /// items into operation buckets, by comparing this new set to the old
    /// set.
    /// </summary>
    /// <typeparam name="T">The type of items.</typeparam>
    public sealed class CrudGrouper<T>
    {
        /// <summary>
        /// Gets the added items.
        /// </summary>
        public IList<T> Added { get; }

        /// <summary>
        /// Gets the updated items.
        /// </summary>
        public IList<T> Updated { get; }

        /// <summary>
        /// Gets the deleted items.
        /// </summary>
        public IList<T> Deleted { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CrudGrouper{T}"/> class.
        /// </summary>
        public CrudGrouper()
        {
            Added = new List<T>();
            Updated = new List<T>();
            Deleted = new List<T>();
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            Added.Clear();
            Updated.Clear();
            Deleted.Clear();
        }

        /// <summary>
        /// Groups the specified items into operation sets.
        /// </summary>
        /// <param name="newItems">The new items.</param>
        /// <param name="oldItems">The old items.</param>
        /// <param name="haveSameId">The function used to test whether two items
        /// have same identity.</param>
        /// <exception cref="ArgumentNullException">newItems or oldItems or
        /// haveSameId</exception>
        public void Group(IList<T> newItems, IList<T> oldItems,
            Func<T, T, bool> haveSameId)
        {
            if (newItems == null)
                throw new ArgumentNullException(nameof(newItems));
            if (oldItems == null)
                throw new ArgumentNullException(nameof(oldItems));
            if (haveSameId == null)
                throw new ArgumentNullException(nameof(haveSameId));

            Clear();

            // for each new item
            foreach (T ni in newItems)
            {
                // if found in old set, it's an update;
                // else it's an addition
                if (oldItems.Any(oi => haveSameId(ni, oi)))
                    Updated.Add(ni);
                else
                    Added.Add(ni);
            }

            // for each old item
            foreach (T oi in oldItems)
            {
                // if not found in new set, it's a deletion
                if (newItems.All(ni => !haveSameId(oi, ni)))
                    Deleted.Add(oi);
            }
        }
    }
}
