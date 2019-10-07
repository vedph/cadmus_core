using System;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// Keyword.
    /// </summary>
    /// <remarks>A keyword has an ISO 639-3 language ID and a value, and
    /// represents any relevant keyword linked to an item.</remarks>
    public sealed class Keyword :
        IEquatable<Keyword>, IComparable, IComparable<Keyword>
    {
        #region Properties
        /// <summary>
        /// Language (usually an ISO 639 3-letters code).
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Keyword text value.
        /// </summary>
        public string Value { get; set; }
        #endregion

        /// <summary>
        /// Textual representation of this keyword.
        /// </summary>
        /// <returns>string in format <c>language: value</c></returns>
        public override string ToString()
        {
            return $"[{Language}] {Value}";
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of
        /// the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/>
        /// parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Keyword other)
        {
            if (other == null) return false;
            return Language == other.Language && Value == other.Value;
        }

        /// <summary>
        /// Compare this keyword to another.
        /// </summary>
        /// <param name="other">other keyword</param>
        /// <returns>comparison result (first by language, then by value;
        /// case insensitive)</returns>
        /// <exception cref="ArgumentNullException">null keyword</exception>
        public int CompareTo(Keyword other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            return Language != other.Language
                ? string.CompareOrdinal(Language, other.Language)
                : string.Compare(Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Compare this keyword to another.
        /// </summary>
        /// <param name="obj">other keyword</param>
        /// <returns>comparison result (first by language, then by value;
        /// case insensitive)</returns>
        /// <exception cref="ArgumentNullException">null keyword</exception>
        public int CompareTo(object obj)
        {
            return CompareTo(obj as Keyword);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is
        /// equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with
        /// this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object" /> is equal
        /// to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is null)
            {
                return false;
            }

            Keyword other = obj as Keyword;
            if (obj == null) return false;

            return Language == other.Language && Value == other.Value;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing
        /// algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Keyword left, Keyword right)
        {
            if (left is null)
            {
                return right is null;
            }

            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Keyword left, Keyword right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator <(Keyword left, Keyword right)
        {
            return left is null ? right is object : left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator <=(Keyword left, Keyword right)
        {
            return left is null || left.CompareTo(right) <= 0;
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator >(Keyword left, Keyword right)
        {
            return left is object && left.CompareTo(right) > 0;
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator >=(Keyword left, Keyword right)
        {
            return left is null ? right is null : left.CompareTo(right) >= 0;
        }
    }
}
