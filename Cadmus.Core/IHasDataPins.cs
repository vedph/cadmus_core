using System.Collections.Generic;

namespace Cadmus.Core
{
    /// <summary>
    /// Interface implemented by objects exposing <see cref="DataPin"/>'s.
    /// </summary>
    public interface IHasDataPins
    {
        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        IEnumerable<DataPin> GetDataPins(IItem item = null);
    }
}
