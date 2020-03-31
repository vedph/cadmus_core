namespace Cadmus.Core.Config
{
    /// <summary>
    /// Item browser factory provider. This is used to provide an
    /// <see cref="ItemBrowserFactory"/> from a specified profile file.
    /// </summary>
    public interface IItemBrowserFactoryProvider
    {
        /// <summary>
        /// Gets the part/fragment seeders factory.
        /// </summary>
        /// <param name="profilePath">The profile file path.</param>
        /// <returns>Factory.</returns>
        ItemBrowserFactory GetFactory(string profilePath);
    }
}
