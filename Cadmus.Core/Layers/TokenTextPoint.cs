using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Cadmus.Core.Layers
{
    /// <summary>
    /// Token-based text point: this is the point building up token-based text
    /// location (<see cref="TokenTextLocation"/>).
    /// </summary>
    /// <remarks>
    /// <para>Token-based coordinates include two main components, <see cref="Y"/>
    /// and <see cref="X"/>; <c>Y</c> represents the atomic text block (e.g. a
    /// line) and <c>X</c> the token ordinal in this block.</para>
	/// <para>Further, optionally a text coordinate can refer to just a portion
    /// of a token, defined by the ordinal of its first character
    /// (<see cref="At"/>) and the number of characters to include starting
    /// from it (<see cref="Run"/>); for instance, when mapping a translation
	/// we might require to map the token "and" in "and he" from "isque" to
    /// just the last 3 characters ("que") of the unique token "isque". In this
    /// case we can set <c>At</c>=3 and <c>Run</c>=3.</para> 
	/// </remarks>
    public sealed class TokenTextPoint : ITextPoint
    {
        // sample: 12.3@2x3
        private static readonly Regex _pointRegex = new Regex(
            @"^(?<y>\d+)\.(?<x>\d+)" +
            @"(?:@(?<at>\d+)" +
            @"(?:[x×](?<run>\d+))?)?$", RegexOptions.IgnorePatternWhitespace);

        #region Properties
        /// <summary>
        /// Y (=block) coordinate.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// X (=token) coordinate.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Optional ordinal of character in token.
        /// </summary>
        /// <remarks>This property is used together with <see cref="Run"/> to
        /// define a region in the token referenced by <see cref="X"/>. For
        /// instance, when mapping a translation we might require to map the
        /// token "and" in "and he" from "isque" to just the last 3 characters
        /// ("que") of the unique token "isque". In this case we can set
        /// <c>At</c>=3 and <c>Run</c>=3.</remarks>
        /// <value>ordinal of token character or 0 if not used.</value>
        /// <exception cref="ArgumentOutOfRangeException">attempt to set to
        /// value &lt; 0</exception>
        public short At { get; set; }

        /// <summary>
        /// Optional count of characters run in token.
        /// </summary>
        /// <remarks>This property is used together with <see cref="At"/> to
        /// define a region in the token referenced by <see cref="X"/>. For
        /// instance, when mapping a translation we might require to map the
        /// token "and" in "and he" from "isque" to just the last 3 characters
        /// ("que") of the unique token "isque". In this case we can set
        /// <c>At</c>=3 and <c>Run</c>=3.</remarks>
        /// <value>count of characters to run from <see cref="At"/>, or 0 if
        /// not used.</value>
        /// <exception cref="ArgumentOutOfRangeException">attempt to set to
        /// value &lt; 0</exception>
        public short Run { get; set; }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenTextPoint"/> class.
        /// </summary>
        public TokenTextPoint()
        {
        }

        /// <summary>
        /// Build a new coords structure from the specified Y, X, at and run values.
        /// </summary>
        /// <param name="y">Y value</param>
        /// <param name="x">X value</param>
        /// <param name="at">at value (0 for whole token)</param>
        /// <param name="run">run value (0 for whole token)</param>
        /// <exception cref="ArgumentOutOfRangeException">invalid at/run value
        /// </exception>
        public TokenTextPoint(int y, int x, short at = 0, short run = 0)
        {
            Y = y;
            X = x;
            At = at;
            Run = run;
        }

        /// <summary>
        /// Clear this location resetting all its values.
        /// </summary>
        public void Clear()
        {
            Y = X = 0;
            At = Run = 0;
        }

        /// <summary>
        /// Read location values from the specified text into this point.
        /// </summary>
        /// <param name="text">text with location</param>
        /// <exception cref="ArgumentNullException">null text</exception>
        public void Read(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            Match m = _pointRegex.Match(text);
            if (!m.Success)
            {
                throw new ArgumentException(LocalizedStrings.Format(
                    Properties.Resources.InvalidTokenPoint, text));
            }

            Y = int.Parse(m.Groups["y"].Value, CultureInfo.InvariantCulture);
            X = int.Parse(m.Groups["x"].Value, CultureInfo.InvariantCulture);

            // at, run
            if (m.Groups["at"].Length > 0)
            {
                At = short.Parse(m.Groups["at"].Value, CultureInfo.InvariantCulture);
                Run = m.Groups["run"].Length > 0 ?
                    short.Parse(m.Groups["run"].Value, CultureInfo.InvariantCulture) :
                    (short)1;
            }
            else
            {
                At = Run = 0;
            }
        }

        /// <summary>
        /// Copy location values from the specified source point into this point.
        /// </summary>
        /// <param name="point">source point</param>
        /// <exception cref="ArgumentNullException">null source point</exception>
        public void CopyFrom(ITextPoint point)
        {
            if (!(point is TokenTextPoint p))
                throw new ArgumentNullException(nameof(point));

            Y = p.Y;
            X = p.X;
            At = p.At;
            Run = p.Run;
        }

        #region Text
        /// <summary>
        /// Textual representation of these coords.
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}.{1}", Y, X);
            if (At > 0)
            {
                sb.AppendFormat("@{0}", At);
                if (Run > 1) sb.AppendFormat("x", Run);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Parse coords text.
        /// </summary>
        /// <remarks>Coords text is in the form <c>Y.X</c> or <c>Y.X@AxR</c>
        /// or <c>Y.X@A</c> (in this case
        /// run defaults to 1).</remarks>
        /// <param name="text">text to parse</param>
        /// <exception cref="ArgumentNullException">null text</exception>
        /// <exception cref="ArgumentException">invalid text</exception>
        public static TokenTextPoint Parse(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            TokenTextPoint point = new TokenTextPoint();
            point.Read(text);
            return point;
        }
        #endregion

        #region Comparison
        /// <summary>
        /// Compare these coordinates to the specified coordinates.
        /// </summary>
        /// <param name="other">coordinates to compare</param>
        /// <returns>comparison result</returns>
        /// <exception cref="ArgumentNullException">null coords</exception>
        public int CompareTo(ITextPoint other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;

            if (!(other is TokenTextPoint o))
                throw new ArgumentNullException(nameof(other));

            if (Y != o.Y) return Y - o.Y;
            if (X != o.X) return X - o.X;

            if (At != o.At) return At - o.At;
            return Run - o.Run;
        }

        /// <summary>
        /// Compare this point to the specified one without taking into account
        /// token portions (i.e. <see cref="At"/> and <see cref="Run"/>).
        /// </summary>
        /// <param name="other">the point to compare to this point</param>
        /// <returns>comparison result</returns>
        /// <exception cref="ArgumentNullException">null point</exception>
        public int IntegralCompareTo(ITextPoint other)
        {
            if (!(other is TokenTextPoint o))
                throw new ArgumentNullException(nameof(other));

            if (Y != o.Y) return Y - o.Y;
            if (X != o.X) return X - o.X;
            return 0;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type
        /// and returns an integer that indicates whether the current instance
        /// precedes, follows, or occurs in the same position in the sort order
        /// as the other object.</summary>
        /// <returns>A value that indicates the relative order of the objects
        /// being compared. The return value has these meanings: less than zero:
        /// this instance precedes <paramref name="obj" /> in the sort order;
        /// zero: this instance occurs in the same position in the sort order
        /// as <paramref name="obj" />; greater than zero: this instance follows
        /// <paramref name="obj" /> in the sort order.</returns>
        /// <param name="obj">An object to compare with this instance. </param>
        /// <exception cref="ArgumentException"><paramref name="obj" /> is not
        /// the same type as this instance.</exception>
        public int CompareTo(object obj)
        {
            if (obj is null) return 1;
            if (ReferenceEquals(this, obj)) return 0;

            if (!(obj is TokenTextPoint))
            {
                throw new ArgumentException(
                    $"Object must be of type {nameof(TokenTextPoint)}");
            }

            return CompareTo((TokenTextPoint)obj);
        }
        #endregion

        #region IEquatable<TokenPoint> Members
        /// <summary>
        /// True if these coordinates are equal to the specified coordinates.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>True if equal.</returns>
        public bool Equals(ITextPoint other)
        {
            return CompareTo(other) == 0;
        }
        #endregion

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>new object</returns>
        public ITextPoint Clone()
        {
            return new TokenTextPoint(Y, X, At, Run);
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

            TokenTextPoint other = obj as TokenTextPoint;
            if (obj == null) return false;
            return Y == other.Y
                && X == other.X
                && At == other.At
                && Run == other.Run;
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
        public static bool operator ==(TokenTextPoint left, TokenTextPoint right)
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
        public static bool operator !=(TokenTextPoint left, TokenTextPoint right)
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
        public static bool operator <(TokenTextPoint left, TokenTextPoint right)
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
        public static bool operator <=(TokenTextPoint left, TokenTextPoint right)
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
        public static bool operator >(TokenTextPoint left, TokenTextPoint right)
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
        public static bool operator >=(TokenTextPoint left, TokenTextPoint right)
        {
            return left is null ? right is null : left.CompareTo(right) >= 0;
        }
    }
}
