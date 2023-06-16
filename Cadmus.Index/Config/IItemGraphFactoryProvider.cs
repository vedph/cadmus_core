namespace Cadmus.Index.Config;

/// <summary>
/// Item graph factory provider. This is used to provide an
/// <see cref="ItemGraphFactory"/> from a specified profile file.
/// </summary>
public interface IItemGraphFactoryProvider
{
    /// <summary>
    /// Gets the part/fragment seeders factory.
    /// </summary>
    /// <param name="profile">The profile.</param>
    /// <returns>Factory.</returns>
    ItemGraphFactory GetFactory(string profile);
}
