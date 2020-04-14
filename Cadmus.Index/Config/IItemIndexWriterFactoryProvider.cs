namespace Cadmus.Index.Config
{
    /// <summary>
    /// Item browser factory provider. This is used to provide an
    /// <see cref="ItemIndexWriterFactory"/> from a specified profile file.
    /// </summary>
    public interface IItemIndexWriterFactoryProvider
    {
        /// <summary>
        /// Gets the part/fragment seeders factory.
        /// </summary>
        /// <param name="profile">The profile.</param>
        /// <returns>Factory.</returns>
        ItemIndexWriterFactory GetFactory(string profile);
    }
}
