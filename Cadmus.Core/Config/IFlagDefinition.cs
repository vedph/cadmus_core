namespace Cadmus.Core.Config
{
    /// <summary>
    /// Flag definition.
    /// </summary>
    public interface IFlagDefinition
    {
        /// <summary>
        /// Gets or sets the bit value, representing the ID of the flag.
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        string Label { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets or sets the color key.
        /// </summary>
        /// <value>
        /// The color key, with format RRGGBB.
        /// </value>
        string ColorKey { get; set; }
    }
}