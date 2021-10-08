using Cadmus.Seed;

namespace Cadmus.Cli.Core
{
    /// <summary>
    /// CLI seeder factory provider.
    /// </summary>
    public interface ICliPartSeederFactoryProvider
    {
        /// <summary>
        /// Gets the factory.
        /// </summary>
        /// <param name="profile">The profile.</param>
        /// <returns>The factory.</returns>
        PartSeederFactory GetFactory(string profile);
    }
}
