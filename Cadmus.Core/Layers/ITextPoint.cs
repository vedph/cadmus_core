using System;

namespace Cadmus.Core.Layers
{
    /// <summary>
    /// Interface implemented by any object representing a single point in a text 
    /// location.
    /// </summary>
    public interface ITextPoint :
        IComparable<ITextPoint>, IEquatable<ITextPoint>, IComparable
    {
        /// <summary>
        /// Clear this location resetting all its values.
        /// </summary>
        void Clear();

        /// <summary>
        /// Read location values from the specified text into this point.
        /// </summary>
        /// <param name="text">text with location</param>
        void Read(string text);

        /// <summary>
        /// Copy location values from the specified source point into this point.
        /// </summary>
        /// <param name="point">source point</param>
        void CopyFrom(ITextPoint point);

        /// <summary>
        /// Compare this point to the specified one without taking into account
        /// token portions.
        /// </summary>
        /// <param name="other">the point to compare to this point</param>
        /// <returns>comparison result</returns>
        /// <exception cref="ArgumentNullException">null point</exception>
        int IntegralCompareTo(ITextPoint other);

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>new object</returns>
        ITextPoint Clone();
    }
}
