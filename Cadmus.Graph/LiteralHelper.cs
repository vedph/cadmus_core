using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Cadmus.Graph;

/// <summary>
/// Helper for triple's literal value handling.
/// </summary>
public static partial class LiteralHelper
{
    [GeneratedRegex("(?:(?:\\^\\^(?<t>.+))|(?:\\@(?<l>[a-z]+)))?$",
        RegexOptions.Compiled)]
    private static partial Regex GetLitRegex();

    private static readonly Regex _litRegex = GetLitRegex();

    /// <summary>
    /// Converts the specified string value to boolean.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>True or false. False is returned also for invalid values.
    /// </returns>
    public static bool ConvertToBoolean(string? value)
    {
        return value?.ToLowerInvariant() switch
        {
            "1" => true,
            "0" => false,
            "true" => true,
            "false" => false,
            _ => false
        };
    }

    /// <summary>
    /// Parses the literal value of the specified triple, adjusting it
    /// accordingly.
    /// </summary>
    /// <param name="triple">The triple.</param>
    /// <exception cref="ArgumentNullException">triple</exception>
    public static void AdjustLiteral(Triple triple)
    {
        ArgumentNullException.ThrowIfNull(triple);

        if (triple.ObjectLiteral == null) return;

        // parse ^^type or @lang, removing them from the literal itself
        Match m = _litRegex.Match(triple.ObjectLiteral);
        if (m.Success)
        {
            // remove suffix
            triple.ObjectLiteral = triple.ObjectLiteral[..m.Index];
            // unwrap from ""
            triple.ObjectLiteral = triple.ObjectLiteral.Trim('"');

            // handle compatible numeric types
            if (m.Groups["t"].Length > 0)
            {
                triple.LiteralType = m.Groups["t"].Value;
                // from https://docs.microsoft.com/en-us/dotnet/standard/data/xml/mapping-xml-data-types-to-clr-types
                switch (triple.LiteralType)
                {
                    case "xs:boolean":
                    case "xsd:boolean":
                        triple.LiteralNumber =
                            ConvertToBoolean(triple.ObjectLiteral) ? 1 : 0;
                        break;
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
                        triple.LiteralNumber =
                            Convert.ToDouble(triple.ObjectLiteral,
                                             CultureInfo.InvariantCulture);
                        break;
                }
            }
            else if (m.Groups["l"].Length > 0)
            {
                triple.LiteralLanguage = m.Groups["l"].Value;
            }
        }
        else triple.ObjectLiteral = triple.ObjectLiteral.Trim('"');

        // filter
        StringBuilder sb = new(triple.ObjectLiteral.Length);
        foreach (char c in triple.ObjectLiteral)
        {
            if (char.IsLetter(c)) sb.Append(UidFilter.GetSegment(c));
            else if (c == '\'' || char.IsDigit(c) || char.IsWhiteSpace(c))
                sb.Append(c);
        }
        triple.ObjectLiteralIx = sb.ToString();
    }
}
