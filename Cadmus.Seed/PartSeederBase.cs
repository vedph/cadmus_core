using Bogus;
using Cadmus.Core;
using System;

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
        /// Set the general options for seeding.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <exception cref="ArgumentNullException">options</exception>
        public void SetSeedOptions(SeedOptions options)
        {
            Options = options ??
                throw new ArgumentNullException(nameof(options));
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
            // (no thesaurus scope)

            // eventually assign a role
            if (roleId?.StartsWith(PartBase.FR_PREFIX) == true)
            {
                if (roleId.IndexOf(':') == -1
                    && Options.FragmentRoles?.Length > 0
                    && Randomizer.Seed.Next(0, ASSIGN_ROLE_MAX) == 1)
                {
                    part.RoleId += ":" + SeedHelper.RandomPickOneOf(
                        Options.FragmentRoles);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(roleId)
                    && Options.PartRoles?.Length > 0
                    && Randomizer.Seed.Next(0, ASSIGN_ROLE_MAX) == 1)
                {
                    part.RoleId = SeedHelper.RandomPickOneOf(Options.PartRoles);
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
