using System;

namespace Cadmus.Core.Layers;

/// <summary>
/// Token-based text location, including 1 or 2 <see cref="TokenTextPoint"/>'s.
/// This represents the location of either a single token
/// (when <see cref="B"/> is null), or the location of a range of
/// contiguous tokens (from <see cref="A"/> to <see cref="B"/>,
/// inclusive). Further, each of this points can locate only a portion of
/// its target, starting from a character ordinal and extending for the
/// specified run of characters.
/// </summary>
public sealed class TokenTextLocation : ITextLocation<TokenTextPoint>,
    IEquatable<TokenTextLocation>
{
    private TokenTextPoint _a;
    private TokenTextPoint? _b;

    /// <summary>
    /// Gets or sets the primary.
    /// </summary>
    /// <exception cref="ArgumentNullException">value</exception>
    public TokenTextPoint A
    {
        get => _a;
        set
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            _a = (TokenTextPoint)value.Clone();
        }
    }

    /// <summary>
    /// Gets or sets the secondary point.
    /// </summary>
    public TokenTextPoint? B
    {
        get { return (TokenTextPoint?)_b?.Clone(); }
        set { _b = (TokenTextPoint?)value?.Clone(); }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is a range.
    /// </summary>
    public bool IsRange => _b is not null;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenTextLocation"/> class.
    /// </summary>
    public TokenTextLocation()
    {
        _a = new TokenTextPoint();
    }

    #region Equality
    /// <summary>
    /// Indicates whether the current object is equal to another object
    /// of the same type.</summary>
    /// <returns>true if the current object is equal to the
    /// <paramref name="other" /> parameter; otherwise, false.</returns>
    /// <param name="other">An object to compare with this object.</param>
    public bool Equals(TokenTextLocation? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Equals(_a, other._a) && Equals(_b, other._b);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current
    /// object.
    /// </summary>
    /// <returns>true if the specified object is equal to the current object;
    /// otherwise, false.</returns>
    /// <param name="obj">The object to compare with the current
    /// object.</param>
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is TokenTextLocation location && Equals(location);
    }

    /// <summary>
    /// Serves as the default hash function.
    ///  </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            return (_a.GetHashCode() * 397)
                   ^ (B?.GetHashCode() ?? 0);
        }
    }
    #endregion

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>New instance.</returns>
    public TokenTextLocation Clone()
    {
        return new TokenTextLocation
        {
            _a = (TokenTextPoint)_a.Clone(),
            _b = (TokenTextPoint?)_b?.Clone()
        };
    }

    #region Comparison
    /// <summary>
    /// Compares the current instance with another object of the same type
    /// and returns an integer that indicates whether the current instance
    /// precedes, follows, or occurs in the same position in the sort order
    /// as the other object.
    /// </summary>
    /// <returns>A value that indicates the relative order of the objects
    /// being compared. The return value has these meanings: less than zero:
    /// this instance  precedes <paramref name="other" /> in the sort order;
    /// zero: this instance  occurs in the same position in the sort order
    /// as <paramref name="other" />; greater than zero: this instance
    /// follows <paramref name="other" /> in the sort order.</returns>
    /// <param name="other">An object to compare with this instance.</param>
    public int CompareTo(TokenTextLocation other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;

        int pt1Comparison = _a.CompareTo(other._a);
        if (pt1Comparison != 0) return pt1Comparison;

        if (_b is null && other._b is null) return 0;
        if (_b is null) return -1;
        return _b.CompareTo(other._b);
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
    /// as <paramref name="obj" />; greater than zero This instance follows
    /// <paramref name="obj" /> in the sort order.</returns>
    /// <param name="obj">An object to compare with this instance.</param>
    /// <exception cref="ArgumentException">
    /// <paramref name="obj" /> is not the same type as this instance.
    /// </exception>
    public int CompareTo(object obj)
    {
        if (obj is null) return 1;
        if (ReferenceEquals(this, obj)) return 0;

        if (obj is not TokenTextLocation)
        {
            throw new ArgumentException(
                $"Object must be of type {nameof(TokenTextLocation)}");
        }
        return CompareTo((TokenTextLocation) obj);
    }
    #endregion

    #region Overlap
    /// <summary>
    /// True if these layer item coords overlap with any of the tokens
    /// referenced by <paramref name="other"/>.
    /// </summary>
    /// <param name="other">the location to test for inclusion</param>
    /// <returns>true if this location overlaps the specified coords</returns>
    /// <exception cref="ArgumentNullException">null location</exception>
    public bool Overlaps(ITextLocation<TokenTextPoint> other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));

        // empty coords never overlaps
        if (other.A.Y == 0) return false;

        // (in the following comments A=this and B=other)
        // 1) if B is a point, cases are:
        //  1.1. A is a point: A overlaps with B (both points) if they're equal.
        //  1.2. A is a range: A (range) overlaps with B (point) 
        //       when A-left is <= B-left and A-right is >= B-right.
        if (!other.IsRange)
        {
            return !IsRange
                ? _a.IntegralCompareTo(other.A) == 0
                : _a.CompareTo(other.A) <= 0 && B?.CompareTo(other.A) >= 0;
        }

        // 2) if B is a range, cases are:
        //	2.1. A is a point: A (point) overlaps with B (range) when
        //       B-left <= A and B-right >= A
        //	2.2. A is a range: A (range) overlaps with B (range) 
        //       when B-right >= A-left and B-left <= A-right
        return !IsRange ?
            other.A.CompareTo(_a) <= 0 && other.B?.CompareTo(_a) >= 0 :
            other.B?.CompareTo(_a) >= 0 && other.A.CompareTo(B) <= 0;
    }

    /// <summary>
    /// True if this layer item coords contain the token identified by the
    /// specified text coords.
    /// </summary>
    /// <param name="y">y-coords</param>
    /// <param name="x">x-coords</param>
    /// <returns><c>true</c> if included</returns>
    public bool Overlaps(int y, int x)
    {
        // an empty coords cannot contain anything
        if (_a.Y == 0) return false;

        return Overlaps(new TokenTextLocation
        {
            A = new TokenTextPoint(y, x)
        });
    }
    #endregion

    /// <summary>
    /// Shift this location by the specified Y and/or X amount, returning
    /// the result into a new location.
    /// </summary>
    /// <param name="dy">amount to shift for Y (positive or negative,
    /// 0=no shift)</param>
    /// <param name="dx">amount to shift for X (positive or negative,
    /// 0=no shift)</param>
    /// <returns>The shifted location.</returns>
    public TokenTextLocation Shift(int dy, int dx)
    {
        return _a.Y == 0
            ? Clone()
            : new TokenTextLocation
            {
                _a = new TokenTextPoint(_a.Y + dy, _a.X + dx, _a.At, _a.Run),
                _b = _b is not null
                    ? new TokenTextPoint(_b.Y + dy, _b.X + dx, _b.At, _b.Run)
                    : null
            };
    }

    /// <summary>
    /// Parses the specified text representing a token-based location.
    /// </summary>
    /// <param name="text">The text, with form <c>primary-secondary</c>
    /// or just <c>primary</c>, where each of these points is expressed
    /// as specified by <see cref="TokenTextPoint.Parse"/>.</param>
    /// <returns>location</returns>
    /// <exception cref="ArgumentNullException">null text</exception>
    public static TokenTextLocation Parse(string text)
    {
        if (text == null) throw new ArgumentNullException(nameof(text));

        string[] a = text.Split('-');
        if (a.Length == 0) return new TokenTextLocation();

        return new TokenTextLocation
        {
            _a = TokenTextPoint.Parse(a[0]),
            _b = a.Length > 1 ? TokenTextPoint.Parse(a[1]) : null
        };
    }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return IsRange ? $"{_a}-{_b}" : _a.ToString();
    }
}
