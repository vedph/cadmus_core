using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Cadmus.Core.Blocks;
using Cadmus.Core.Layers;
using Fusi.Tools.Config;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// Token-based text part, to be used with text layers.
    /// </summary>
    /// <seealso cref="PartBase" />
    [Tag("token-text")]
    public sealed class TokenTextPart : PartBase, IHasText
    {
        /// <summary>
        /// Gets the text lines.
        /// </summary>
        public List<TextLine> Lines { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenTextPart"/> class.
        /// </summary>
        public TokenTextPart()
        {
            Lines = new List<TextLine>();
        }

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// </summary>
        /// <returns>pins</returns>
        public override IEnumerable<DataPin> GetDataPins()
        {
            return Enumerable.Empty<DataPin>();
        }

        /// <summary>
        /// Get a single string representing the whole text, line by line.
        /// </summary>
        /// <returns>text</returns>
        public string GetText()
        {
            return Lines == null ?
                "" :
                string.Join(Environment.NewLine, from l in Lines select l.Text);
        }

        private static void ExtractFirstTokenText(
            ITextLocation<TokenTextPoint> location, string token,
            bool wholeToken, string begMarker, string endMarker, StringBuilder sb)
        {
            // is it partial?
            if (location.Primary.At > 0)
            {
                // if we must get the whole token get it and add markers if any
                if (wholeToken)
                {
                    // left part
                    sb.Append(token, 0, location.Primary.At - 1);

                    // marker
                    if (!string.IsNullOrEmpty(begMarker)) sb.Append(begMarker);

                    // right part (with end marker if not a range)
                    if (location.IsRange)
                    {
                        sb.Append(token, location.Primary.At - 1,
                            token.Length - location.Primary.At - 1);
                    }
                    else
                    {
                        sb.Append(token, location.Primary.At - 1,
                            location.Primary.Run);

                        if (!string.IsNullOrEmpty(endMarker)) sb.Append(endMarker);

                        if (location.Primary.At - 1 + location.Primary.Run < token.Length)
                        {
                            sb.Append(token,
                                location.Primary.At - 1 + location.Primary.Run,
                                token.Length - location.Primary.At - 1 + location.Primary.Run);
                        }
                    }
                }

                // else just add the token portion
                else
                {
                    sb.Append(location.IsRange
                        ? token.Substring(location.Primary.At - 1)
                        : token.Substring(location.Primary.At - 1, location.Primary.Run));
                }
            }

            // not a partial token, just copy it
            else
            {
                sb.Append(token);
            }
        }

        private static void ExtractRangeLastTokenText(ITextLocation<TokenTextPoint> location, string token,
            bool wholeToken, string endMarker, StringBuilder sb)
        {
            if (location.Secondary.At > 0)
            {
                // add token left portion
                sb.Append(token, 0, location.Secondary.Run);

                // if we must get the whole token get also its right portion and add markers if any
                if (wholeToken)
                {
                    if (!string.IsNullOrEmpty(endMarker)) sb.Append(endMarker);

                    if (location.Secondary.At - 1 + location.Secondary.Run < token.Length)
                    {
                        sb.Append(token,
                            location.Secondary.At - 1 + location.Secondary.Run,
                            token.Length - location.Secondary.At - 1 + location.Secondary.Run);
                    }
                }
            }

            // not a partial token, just copy it
            else
            {
                sb.Append(token);
            }
        }

        /// <summary>
        /// Get a single string representing the text included in the specified coordinates.
        /// </summary>
        /// <param name="location">coordinates</param>
        /// <param name="wholeToken">true to extract the whole token, even if coordinates refer to a portion of it;
        /// false to extract only the specified portion.</param>
        /// <param name="begMarker">marker to be inserted at the beginning of the token portion specified by 
        /// <see cref="TokenTextPoint.At"/>, when <paramref name="wholeToken"/> is true. Null or empty for no 
        /// marker.</param>
        /// <param name="endMarker">marker to be inserted at the end of the token portion specified by 
        /// <see cref="TokenTextPoint.At"/> and <see cref="TokenTextPoint.Run"/>, when <paramref name="wholeToken"/> 
        /// is true. Null or empty for no marker.</param>
        /// <returns>multiline text</returns>
        /// <exception cref="ArgumentNullException">null location</exception>
        public string GetText(ITextLocation<TokenTextPoint> location, bool wholeToken = false,
            string begMarker = "[", string endMarker = "]")
        {
            if (location == null) throw new ArgumentNullException(nameof(location));

            StringBuilder sb = new StringBuilder();
            int lastY = location.IsRange ? location.Secondary.Y : location.Primary.Y;

            for (int y = location.Primary.Y; y <= lastY; y++)
            {
                string[] tokens = Lines[y - 1].GetTokens();
                int start = 1;

                // if it's the 1st line extract 1st token or its portion
                if (y == location.Primary.Y)
                {
                    start = location.Primary.X;
                    ExtractFirstTokenText(location, tokens[start - 1], wholeToken, begMarker, endMarker, sb);

                    // if we have just 1 token selected return its text as extracted
                    if (!location.IsRange) return sb.ToString();

                    // else we're going to deal with a range
                    start++;
                } // 1st
                else
                {
                    sb.AppendLine();
                }

                // range: set end token
                Debug.Assert(location.IsRange);
                int end = y == lastY ? location.Secondary.X : tokens.Length;

                // copy tokens
                for (int i = start; i <= end; i++)
                {
                    if (i > start || y == location.Primary.Y) sb.Append(' ');

                    if (y == lastY && i == end)
                        ExtractRangeLastTokenText(location, tokens[i - 1], wholeToken, endMarker, sb);
                    else
                        sb.Append(tokens[i - 1]);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns a <see cref="String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(TokenTextPart)}: {Lines?.Count}";
        }
    }
}
