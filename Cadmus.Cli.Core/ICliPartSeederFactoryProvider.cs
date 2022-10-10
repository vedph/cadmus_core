using Cadmus.Seed;
using System;

namespace Cadmus.Cli.Core
{
    /// <summary>
    /// CLI seeder factory provider.
    /// </summary>
    [Obsolete("CLI providers will be removed. Use providers from PRJ.Services " +
        "library implementing Cadmus.Seed.IPartSeederFactoryProvider instead.")]
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
