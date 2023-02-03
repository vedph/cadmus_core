namespace Cadmus.Index.Config;

/// <summary>
/// Item browser factory provider. This is used to provide an
/// <see cref="ItemIndexFactory"/> from a specified profile file.
/// </summary>
public interface IItemIndexFactoryProvider
{
    /// <summary>
    /// Gets the part/fragment seeders factory.
    /// </summary>
    /// <param name="profile">The profile.</param>
    /// <returns>Factory.</returns>
    ItemIndexFactory GetFactory(string profile);
}
