using Fusi.Text.Unicode;
using System.Text;

namespace Cadmus.Core
{
    /// <summary>
    /// A simple general purpose text filter for pin values. This preserves
    /// only letters, apostrophes and whitespaces, also removing any diacritics
    /// from the letters and lowercasing them. Whitespaces are flattened into
    /// spaces and normalized.
    /// </summary>
    public static class PinTextFilter
    {
        private static UniData _ud;

        /// <summary>
        /// Apply this filter to the specified text, by keeping only
        /// letters/apostrophe and whitespaces. All the diacritics are removed,
        /// and uppercase letters are lowercased. Whitespaces are normalized
        /// and flattened into space, and get trimmed if initial or final.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="preserveDigits">True to preserve also digits; false
        /// to discard them.</param>
        /// <returns>The filtered text.</returns>
        public static string Apply(string text, bool preserveDigits = false)
        {
            if (string.IsNullOrEmpty(text)) return text;

            StringBuilder sb = new StringBuilder();
            bool prevWS = true;
            if (_ud == null) _ud = new UniData();

            foreach (char c in text)
            {
                switch (c)
                {
                    case '\'':
                        sb.Append('\'');
                        prevWS = false;
                        break;
                    default:
                        if (char.IsWhiteSpace(c))
                        {
                            if (prevWS) break;
                            sb.Append(' ');
                            prevWS = true;
                            break;
                        }
                        if (char.IsLetter(c))
                        {
                            char seg = _ud.GetSegment(c, true);
                            if (seg != 0) sb.Append(char.ToLower(seg));
                            prevWS = false;
                            break;
                        }
                        if (preserveDigits && char.IsDigit(c))
                        {
                            sb.Append(c);
                            break;
                        }
                        prevWS = false;
                        break;
                }
            }

            // right trim
            if (sb.Length > 0 && sb[sb.Length - 1] == ' ')
                sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }
    }
}
