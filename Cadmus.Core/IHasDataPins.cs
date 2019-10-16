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
        IEnumerable<DataPin> GetDataPins();
    }
}
