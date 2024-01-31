using Fusi.Text.Unicode;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace Cadmus.Graph;

/// <summary>
/// Triple value supplier for object-dependent data. This utility class can be
/// used when entering triples whose object is a literal value, in order to
/// supply the missing values for the triple's literal properties.
/// </summary>
public static class TripleObjectSupplier
{
    private static readonly UniData _ud = new();

    [return: NotNullIfNotNull(nameof(text))]
    private static string? Filter(string? text, bool numeric)
    {
        if (string.IsNullOrEmpty(text)) return text;

        StringBuilder sb = new(text.Length);
        foreach (char c in text)
        {
            if (char.IsWhiteSpace(c))
            {
                // append if not preceded by space
                if (sb.Length == 0 || sb[^1] != ' ') sb.Append(' ');
            }
            else if (numeric && (c == '+' || c== '-' || c == '.' || c == ','))
            {
                sb.Append(c);
            }
            else if (char.IsLetterOrDigit(c) || c == '\'')
            {
                sb.Append(char.ToLowerInvariant(_ud.GetSegment(c, true)));
            }
        }
        return sb.ToString();
    }

    private static bool SupplyNumber(Triple triple)
    {
        // from https://docs.microsoft.com/en-us/dotnet/standard/data/xml/mapping-xml-data-types-to-clr-types
        switch (triple.LiteralType)
        {
            case "xs:boolean":
            case "xsd:boolean":
                if (triple.LiteralNumber != null) return false;
                triple.LiteralNumber = LiteralHelper.ConvertToBoolean(
                    triple.ObjectLiteral!) ? 1 : 0;
                return false;
            case "xs:byte":
            case "xsd:byte":
            case "xs:short":
            case "xsd:short":
            case "xs:int":
            case "xsd:int":
            case "xs:integer":
            case "xsd:integer":
            case "xs:long":
            case "xsd:long":
            case "xs:float":
            case "xsd:float":
            case "xs:double":
            case "xsd:double":
            case "xs:unsignedByte":
            case "xsd:unsignedByte":
            case "xs:unsignedShort":
            case "xsd:unsignedShort":
            case "xs:unsignedInt":
            case "xsd:unsignedInt":
            case "xs:unsignedLong":
            case "xsd:unsignedLong":
                if (triple.LiteralNumber != null) return true;
                triple.LiteralNumber = Convert.ToDouble(triple.ObjectLiteral,
                    CultureInfo.InvariantCulture);
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Supplies literal-dependent values in the specified triple, when not
    /// specified.
    /// </summary>
    /// <param name="triple">The triple.</param>
    /// <param name="defaultLang">The default language to be set if any.</param>
    /// <exception cref="ArgumentNullException">triple</exception>
    public static void Supply(Triple triple, string? defaultLang = null)
    {
        ArgumentNullException.ThrowIfNull(triple);

        if (triple.ObjectLiteral == null) return;

        // supply number
        bool numeric = SupplyNumber(triple);

        // supply indexed form
        if (triple.ObjectLiteralIx == null)
            triple.ObjectLiteralIx = Filter(triple.ObjectLiteral, numeric);

        // supply language
        if (triple.LiteralLanguage == null && defaultLang != null)
            triple.LiteralLanguage = defaultLang;
    }
}
