using Cadmus.Core;

namespace Cadmus.Seed
{
    /// <summary>
    /// Cadmus part seeder. This is used to seed a part with
    /// random data. Each implementor should get injected in its constructor
    /// an instance of <see cref="IPartTypeProvider"/>, which allows it
    /// to instantiate the part, and get a <see cref="TagAttribute"/> with
    /// value equal to <c>seed.</c> plus the part type ID.
    /// </summary>
    public interface IPartSeeder
    {
        /// <summary>
        /// Configures this seeder with the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        void Configure(SeedOptions options);

        /// <summary>
        /// Creates and seeds a new part.
        /// </summary>
        /// <param name="item">The item this part should belong to.</param>
        /// <param name="factory">The part seeder factory. This is used
        /// for layer parts, which need to seed a set of fragments.</param>
        /// <returns>A new part.</returns>
        IPart GetPart(IItem item, PartSeederFactory factory);
    }
}
