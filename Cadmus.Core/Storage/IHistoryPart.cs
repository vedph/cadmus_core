using Cadmus.Core.Blocks;

namespace Cadmus.Core.Storage
{
    /// <summary>
    /// Wrapper for a part stored in parts history.
    /// </summary>
    /// <typeparam name="T">the type of the part</typeparam>
    public interface IHistoryPart<T> : IHasHistory where T : class, IPart
    {
        /// <summary>
        /// Gets or sets the history record identifier.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the wrapped part.
        /// </summary>
        T Content { get; set; }
    }
}
