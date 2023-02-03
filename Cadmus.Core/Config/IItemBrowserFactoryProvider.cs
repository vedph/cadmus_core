namespace Cadmus.Core.Config;

/// <summary>
/// Item browser factory provider. This is used to provide an
/// <see cref="ItemBrowserFactory"/> from a specified profile file.
/// </summary>
public interface IItemBrowserFactoryProvider
{
    /// <summary>
    /// Gets the part/fragment seeders factory.
    /// </summary>
    /// <param name="profile">The profile.</param>
    /// <returns>Factory.</returns>
    ItemBrowserFactory GetFactory(string profile);
}
