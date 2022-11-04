namespace Cadmus.Core
{
    /// <summary>
    /// Information about a <see cref="DataPin"/>.
    /// </summary>
    /// <seealso cref="DataPin" />
    public sealed class DataPinInfo : DataPin
    {
        /// <summary>
        /// Gets or sets the part type identifier.
        /// </summary>
        public string? PartTypeId { get; set; }
    }
}
