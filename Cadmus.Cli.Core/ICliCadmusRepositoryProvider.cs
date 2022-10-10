using Cadmus.Core.Storage;
using System;

namespace Cadmus.Cli.Core
{
    /// <summary>
    /// CLI Cadmus repository provider.
    /// </summary>
    [Obsolete("CLI providers will be removed. Use providers from PRJ.Services " +
        "library implementing Cadmus.Core.IRepositoryProvider instead.")]
    public interface ICliCadmusRepositoryProvider
    {
        /// <summary>
        /// Gets or sets the connection string template, having placeholder
        /// <c>{0}</c> for the database name.
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// Creates the repository.
        /// </summary>
        /// <param name="database">The database name.</param>
        /// <returns>Repository.</returns>
        ICadmusRepository CreateRepository(string database);
    }
}
