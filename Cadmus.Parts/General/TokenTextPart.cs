﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using Cadmus.Core;
using Cadmus.Core.Layers;
using Fusi.Tools.Config;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// Token-based text part, to be used as the base text for token-based
    /// text layers.
    /// Tag: <c>it.vedph.token-text</c>.
    /// </summary>
    /// <seealso cref="PartBase" />
    [Tag("it.vedph.token-text")]
    public sealed class TokenTextPart : PartBase, IHasText
    {
        /// <summary>
        /// Gets or sets the citation. This is an optional arbitrary string,
        /// used to virtually connect several text parts belonging to a bigger
        /// unit.
        /// </summary>
        public string Citation { get; set; }

        /// <summary>
        /// Gets the text lines.
        /// </summary>
        public List<TextLine> Lines { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenTextPart"/> class.
        /// Note that this part automatically gets a role ID equal to
        /// <see cref="PartBase.BASE_TEXT_ROLE_ID"/>.
        /// </summary>
        public TokenTextPart()
        {
            Lines = new List<TextLine>();
            RoleId = BASE_TEXT_ROLE_ID;
        }

        /// <summary>
        /// Get all the key=value pairs (pins) exposed by the implementor.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <returns>The pins: <c>line-count</c>, <c>citation</c>=citation if any.
        /// </returns>
        public override IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            List<DataPin> pins = new List<DataPin>
            {
                CreateDataPin("line-count",
                Lines?.Count.ToString(CultureInfo.InvariantCulture) ?? "0")
            };

            if (!string.IsNullOrEmpty(Citation))
                pins.Add(CreateDataPin("citation", Citation));

            return pins;
        }

        /// <summary>
        /// Gets the definitions of data pins used by the implementor.
        /// </summary>
        /// <returns>Data pins definitions.</returns>
        public override IList<DataPinDefinition> GetDataPinDefinitions()
        {
            return new List<DataPinDefinition>(new[]
            {
                new DataPinDefinition(DataPinValueType.Integer,
                    "line-count",
                    "The lines count."),
                new DataPinDefinition(DataPinValueType.String,
                    "citation",
                    "The citation if any.")
            });
        }

        /// <summary>
        /// Get a single string representing the whole text, line by line.
        /// </summary>
        /// <returns>Text.</returns>
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
            if (location.A.At > 0)
            {
                // if we must get the whole token get it and add markers if any
                if (wholeToken)
                {
                    // left part
                    sb.Append(token, 0, location.A.At - 1);

                    // marker
                    if (!string.IsNullOrEmpty(begMarker)) sb.Append(begMarker);

                    // right part (with end marker if not a range)
                    if (location.IsRange)
                    {
                        sb.Append(token, location.A.At - 1,
                            token.Length - location.A.At - 1);
                    }
                    else
                    {
                        sb.Append(token, location.A.At - 1,
                            location.A.Run);

                        if (!string.IsNullOrEmpty(endMarker)) sb.Append(endMarker);

                        if (location.A.At - 1 + location.A.Run
                            < token.Length)
                        {
                            sb.Append(token,
                                location.A.At - 1 + location.A.Run,
                                token.Length - location.A.At - 1
                                    + location.A.Run);
                        }
                    }
                }

                // else just add the token portion
                else
                {
                    sb.Append(location.IsRange
                        ? token.Substring(location.A.At - 1)
                        : token.Substring(location.A.At - 1,
                            location.A.Run));
                }
            }

            // not a partial token, just copy it
            else
            {
                sb.Append(token);
            }
        }

        private static void ExtractRangeLastTokenText(
            ITextLocation<TokenTextPoint> location, string token,
            bool wholeToken, string endMarker, StringBuilder sb)
        {
            if (location.B.At > 0)
            {
                // add token left portion
                sb.Append(token, 0, location.B.Run);

                // if we must get the whole token get also its right portion
                // and add markers if any
                if (wholeToken)
                {
                    if (!string.IsNullOrEmpty(endMarker)) sb.Append(endMarker);

                    if (location.B.At - 1 + location.B.Run < token.Length)
                    {
                        sb.Append(token,
                            location.B.At - 1 + location.B.Run,
                            token.Length - location.B.At - 1 + location.B.Run);
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
        /// Get a single string representing the text included in the specified
        /// coordinates.
        /// </summary>
        /// <param name="location">coordinates</param>
        /// <param name="wholeToken">true to extract the whole token, even if
        /// the coordinates refer to a portion of it; false to extract only the
        /// specified portion.</param>
        /// <param name="begMarker">marker to be inserted at the beginning of
        /// the token portion specified by <see cref="TokenTextPoint.At"/>,
        /// when <paramref name="wholeToken"/> is true. Null or empty for no 
        /// marker.</param>
        /// <param name="endMarker">marker to be inserted at the end of the
        /// token portion specified by <see cref="TokenTextPoint.At"/> and
        /// <see cref="TokenTextPoint.Run"/>, when <paramref name="wholeToken"/> 
        /// is true. Null or empty for no marker.</param>
        /// <returns>Multiline text.</returns>
        /// <exception cref="ArgumentNullException">null location</exception>
        public string GetText(ITextLocation<TokenTextPoint> location,
            bool wholeToken = false,
            string begMarker = "[", string endMarker = "]")
        {
            if (location == null)
                throw new ArgumentNullException(nameof(location));

            if (Lines == null) return "";

            StringBuilder sb = new StringBuilder();
            int lastY = location.IsRange ? location.B.Y : location.A.Y;

            for (int y = location.A.Y; y <= lastY; y++)
            {
                string[] tokens = Lines[y - 1].GetTokens();
                int start = 1;

                // if it's the 1st line extract 1st token or its portion
                if (y == location.A.Y)
                {
                    start = location.A.X;
                    ExtractFirstTokenText(location, tokens[start - 1],
                        wholeToken, begMarker, endMarker, sb);

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
                int end = y == lastY ? location.B.X : tokens.Length;

                // copy tokens
                for (int i = start; i <= end; i++)
                {
                    if (i > start || y == location.A.Y) sb.Append(' ');

                    if (y == lastY && i == end)
                    {
                        ExtractRangeLastTokenText(location, tokens[i - 1],
                            wholeToken, endMarker, sb);
                    }
                    else
                    {
                        sb.Append(tokens[i - 1]);
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"[TokenText] {Lines?.Count}";
        }
    }
}
