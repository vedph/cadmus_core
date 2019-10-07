namespace Cadmus.Core.Layers
{
    /// <summary>
    /// Text location interface.
    /// </summary>
    /// <typeparam name="TPoint">type of point</typeparam>
    public interface ITextLocation<TPoint> where TPoint : ITextPoint
    {
        /// <summary>
        /// Gets or sets the primary point.
        /// </summary>
        TPoint Primary { get; set; }

        /// <summary>
        /// Gets or sets the optional secondary point.
        /// </summary>
        TPoint Secondary { get; set; }

        /// <summary>
        /// True if this location represents a range defined
        /// by both <see cref="Primary"/> and <see cref="Secondary"/>
        /// points.
        /// </summary>
        bool IsRange { get; }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        void Clear();

        /// <summary>
        /// Copies points values from the <paramref name="other"/> specified
        /// location.
        /// </summary>
        /// <param name="other">The other location.</param>
        void CopyFrom(ITextLocation<TPoint> other);

        /// <summary>
        /// True if the <paramref name="other"/> specified location overlaps
        /// any part of this location.
        /// </summary>
        /// <param name="other">The other location.</param>
        /// <returns>true on overlap</returns>
        bool Overlaps(ITextLocation<TPoint> other);
    }
}
