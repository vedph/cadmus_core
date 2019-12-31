using Cadmus.Core;

namespace Cadmus.Seed
{
    /// <summary>
    /// Cadmus part seeder. This is used to seed a part with random data.
    /// </summary>
    public interface IPartSeeder
    {
        /// <summary>
        /// Set the general options for seeding.
        /// </summary>
        /// <param name="options">The options.</param>
        void SetSeedOptions(SeedOptions options);

        /// <summary>
        /// Creates and seeds a new part.
        /// </summary>
        /// <param name="item">The item this part should belong to.</param>
        /// <param name="roleId">The optional part role ID.</param>
        /// <param name="factory">The part seeder factory. This is used
        /// for layer parts, which need to seed a set of fragments.</param>
        /// <returns>A new part.</returns>
        IPart GetPart(IItem item, string roleId, PartSeederFactory factory);
    }
}
