using System.Collections.Generic;

namespace Cadmus.Core.Blocks
{
    /// <summary>
    /// Interface implemented by objects exposing data pins.
    /// </summary>
    public interface IHasDataPins
    {
        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// </summary>
        IEnumerable<DataPin> GetDataPins();
    }
}
