namespace Cadmus.Core
{
    /// <summary>
    /// A simple text filter for <see cref="DataPin"/>'s. Implementations of
    /// this interface can be used to filter text values when creating data
    /// pins.
    /// </summary>
    public interface IDataPinTextFilter
    {
        /// <summary>
        /// Applies this filter to the specified text.
        /// </summary>
        /// <param name="text">The text to apply this filter to.</param>
        /// <returns>Filtered text.</returns>
        /// <param name="options">An optional object with filter-specific
        /// options.</param>
        string? Apply(string? text, object? options = null);
    }
}
