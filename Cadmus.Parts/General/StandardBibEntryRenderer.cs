using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// Standard bibliographic entry renderer. This is just a general-purpose
    /// renderer used to summarize a bibliographic entry.
    /// </summary>
    /// <seealso cref="IBibEntryRenderer" />
    public sealed class StandardBibEntryRenderer : IBibEntryRenderer
    {
        private const string SEP = " -- ";

        private void AppendAuthors(IList<BibAuthor> authors, StringBuilder sb)
        {
            sb.Append(string.Join("; ",
                from a in authors select a.ToString()));
            sb.Append(SEP);
        }

        private static void TrimEnd(StringBuilder sb)
        {
            int i = sb.Length;
            while (i > 0 && char.IsWhiteSpace(sb[i - 1])) i--;
            if (i < sb.Length) sb.Remove(i, sb.Length - i);
        }

        /// <summary>
        /// Renders the specified entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>String.</returns>
        /// <exception cref="ArgumentNullException">entry</exception>
        public string Render(BibEntry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            StringBuilder sb = new StringBuilder();

            // [type]
            sb.Append('[').Append(entry.TypeId).Append("] ");

            // author(s) --
            if (entry.Authors?.Length > 0) AppendAuthors(entry.Authors, sb);

            // title --
            if (!string.IsNullOrEmpty(entry.Title))
                sb.Append(entry.Title).Append(SEP);

            // contributors --
            if (entry.Contributors?.Length > 0)
                AppendAuthors(entry.Contributors, sb);

            // container,
            if (!string.IsNullOrEmpty(entry.Container))
                sb.Append(entry.Container).Append(", ");

            // number,
            if (!string.IsNullOrEmpty(entry.Number))
                sb.Append(entry.Number).Append(", ");

            // place
            if (!string.IsNullOrEmpty(entry.PlacePub))
                sb.Append(entry.PlacePub).Append(' ');

            // year
            if (entry.YearPub > 0)
                sb.Append(entry.YearPub).Append(' ');

            // [ed]
            if (entry.Edition > 0)
                sb.Append('[').Append(entry.Edition).Append("] ");

            // pages
            if (entry.FirstPage > 0)
            {
                sb.Append(entry.FirstPage);
                if (entry.LastPage > 0)
                    sb.Append('-').Append(entry.LastPage);
            }

            TrimEnd(sb);

            return sb.ToString();
        }
    }
}
