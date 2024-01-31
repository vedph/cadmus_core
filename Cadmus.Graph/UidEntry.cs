using System.Globalization;

namespace Cadmus.Graph;

/// <summary>
/// An entry in the UID lookup set.
/// </summary>
public class UidEntry
{
    /// <summary>
    /// Gets the entry numeric ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the SID.
    /// </summary>
    public string? Sid { get; set; }

    /// <summary>
    /// Gets or sets the unsuffixed portion of the SID.
    /// </summary>
    public string? Unsuffixed { get; set; }

    /// <summary>
    /// Gets or sets the numeric suffix added to <see cref="Unsuffixed"/>
    /// to build the full SID.
    /// </summary>
    public bool HasSuffix { get; set; }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return Unsuffixed + (HasSuffix
            ? Id.ToString(CultureInfo.InvariantCulture) : "");
    }
}
