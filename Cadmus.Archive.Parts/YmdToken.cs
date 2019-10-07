using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cadmus.Archive.Parts
{
    /// <summary>
    /// YMD token used in parsing archive dates.
    /// </summary>
    internal sealed class YmdToken
    {
        private static readonly Regex _bracketedRegex =
            new Regex(@"^\s*(?<l>\[)?\s*(?<v>[^]]+)\s*(?<r>\])?\s*$");

        public static string[] MonthFullNames => new[]
        {
            "gennaio", "febbraio", "marzo", "aprile", "maggio", "giugno",
            "luglio", "agosto", "settembre", "ottobre", "novembre", "dicembre"
        };

        public static string[] MonthShortNames => new[]
        {
            "gen.", "feb.", "mar.", "apr.", "mag.", "giu.",
            "lug.", "ago.", "set.", "ott.", "nov.", "dic.",
            // obsolete abbreviations
            "genn.", "febb.", "lu.", "ag.", "sett."
        };

        public char Type { get; set; }
        public short Value { get; set; }
        public bool HasLeftBracket { get; set; }
        public bool HasRightBracket { get; set; }
        public bool IsInferred { get; set; }

        private static short GetValueFromFullMonth(string text)
        {
            int i = Array.IndexOf(MonthFullNames, text);
            return (short) (i > -1 ? i + 1 : 0);
        }

        private static short GetValueFromShortMonth(string text)
        {
            int i = Array.IndexOf(MonthShortNames, text);
            if (i == -1) return 0;
            switch (i)
            {
                case 12:
                    return 1;
                case 13:
                    return 2;
                case 14:
                    return 7;
                case 15:
                    return 8;
                case 16:
                    return 9;
                default:
                    return (short)(i + 1);
            }
        }

        /// <summary>
        /// Parses the specified text representing an YMD token, eventually
        /// surrounded by an opening and/or closing square bracket.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="type">The type of token to parse: <c>y</c>, <c>m</c>,
        /// <c>d</c>.</param>
        /// <returns>parsed token or null if invalid</returns>
        /// <exception cref="ArgumentNullException">null text</exception>
        public static YmdToken Parse(string text, char type)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            text = text.Trim().ToLowerInvariant();
            Match m = _bracketedRegex.Match(text);
            if (!m.Success) return null;

            YmdToken token = new YmdToken {Type = type};
            if (m.Groups["l"].Length > 0) token.HasLeftBracket = true;
            if (m.Groups["r"].Length > 0) token.HasRightBracket = true;
            string sValue = m.Groups["v"].Value.ToLowerInvariant();

            switch (char.ToLowerInvariant(type))
            {
                case 'y':
                    token.Value = short.Parse(sValue, CultureInfo.InvariantCulture);
                    break;

                case 'm':
                    if (sValue.Any(char.IsLetter))
                    {
                        short n = GetValueFromFullMonth(sValue);
                        if (n > 0)
                        {
                            token.Value = n;
                        }
                        else
                        {
                            string s = sValue.EndsWith(".", StringComparison.Ordinal)
                                ? sValue
                                : sValue + ".";
                            n = GetValueFromShortMonth(s);
                            if (n > 0) token.Value = n;
                        }
                    }
                    else token.Value = short.Parse(sValue, CultureInfo.InvariantCulture);
                    break;

                case 'd':
                    goto case 'y';
            }

            return token;
        }

        /// <summary>
        /// Returns a <see cref="String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{Type}={(HasLeftBracket? "[":"")}{Value}{(HasRightBracket ? "]" : "")}";
        }
    }
}
