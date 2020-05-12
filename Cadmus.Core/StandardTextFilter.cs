using Fusi.Text.Unicode;
using System.Text;

namespace Cadmus.Core
{
    /// <summary>
    /// A general purpose text filter, which can be used by parts when they
    /// need to normalize their pins values.
    /// This filter preserves only letters, digits, whitespaces (normalized),
    /// and apostrophe. Letters are all lowercase and without any diacritics.
    /// </summary>
    public static class StandardTextFilter
    {
        private static UniData _ud;

        /// <summary>
        /// Apply this filter to the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The filtered text.</returns>
        public static string Apply(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            if (_ud == null) _ud = new UniData();

            StringBuilder sb = new StringBuilder();
            bool prevWs = false;

            foreach (char c in text)
            {
                if (char.IsWhiteSpace(c))
                {
                    if (prevWs || sb.Length == 0) continue;
                    sb.Append(' ');
                    prevWs = true;
                }
                else
                {
                    prevWs = false;
                    switch (c)
                    {
                        case '\u2019':
                        case '\'':
                            sb.Append('\'');
                            break;
                        default:
                            if (char.IsLetter(c))
                            {
                                sb.Append(_ud.GetSegment(
                                    char.ToLowerInvariant(c), true));
                            }
                            else
                            {
                                if (char.IsDigit(c)) sb.Append(c);
                            }
                            break;
                    }
                }
            }
            return sb.ToString().TrimEnd();
        }
    }
}
