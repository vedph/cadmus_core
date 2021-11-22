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
        /// Gets the part type provider.
        /// </summary>
        /// <returns>part type provider</returns>
        IPartTypeProvider GetPartTypeProvider();

        /// <summary>
        /// Creates a Cadmus repository.
        /// </summary>
        /// <returns>repository</returns>
        /// <exception cref="ArgumentNullException">null database</exception>
        ICadmusRepository CreateRepository();
    }
}
