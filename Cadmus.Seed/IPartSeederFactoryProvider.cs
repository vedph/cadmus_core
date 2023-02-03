namespace Cadmus.Seed;

/// <summary>
/// Part and fragments seeders factory provider. This is used to provide
/// a <see cref="PartSeederFactory"/>.
/// </summary>
public interface IPartSeederFactoryProvider
{
    /// <summary>
    /// Gets the part/fragment seeders factory.
    /// </summary>
    /// <param name="profile">The profile.</param>
    /// <returns>Factory.</returns>
    PartSeederFactory GetFactory(string profile);
}
