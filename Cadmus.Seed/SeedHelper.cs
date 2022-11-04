using Bogus;
using System.Collections.Generic;

namespace Cadmus.Seed
{
    /// <summary>
    /// Seeders helper functions.
    /// </summary>
    static public class SeedHelper
    {
        /// <summary>
        /// Randomly pick <paramref name="count"/> distinct entries from
        /// <paramref name="entries"/>. This uses the Bogus randomizer
        /// (<see cref="Randomizer.Seed"/>).
        /// </summary>
        /// <typeparam name="T">The type of entries.</typeparam>
        /// <param name="entries">The entries.</param>
        /// <param name="count">The desired count of entries.</param>
        /// <param name="maxAttempts">The maximum attempts count before
        /// giving up finding a distinct entry.</param>
        /// <returns>Entries (from 1 to <paramref name="count"/>; default(T)
        /// if no entries).</returns>
        public static IList<T>? RandomPickOf<T>(IList<T> entries,
            int count = 1,
            int maxAttempts = 10)
        {
            if (entries == null || entries.Count == 0) return default;

            HashSet<int> pickedIndexes = new();

            while (pickedIndexes.Count < count)
            {
                int index = Randomizer.Seed.Next(0, entries.Count);

                if (pickedIndexes.Contains(index))
                {
                    if (--maxAttempts == 0) break;
                }
                else pickedIndexes.Add(index);
            }

            T[] picked = new T[pickedIndexes.Count];
            int i = 0;
            foreach (int pickedIndex in pickedIndexes)
                picked[i++] = entries[pickedIndex];

            return picked;
        }

        /// <summary>
        /// Randomly pick a single entry from <paramref name="entries"/>.
        /// This uses the Bogus randomizer (<see cref="Randomizer.Seed"/>).
        /// </summary>
        /// <typeparam name="T">The type of entries.</typeparam>
        /// <param name="entries">The entries.</param>
        /// <returns>Entry, default(T) if no entries.</returns>
        public static T? RandomPickOneOf<T>(IList<T> entries)
        {
            if (entries == null || entries.Count == 0) return default;
            return entries[Randomizer.Seed.Next(0, entries.Count)];
        }
    }
}
