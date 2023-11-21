using Cadmus.Core.Storage;
using Fusi.Text.Unicode;
using Fusi.Tools.Configuration;
using System;
using System.Text;

namespace Cadmus.Core;

/// <summary>
/// Standard item sort key builder.
/// Tag: <c>item-sort-key-builder.standard</c>.
/// </summary>
/// <remarks>The standard item sort key builder just relies on item's
/// title, which is normalized by flattening any whitespace to a single
/// space, trimming the result at both ends, lowercasing all the letters
/// and removing any diacritics from them, and keeping only letters,
/// digits, and apostrophe.</remarks>
/// <seealso cref="IItemSortKeyBuilder" />
[Tag("item-sort-key-builder.standard")]
public sealed class StandardItemSortKeyBuilder : IItemSortKeyBuilder
{
    private readonly static UniData _ud = new();

    /// <summary>
    /// Builds the sort key from the specified item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="repository">The optional repository.</param>
    /// <returns>Sort key.</returns>
    /// <exception cref="ArgumentNullException">item</exception>
    public string BuildKey(IItem item, ICadmusRepository? repository)
    {
        ArgumentNullException.ThrowIfNull(item);

        StringBuilder sb = new();
        foreach (char c in item.Title ?? "")
        {
            if (char.IsWhiteSpace(c))
            {
                if (sb.Length > 0 && (sb[^1] != ' '))
                    sb.Append(' ');
            }
            else
            {
                if (char.IsDigit(c) || c == '\'')
                {
                    sb.Append(c);
                    continue;
                }
                if (char.IsLetter(c))
                {
                    char seg = _ud.GetSegment(char.ToLowerInvariant(c), true);
                    if (seg != '\0') sb.Append(seg);
                }
            }
        }

        return sb.ToString().TrimEnd();
    }
}
