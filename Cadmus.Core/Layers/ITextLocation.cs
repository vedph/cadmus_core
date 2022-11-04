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
        TPoint A { get; set; }

        /// <summary>
        /// Gets or sets the optional secondary point.
        /// </summary>
        TPoint? B { get; set; }

        /// <summary>
        /// True if this location represents a range defined
        /// by both <see cref="A"/> and <see cref="B"/>
        /// points.
        /// </summary>
        bool IsRange { get; }

        /// <summary>
        /// True if the <paramref name="other"/> specified location overlaps
        /// any part of this location.
        /// </summary>
        /// <param name="other">The other location.</param>
        /// <returns>true on overlap</returns>
        bool Overlaps(ITextLocation<TPoint> other);
    }
}
