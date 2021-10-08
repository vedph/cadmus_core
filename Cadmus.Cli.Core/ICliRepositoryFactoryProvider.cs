using Cadmus.Core.Storage;

namespace Cadmus.Cli.Core
{
    /// <summary>
    /// CLI repository factory provider.
    /// </summary>
    public interface ICliRepositoryFactoryProvider
    {
        /// <summary>
        /// Gets or sets the connection string template, having placeholder
        /// <c>{0}</c> for the database name.
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// Creates the repository.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>Repository.</returns>
        ICadmusRepository CreateRepository(string database);
    }
}
