using Cadmus.Core.Config;
using Cadmus.Core.Storage;
using System;

namespace Cadmus.Core
{
    /// <summary>
    /// Cadmus repository provider. This is used to provide implementations
    /// of <see cref="IPartTypeProvider"/> and <see cref="ICadmusRepository"/>.
    /// </summary>
    public interface IRepositoryProvider
    {
        /// <summary>
        /// Gets or sets the connection string to use.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets the part type provider.
        /// </summary>
        /// <returns>part type provider</returns>
        IPartTypeProvider GetPartTypeProvider();

        /// <summary>
        /// Creates a Cadmus repository.
        /// You must set <see cref="ConnectionString"/> before calling this method.
        /// </summary>
        /// <returns>repository</returns>
        ICadmusRepository CreateRepository();
    }
}
