using Bogus;
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
        // assign a role in 1 case out of 10
        private const int ASSIGN_ROLE_MAX = 10;

        /// <summary>
        /// Gets the options.
        /// </summary>
        protected SeedOptions Options { get; private set;}

        /// <summary>
        /// Configures this seeder with the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <exception cref="ArgumentNullException">options</exception>
        public void Configure(SeedOptions options)
        {
            Options = options ??
                throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Randomly pick any of the specified entries.
        /// </summary>
        /// <typeparam name="T">The type of entries.</typeparam>
        /// <param name="entries">The entries.</param>
        /// <param name="excluded">The optional entries to be excluded.</param>
        /// <returns>Entry, or default value for T if entries is null or empty.
        /// </returns>
        protected static T RandomPickOf<T>(IList<T> entries, IList<T> excluded = null)
        {
            if (entries == null || entries.Count == 0) return default;

            T pick = default;
            int maxAttempts = 10;
            while (maxAttempts > 0)
            {
                pick = entries[Randomizer.Seed.Next(0, entries.Count)];
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
        protected static T RandomPickOf<T>(IList<T> entries, HashSet<T> excluded = null)
        {
            if (entries == null || entries.Count == 0) return default;

            T pick = default;
            int maxAttempts = 10;
            while (maxAttempts > 0)
            {
                pick = entries[Randomizer.Seed.Next(0, entries.Count)];
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
        /// <param name="roleId">The part's role ID or null.</param>
        /// <param name="item">The item.</param>
        /// <exception cref="ArgumentNullException">part or item</exception>
        protected void SetPartMetadata(IPart part, string roleId, IItem item)
        {
            if (part == null) throw new ArgumentNullException(nameof(part));
            if (item == null) throw new ArgumentNullException(nameof(item));

            part.ItemId = item.Id;
            part.CreatorId = item.CreatorId;
            part.UserId = item.UserId;
            part.RoleId = string.IsNullOrEmpty(roleId) ? null : roleId;

            // eventually assign a role
            if (roleId?.StartsWith(PartBase.FR_PREFIX) == true)
            {
                if (roleId.IndexOf(':') == -1
                    && Options.FragmentRoles?.Length > 0
                    && Randomizer.Seed.Next(0, ASSIGN_ROLE_MAX) == 1)
                {
                    part.RoleId += ":" + RandomPickOf(Options.FragmentRoles, (string[])null);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(roleId)
                    && Options.PartRoles?.Length > 0
                    && Randomizer.Seed.Next(0, ASSIGN_ROLE_MAX) == 1)
                {
                    part.RoleId = RandomPickOf(Options.PartRoles, (string[])null);
                }
            }
        }

        /// <summary>
        /// Creates and seeds a new part.
        /// </summary>
        /// <param name="item">The item this part should belong to.</param>
        /// <param name="roleId">The optional part role ID.</param>
        /// <param name="factory">The part seeder factory. This is used
        /// for layer parts, which need to seed a set of fragments.</param>
        /// <returns>A new part.</returns>
        public abstract IPart GetPart(IItem item, string roleId,
            PartSeederFactory factory);
    }
}
