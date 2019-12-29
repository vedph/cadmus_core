using Cadmus.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cadmus.Seed
{
    /// <summary>
    /// Base class for part seeders.
    /// </summary>
    /// <seealso cref="IPartSeeder" />
    public abstract class PartSeederBase : IPartSeeder
    {
        /// <summary>
        /// Gets the options.
        /// </summary>
        protected SeedOptions Options { get; private set;}

        /// <summary>
        /// Gets the random number generator.
        /// </summary>
        protected Random Random { get; private set; }

        /// <summary>
        /// Configures this seeder with the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <exception cref="ArgumentNullException">options</exception>
        public void Configure(SeedOptions options)
        {
            Options = options ??
                throw new ArgumentNullException(nameof(options));
            Random = options.Seed != null
                ? new Random(options.Seed.Value)
                : new Random();
        }

        /// <summary>
        /// Randomly pick any of the specified entries.
        /// </summary>
        /// <typeparam name="T">The type of entries.</typeparam>
        /// <param name="entries">The entries.</param>
        /// <param name="excluded">The optional entries to be excluded.</param>
        /// <returns>Entry, or default value for T if entries is null or empty.
        /// </returns>
        protected T RandomPickOf<T>(IList<T> entries, IList<T> excluded = null)
        {
            if (entries == null || entries.Count == 0) return default;

            T pick = default;
            int maxAttempts = 10;
            while (maxAttempts > 0)
            {
                pick = entries[Random.Next(0, entries.Count)];
                if (excluded.Any(e => e.Equals(pick)))
                {
                    if (--maxAttempts == 0) break;
                }
                else
                {
                    break;
                }
            }
            return pick;
        }

        /// <summary>
        /// Randomly pick any of the specified entries.
        /// </summary>
        /// <typeparam name="T">The type of entries.</typeparam>
        /// <param name="entries">The entries.</param>
        /// <param name="excluded">The optional entries to be excluded.</param>
        /// <returns>Entry, or default value for T if entries is null or empty.
        /// </returns>
        protected T RandomPickOf<T>(IList<T> entries, HashSet<T> excluded = null)
        {
            if (entries == null || entries.Count == 0) return default;

            T pick = default;
            int maxAttempts = 10;
            while (maxAttempts > 0)
            {
                pick = entries[Random.Next(0, entries.Count)];
                if (excluded.Contains(pick))
                {
                    if (--maxAttempts == 0) break;
                }
                else
                {
                    break;
                }
            }
            return pick;
        }

        /// <summary>
        /// Sets the part metadata: item ID, creator and user ID, role ID.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <param name="item">The item.</param>
        /// <exception cref="ArgumentNullException">part or item</exception>
        protected void SetPartMetadata(IPart part, IItem item)
        {
            if (part == null) throw new ArgumentNullException(nameof(part));
            if (item == null) throw new ArgumentNullException(nameof(item));

            part.ItemId = item.Id;
            part.CreatorId = item.CreatorId;
            part.UserId = item.UserId;

            // 1 out of 10 cases can have a role
            if (Options.PartRoles?.Length > 0
                && Random.Next(0, 10) == 1)
            {
                part.RoleId = RandomPickOf(Options.PartRoles, (string[])null);
            }
        }

        /// <summary>
        /// Creates and seeds a new part.
        /// </summary>
        /// <param name="item">The item this part should belong to.</param>
        /// <param name="factory">The part seeder factory. This is used
        /// for layer parts, which need to seed a set of fragments.</param>
        /// <returns>A new part.</returns>
        public abstract IPart GetPart(IItem item, PartSeederFactory factory);
    }
}
