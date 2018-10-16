namespace Cadmus.Core.Config
{
    /// <summary>
    /// Item's flag definition.
    /// </summary>
    /// <remarks>Flags are a generic, yet compact mechanism for adding some
    /// simple boolean tags to any item. It is just a bitfield where each bit
    /// has a different meaning, when used. The meaning of each bit is specified
    /// by this definition.</remarks>
    public class FlagDefinition : IFlagDefinition
    {
        /// <summary>
        /// Gets or sets the bit value, representing the ID of the flag.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the color key.
        /// </summary>
        /// <value>
        /// The color key, with format RRGGBB.
        /// </value>
        public string ColorKey { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{Id}: {Label}";
        }
    }
}
