namespace Cadmus.Core
{
    /// <summary>
    /// Part data pin.
    /// </summary>
    /// <remarks>
    /// A data pin is just a name/value pair derived from an item's part.
    /// Most parts expose such pairs to the outer world mainly for indexing
    /// purposes.
    /// <para>
    /// For instance, a datation part might expose a numeric value representing
    /// its content. As each part is an independent entity, with its own model,
    /// it is only the part which can know which data could be extracted from
    /// it for indexing. Thus, typically parts implement this interface.
    /// </para>
    /// <para>
    /// The data pin is generic enough to represent different levels of
    /// granularity: it may just be a name/value property, or a more complex
    /// entity, e.g.a triple.
    /// </para>
    /// <para>
    /// Note that these pins are not stored with parts, but calculated, and
    /// stored in a separate index. The calculation is implemented in the
    /// part's own code.
    /// </para>
    /// </remarks>
    public class DataPin
    {
        /// <summary>
        /// Gets or sets the item identifier.
        /// </summary>
        public string? ItemId { get; set; }

        /// <summary>
        /// Gets or sets the part identifier.
        /// </summary>
        public string? PartId { get; set; }

        /// <summary>
        /// Gets or sets the optional role identifier.
        /// </summary>
        public string? RoleId { get; set; }

        /// <summary>
        /// Gets or sets the pin name.
        /// </summary>
        /// <value>The name.</value>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{ItemId}.{PartId}: {Name}=[{Value}]";
        }
    }
}