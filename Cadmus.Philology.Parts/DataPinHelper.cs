using Cadmus.Core;

namespace Cadmus.Philology.Parts
{
    /// <summary>
    /// Data pins helper.
    /// </summary>
    static internal class DataPinHelper
    {
        private static StandardDataPinTextFilter _filter;

        /// <summary>
        /// Gets the default filter used for pins.
        /// This improves performance, as we can share this filter
        /// among several parts.
        /// </summary>
        static public IDataPinTextFilter DefaultFilter
        {
            get { return _filter ?? (_filter = new StandardDataPinTextFilter()); }
        }
    }
}
