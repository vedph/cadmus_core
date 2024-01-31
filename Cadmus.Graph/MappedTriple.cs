using System;

namespace Cadmus.Graph;

/// <summary>
/// A triple defined in a <see cref="NodeMapping"/>.
/// </summary>
public class MappedTriple
{
    /// <summary>
    /// Subject URI template.
    /// </summary>
    public string? S { get; set; }

    /// <summary>
    /// Predicate URI template.
    /// </summary>
    public string? P { get; set; }

    /// <summary>
    /// Object URI template.
    /// </summary>
    public string? O { get; set; }

    /// <summary>
    /// Object literal template. This is a string wrapped in <c>""</c>,
    /// eventually followed by a language code in the form <c>@lang</c>,
    /// or by a data type in the form <c>^^type</c>.
    /// When creating the mapping output, <see cref="LiteralHelper"/> will be
    /// used to parse this value accordingly.
    /// </summary>
    public string? OL { get; set; }

    /// <summary>
    /// Parses the specified text as a mapped triple.
    /// </summary>
    /// <param name="text">The text with form <c>S P O</c> or <c>S P "OL"</c>,
    /// <c>S P "OL"@lang</c>, <c>S P "OL"^^type</c>.</param>
    /// <returns>Triple or null if invalid.</returns>
    public static MappedTriple? Parse(string? text)
    {
        if (string.IsNullOrEmpty(text)) return null;

        // S
        int i = text.IndexOf(' ');
        if (i == -1) return null;
        string s = text[..i];

        // P
        while (i < text.Length && text[i] == ' ') i++;
        int pi = i;
        i = text.IndexOf(' ', pi);
        if (i == -1) return null;
        string p = text[pi..i];

        // O
        while (i < text.Length && text[i] == ' ') i++;
        if (i == text.Length) return null;
        string o = text[i..];
        return o.StartsWith("\"", StringComparison.Ordinal)
            ? new MappedTriple
            {
                S = s,
                P = p,
                OL = o
            }
            : new MappedTriple
            {
                S = s,
                P = p,
                O = o
            };
    }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance, and can be
    /// parsed with <see cref="MappedTriple.Parse(string?)"/>. A literal object
    /// is granted to be wrapped in <c>""</c>, either it's followed by a language
    /// or type identifier or not. This produces objects like <c>"sample"</c>,
    /// <c>"sample"^^en</c>, or <c>"123"^^xs:int</c>.
    /// </returns>
    public override string ToString()
    {
        string o;
        if (O != null)
        {
            o = O;
        }
        else
        {
            o = OL ?? "";
            if (!o.StartsWith('"')) o = $"\"{o}\"";
        }
        return $"{S} {P} {o}";
    }
}
