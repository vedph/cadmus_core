using System.Text;
using System.Text.RegularExpressions;

namespace Cadmus.Graph;

/// <summary>
/// A node defined in a <see cref="NodeMapping"/>.
/// </summary>
public class MappedNode
{
    /// <summary>
    /// The node's UID template.
    /// </summary>
    public string? Uid { get; set; }

    /// <summary>
    /// The optional node's label template.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// The optional node's tag template.
    /// </summary>
    public string? Tag { get; set; }

    /// <summary>
    /// Parse the text representing a <see cref="MappedNode"/>. This consists
    /// of a URI, optionally followed by a couple of square brackets wrapping
    /// a label optionally followed by a tag prefixed by <c>|</c>.
    /// </summary>
    /// <param name="text">Text or null.</param>
    /// <returns>Node or null.</returns>
    public static MappedNode? Parse(string? text)
    {
        if (string.IsNullOrEmpty(text)) return null;

        Match m = Regex.Match(text,
            @"^(?<u>(?:(?!\[[^\[\]]+\]$).)*)(?:\[(?<l>[^]|]+)?(?:\|(?<t>[^]]+)\])?)?",
            RegexOptions.Compiled);
        if (!m.Success) return null;

        string label = m.Groups["l"].Value.Trim();
        string tag = m.Groups["t"].Value.Trim();

        return new MappedNode
        {
            Uid = m.Groups["u"].Value.Trim(),
            Label = label.Length == 0 ? null : label,
            Tag = tag.Length == 0 ? null : tag
        };
    }

    /// <summary>
    /// Convert this node into a parsable representation.
    /// </summary>
    /// <returns>String.</returns>
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append(Uid);

        if (Label != null || Tag != null)
        {
            sb.Append(" [");
            if (!string.IsNullOrEmpty(Label)) sb.Append(Label);
            if (!string.IsNullOrEmpty(Tag)) sb.Append('|').Append(Tag);
            sb.Append(']');
        }

        return sb.ToString();
    }
}
