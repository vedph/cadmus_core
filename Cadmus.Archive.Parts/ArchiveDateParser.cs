using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Fusi.Tools;

namespace Cadmus.Archive.Parts
{
    /// <summary>
    /// A universal parser for Italian archive dates.
    /// </summary>
    public sealed class ArchiveDateParser
    {
        private readonly Regex _ymdSlashSepRegex;
        private readonly Regex _ymdDashSepRegex;
        private readonly Regex _dmyRegex;
        private readonly Regex _dmyNamedRegex;
        private readonly Regex _squaresRegex;
        private readonly Regex _centuryRegex;
        private readonly Regex _sineDataRegex;
        private readonly Regex _centPrefixModRegex;
        private readonly Regex _centSuffixModRegex;
        private readonly Regex _wsRegex;
        private readonly Regex _acRegex;
        private readonly Regex _decadeRegex;

        private readonly Regex[] _dmyRegexes;
        private readonly Regex[] _ymdRegexes;
        private readonly Regex[] _shortenedDmyRegexes;
        private readonly Regex[] _shortenedYmdRegexes;
        private readonly Regex _aboutRegex;
        private readonly Regex _antePostRegex;

        private readonly Regex _betweenRegex;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveDateParser"/>
        /// class.
        /// </summary>
        public ArchiveDateParser()
        {
            _ymdSlashSepRegex = new Regex(
                @"(?:/\d{1,2}/)|" +
                @"(?:(?<!-)[12]\d{3}/\d{1,2}\b)|" +
                @"(?:\b\d{1,2}/[12]\d{3}\b)");
            _ymdDashSepRegex = new Regex(
                @"(?:-\d{1,2}-)|(?:(?<!/)[12]\d{3}-\d{1,2}\b)");

            _dmyRegex = new Regex(
                @"\b((?:(?:\d{1,2})|(?:[a-z]{3,}\s*))[-/\.\s])[12]\d{3}[^\p{L}\d]*$",
                RegexOptions.IgnoreCase);
            _dmyNamedRegex = new Regex(
                "(?:(?:genn?|febb?|mar|apr|mag|giu|lug?|ago?|sett?|ott|nov|dic|" +
                @"gennaio|febbraio|marzo|aprile|maggio|giugno|luglio|agosto|" +
                @"settembre|ottobre|novembre|dicembre)\.)" +
                @"\s*[12]\d{3}\b", RegexOptions.IgnoreCase);

            _squaresRegex = new Regex(@"[\[\]]+");
            _sineDataRegex = new Regex(@"(?:senza data)|(?:s\.d\.)",
                RegexOptions.IgnoreCase);
            _centuryRegex = new Regex("[IVX]+");
            _centPrefixModRegex = new Regex(
                @"^\s*(ca\.?|circa|inizio|II?\s+metà|metà|fine)\b",
                RegexOptions.IgnoreCase);
            _centSuffixModRegex = new Regex(
                @"[IVX]+\s+(?:(?:sec\.|secolo)\s+)?(in\.|ex\.)",
                RegexOptions.IgnoreCase);

            _wsRegex = new Regex(@"\s+");
            _acRegex = new Regex(@"\ba\.C\.\b", RegexOptions.IgnoreCase);

            _decadeRegex = new Regex(
                @"anni\s+(1\d{3}|'[1-9]0|dieci|venti|trenta|quaranta|cinquanta|" +
                "sessanta|settanta|ottanta|novanta)", RegexOptions.IgnoreCase);

            _dmyRegexes = new[]
            {
                // style N like 20-12-1923
                new Regex(
                    @"(?:(?:(?<d>\[?\d{1,2}\]?)[^\d\[]+)?" +
                    @"(?<m>\[?\d{1,2}\]?)[^\d\[]+)?" +
                    @"(?<y>\[?[12]\d{3}\]?)"),

                // style S like 20 dic.1923
                new Regex(@"(?:(?<d>\[?\d{1,2}\]?[^\p{L}\[]+)?" +
                          @"(?<m>\[?(?:genn?|febb?|mar|apr|mag|giu|lug?|ago?|" +
                          @"sett?|ott|nov|dic)\.\]?[^\p{L}\d\[]+))?" +
                          @"(?<y>\[?[12]\d{3}\]?)", RegexOptions.IgnoreCase),

                // style F like 20 dicembre 1923
                new Regex(@"(?:(?<d>\[?\d{1,2}\]?[^\p{L}\[]+)?" +
                          @"(?<m>\[?(?:gennaio|febbraio|marzo|aprile|maggio|" +
                          @"giugno|luglio|agosto|settembre|ottobre|novembre|" +
                          @"dicembre)\]?[^\p{L}\d\[]+))?" +
                          @"(?<y>\[?[12]\d{3}\]?)", RegexOptions.IgnoreCase)
            };

            _ymdRegexes = new[]
            {
                // style N like 1923-12-20
                new Regex(@"(?<y>\[?[12]\d{3}\]?)(?:[^\d\[]+(?<m>\[?\d{1,2}\]?)" +
                          @"(?:[^\d\[]+(?<d>\[?\d{1,2}\]?))?)?"),

                // style S like 1923, dic. 20
                new Regex(@"(?<y>\[?[12]\d{3}\]?)" +
                          @"(?:[^\d\[\p{L}]+" +
                          @"(?<m>\[?(?:genn?|febb?|mar|apr|mag|giu|lug?|ago?" +
                          @"|sett?|ott|nov|dic)\.?\]?)" +
                          @"(?:[^\p{L}\d\[]+(?<d>\[?\d{1,2}\]?))?)?",
                          RegexOptions.IgnoreCase),

                // style F like 1923, dicembre 20
                new Regex(@"(?<y>\[?[12]\d{3}\]?)" +
                          @"(?:[^\d\[\p{L}]+" +
                          @"(?<m>\[?(?:gennaio|febbraio|marzo|aprile|maggio" +
                          @"|giugno|luglio|" +
                          @"agosto|settembre|ottobre|novembre|dicembre)\.?\]?)" +
                          @"(?:[^\p{L}\d\[]+(?<d>\[?\d{1,2}\]?))?)?",
                          RegexOptions.IgnoreCase)
            };

            _shortenedDmyRegexes = new[]
            {
                // style N like 30-05 (DM) or just 05 (M) or just 30 (D)
                new Regex(@"(?:(?<d>\d{1,2})[^\d](?<m>\d{1,2}))|" +
                          @"(?:[^-/\.\d]?(?<md>\d{1,2})[^-/\.\d]?)"),

                // style S like 30 mag. (DM) or just mag. (M) or just 30 (D)
                new Regex(@"(?:(?<d>\d{1,2})\s+" +
                          @"(?:(?<m>\[?(?:genn?|febb?|mar|apr|mag|giu|lug?" +
                          @"|ago?|sett?|ott|nov|dic)\.?\]?)))|" +
                          @"(?:(?<m>\[?(?:genn?|febb?|mar|apr|mag|giu|lug?" +
                          @"|ago?|sett?|ott|nov|dic)\.?\]?))|" +
                          @"(?:(?<d>\b\d{1,2}\b))",
                    RegexOptions.IgnoreCase),

                // style F like 30 maggio (DM) or just maggio (M) or just 30 (D)
                new Regex(@"(?:(?<d>\d{1,2})\s+" +
                          @"(?:(?<m>\[?(?:gennaio|febbraio|marzo|aprile|maggio" +
                          @"|giugno|luglio|agosto|settembre|ottobre|novembre" +
                          @"|dicembre)\.?\]?)))|" +
                          @"(?:(?<m>\[?(?:gennaio|febbraio|marzo|aprile|maggio" +
                          @"|giugno|luglio|agosto|settembre|ottobre|novembre" +
                          @"|dicembre)\.?\]?))|" +
                          @"(?:(?<d>\b\d{1,2}\b))",
                    RegexOptions.IgnoreCase),
            };

            _shortenedYmdRegexes = new[]
            {
                // style N like 06-28 (MD) or just 06 (M) or just 28 (D)
                new Regex(@"(?:(?<m>\d{1,2})[^\d](?<d>\d{1,2}))|" +
                          @"(?:[^-/\.\d]?(?<md>\d{1,2})[^-/\.\d]?)"),

                // style S like giu. 28 or just giu. (M) or just 28 (D)
                new Regex(
                    @"(?:(?<m>\[?(?:genn?|febb?|mar|apr|mag|giu|lug?|ago?|" +
                    @"sett?|ott|nov|dic)\.?\]?)\s+(?<d>\d{1,2}))|" +
                    @"(?:(?<m>\[?(?:genn?|febb?|mar|apr|mag|giu|lug?|ago?|sett?" +
                    @"|ott|nov|dic)\.?\]?))|" +
                    @"(?:(?<d>\b\d{1,2}\b))",
                    RegexOptions.IgnoreCase),

                // style F like giugno 28 or just giugno (M) or just 28 (D)
                new Regex(
                    @"(?:(?<m>\[?(?:gennaio|febbraio|marzo|aprile|maggio|giugno" +
                    @"|luglio|agosto|settembre|ottobre|novembre|dicembre)\.?\]?)" +
                    @"\s+(?<d>\d{1,2}))|" +
                    @"(?:(?<m>\[?(?:gennaio|febbraio|marzo|aprile|maggio|giugno" +
                    @"|luglio|agosto|settembre|ottobre|novembre|dicembre)\.?\]?))|" +
                    @"(?:(?<d>\b\d{1,2}\b))",
                    RegexOptions.IgnoreCase)
            };

            _aboutRegex = new Regex(@"\b(?:ca\.?|circa)\b",
                RegexOptions.IgnoreCase);
            _antePostRegex = new Regex(@"^\s*(ante|post)\b",
                RegexOptions.IgnoreCase);

            _betweenRegex = new Regex(@"tra\s+il\s+(.+?)\s+e\s+il\s+(.+)",
                RegexOptions.IgnoreCase);
        }

        private static DateMonthStyle DetectMonthStyle(string text)
        {
            if (YmdToken.MonthShortNames.Any(s =>
                text.IndexOf(s, StringComparison.OrdinalIgnoreCase) > -1))
            {
                return DateMonthStyle.ShortName;
            }

            if (YmdToken.MonthFullNames.Any(s =>
                text.IndexOf(s, StringComparison.OrdinalIgnoreCase) > -1))
            {
                return DateMonthStyle.FullName;
            }

            return DateMonthStyle.Undefined;
        }

        private char DetectYmdSeparator(string text)
        {
            if (_ymdSlashSepRegex.IsMatch(text)) return '/';
            if (_ymdDashSepRegex.IsMatch(text)) return '-';
            return '\0';
        }

        private Tuple<string, string> SplitRange(string text, char ymdSeparator)
        {
            // corner case: tra il ... e il ...
            Match m = _betweenRegex.Match(text);
            if (m.Success)
            {
                return Tuple.Create(
                   m.Groups[1].Value.Trim(),
                   m.Groups[2].Value.Trim());
            }

            string[] a = text.Split(new[] { " - ", " / " }, StringSplitOptions.None);
            if (a.Length == 2) return Tuple.Create(a[0], a[1]);

            a = ymdSeparator == '/'
                ? text.Split(new[] { '-' }, StringSplitOptions.None)
                : text.Split(new[] { '/' }, StringSplitOptions.None);

            return a.Length == 2 ? Tuple.Create(a[0].Trim(), a[1].Trim()) : null;
        }

        private ArchiveDatePoint ParseCenturyPoint(string text)
        {
            ArchiveDatePoint point = new ArchiveDatePoint
            {
                ValueType = DateValueType.Century
            };

            // prefix modifiers
            Match m = _centPrefixModRegex.Match(text);
            if (m.Success)
            {
                switch (_wsRegex.Replace(m.Groups[1].Value, " ").ToLowerInvariant())
                {
                    case "ca.":
                    case "circa":
                        point.Approximation = ApproximationType.About;
                        break;
                    case "inizio":
                        point.Approximation = ApproximationType.Beginning;
                        break;
                    case "i metà":
                        point.Approximation = ApproximationType.FirstHalf;
                        break;
                    case "metà":
                        point.Approximation = ApproximationType.Mid;
                        break;
                    case "ii metà":
                        point.Approximation = ApproximationType.SecondHalf;
                        break;
                    case "fine":
                        point.Approximation = ApproximationType.End;
                        break;
                }
                // remove matched prefix
                text = text.Substring(m.Length);
            }

            // postfix modifiers
            if (point.Approximation == ApproximationType.None)
            {
                m = _centSuffixModRegex.Match(text);
                if (m.Success)
                {
                    switch (m.Groups[1].Value.ToLowerInvariant())
                    {
                        case "in.":
                            point.Approximation = ApproximationType.Beginning;
                            break;
                        case "ex.":
                            point.Approximation = ApproximationType.End;
                            break;
                    }
                    // remove matched postfix
                    text = text.Remove(m.Groups[1].Index, m.Groups[1].Length);
                }
            }

            // parse value
            m = _centuryRegex.Match(text);
            if (!m.Success)
            {
                throw new ArgumentException(
                   LocalizedStrings.Format(
                       Properties.Resources.NoCenturyValue, text));
            }

            point.Value = (short) RomanNumber.FromRoman(m.Value);
            if (_acRegex.IsMatch(text)) point.Value = (short) -point.Value;

            return point;
        }

        private static ArchiveDatePoint ParseDecade(string text)
        {
            ArchiveDatePoint point = new ArchiveDatePoint
            {
                ValueType = DateValueType.Decade
            };

            switch (text.ToLowerInvariant())
            {
                case "dieci":
                    point.Value = 191;
                    break;
                case "venti":
                    point.Value = 192;
                    break;
                case "trenta":
                    point.Value = 193;
                    break;
                case "quaranta":
                    point.Value = 194;
                    break;
                case "cinquanta":
                    point.Value = 195;
                    break;
                case "sessanta":
                    point.Value = 196;
                    break;
                case "settanta":
                    point.Value = 197;
                    break;
                case "ottanta":
                    point.Value = 198;
                    break;
                case "novanta":
                    point.Value = 199;
                    break;
                default:
                    // 'NN or NNNN
                    point.Value = text[0] == '\''
                        ? (short)(short.Parse(text.Substring(1),
                             CultureInfo.InvariantCulture) / 10 + 190)
                        : (short)
                        (short.Parse(text, CultureInfo.InvariantCulture) / 10);
                    break;
            }
            return point;
        }

        private static void MarkInferred(YmdToken[] tokens)
        {
            bool bInBracket = false;
            foreach (YmdToken token in tokens)
            {
                if (token == null) continue;
                if (token.HasLeftBracket || bInBracket)
                {
                    token.IsInferred = true;
                    bInBracket = true;
                }
                if (token.HasRightBracket) bInBracket = false;
            }
        }

        private ArchiveDatePoint ParsePoint(string text, bool dmy,
            DateMonthStyle monthStyle)
        {
            // senza data or s.d.
            if (_sineDataRegex.IsMatch(text)) return null;

            // century
            if (text.IndexOf("secolo", StringComparison.OrdinalIgnoreCase) > -1 ||
                text.IndexOf("sec.", StringComparison.CurrentCultureIgnoreCase) > -1)
            {
                return ParseCenturyPoint(text);
            }

            // decade
            Match match = _decadeRegex.Match(text);
            if (match.Success) return ParseDecade(match.Groups[1].Value);

            // DMY/YMD
            ArchiveDatePoint point = new ArchiveDatePoint();

            // about
            if (_aboutRegex.IsMatch(text))
                point.Approximation = ApproximationType.About;

            int i = (int) (monthStyle == 0 ? 0 : monthStyle - 1);
            Regex r = dmy ? _dmyRegexes[i] : _ymdRegexes[i];
            match = r.Match(text);
            if (!match.Success) return null;

            YmdToken y = YmdToken.Parse(match.Groups["y"].Value, 'y');
            YmdToken m = YmdToken.Parse(match.Groups["m"].Value, 'm');
            YmdToken d = YmdToken.Parse(match.Groups["d"].Value, 'd');

            // take square brackets into account: this depends on the DMY/YMD order
            MarkInferred(dmy ? new[] {d, m, y} : new[] {y, m, d});

            point.Value = y.Value;
            point.IsYearInferred = y.IsInferred;

            if (m != null)
            {
                point.Month = m.Value;
                point.IsMonthInferred = m.IsInferred;
            }

            if (d != null)
            {
                point.Day = d.Value;
                point.IsDayInferred = d.IsInferred;
            }

            return point;
        }

        private void AdjustYmdForShortenedRange(Tuple<string, string> input,
            ArchiveDate date, DateMonthStyle monthStyle)
        {
            // YMD/YM can shorten Max
            if (date.B != null || date.A?.ValueType != DateValueType.Year ||
                date.A.Month == 0)
            {
                return;
            }

            int i = (int)(monthStyle == 0 ? 0 : monthStyle - 1);
            Match match = _shortenedYmdRegexes[i].Match(input.Item2);
            if (match.Success)
            {
                // copy YM from Min
                date.B = new ArchiveDatePoint
                {
                    ValueType = DateValueType.Year,
                    Value = date.A.Value,
                    IsYearInferred = date.A.IsYearInferred,
                    Month = date.A.Month,
                    IsMonthInferred = date.A.IsMonthInferred
                };

                // style N has groups d/m or md
                if (i == 0)
                {
                    if (match.Groups["md"].Length > 0)
                    {
                        if (date.A.Day > 0)
                        {
                            date.B.Day = short.Parse(match.Groups["md"].Value,
                                CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            date.B.Month = short.Parse(match.Groups["md"].Value,
                               CultureInfo.InvariantCulture);
                        }
                    }
                    else
                    {
                        date.B.Month = short.Parse(match.Groups["m"].Value,
                            CultureInfo.InvariantCulture);
                        date.B.Day = short.Parse(match.Groups["d"].Value,
                            CultureInfo.InvariantCulture);
                    }
                } // N
                else
                {
                    // styles S/F have groups m/d
                    if (match.Groups["m"].Length > 0)
                    {
                        YmdToken m = YmdToken.Parse(match.Groups["m"].Value, 'm');
                        date.B.Month = m.Value;
                    } //eif
                    if (match.Groups["d"].Length > 0)
                    {
                        YmdToken d = YmdToken.Parse(match.Groups["d"].Value, 'd');
                        date.B.Day = d.Value;
                    }
                } // !N
            }
        }

        private void AdjustDmyForShortenedRange(Tuple<string, string> input,
            ArchiveDate date, DateMonthStyle monthStyle)
        {
            if (date.A != null || date.B?.ValueType != DateValueType.Year ||
                date.B.Month == 0)
            {
                return;
            }

            int i = (int)(monthStyle == 0 ? 0 : monthStyle - 1);
            Match match = _shortenedDmyRegexes[i].Match(input.Item1);
            if (match.Success)
            {
                // copy YM from Max
                date.A = new ArchiveDatePoint
                {
                    ValueType = DateValueType.Year,
                    Value = date.B.Value,
                    IsYearInferred = date.B.IsYearInferred,
                    Month = date.B.Month,
                    IsMonthInferred = date.B.IsMonthInferred
                };

                // style N has groups d/m or md
                if (i == 0)
                {
                    if (match.Groups["md"].Length > 0)
                    {
                        if (date.B.Day > 0)
                        {
                            date.A.Day = short.Parse(match.Groups["md"].Value,
                               CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            date.A.Month = short.Parse(match.Groups["md"].Value,
                               CultureInfo.InvariantCulture);
                        }
                    }
                    else
                    {
                        date.A.Month = short.Parse(match.Groups["m"].Value,
                            CultureInfo.InvariantCulture);
                        date.A.Day = short.Parse(match.Groups["d"].Value,
                            CultureInfo.InvariantCulture);
                    }
                } // N
                else
                {
                    // styles S/F have groups m/d
                    if (match.Groups["m"].Length > 0)
                    {
                        YmdToken m = YmdToken.Parse(match.Groups["m"].Value, 'm');
                        date.A.Month = m.Value;
                    }
                    if (match.Groups["d"].Length > 0)
                    {
                        YmdToken d = YmdToken.Parse(match.Groups["d"].Value, 'd');
                        date.A.Day = d.Value;
                    }
                } // !N 
            }
        }

        private void AdjustForShortenedRange(Tuple<string,string> input,
            ArchiveDate date, bool dmy, DateMonthStyle monthStyle)
        {
            // if no match for Min/Max but we're inside a range, 
            // try with shortened ranges
            // DMY can shorten Min
            if (dmy) AdjustDmyForShortenedRange(input, date, monthStyle);
            else AdjustYmdForShortenedRange(input, date, monthStyle);
        }

        /// <summary>
        /// Parses the specified text, representing one or more dates or dates
        /// ranges.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>date(s) parsed</returns>
        /// <exception cref="ArgumentNullException">null text</exception>
        public IList<ArchiveDate> Parse(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            // split at `;` (TODO: prefilter to replace `,` as ranges
            // separator with `;`)
            DateMonthStyle monthStyle = DateMonthStyle.Undefined;
            char ymdSep = '\0';
            List<ArchiveDate> dates = new List<ArchiveDate>();

            // several dates (or dates ranges) are separated by ;
            foreach (string part in text.Split(new[] { ';' },
                StringSplitOptions.RemoveEmptyEntries))
            {
                // remove [ ] to simplify styles detection
                string purgedPart = _squaresRegex.Replace(part, "");

                // detect month style (unless already detected in another part)
                if (monthStyle == DateMonthStyle.Undefined)
                    monthStyle = DetectMonthStyle(purgedPart);

                // detect numeric M style (unless already detected in another part)
                if ((monthStyle == DateMonthStyle.Numeric ||
                     monthStyle == DateMonthStyle.Undefined) &&
                    ymdSep == '\0')
                {
                    ymdSep = DetectYmdSeparator(purgedPart);
                }

                // detect YMD order
                bool dmy = _dmyRegex.IsMatch(purgedPart) ||
                    _dmyNamedRegex.IsMatch(purgedPart);

                // split range pair if any
                var t = SplitRange(part, ymdSep);
                if (t != null)
                {
                    ArchiveDate date = new ArchiveDate
                    {
                        A = ParsePoint(t.Item1, dmy, monthStyle),
                        B = ParsePoint(t.Item2, dmy, monthStyle)
                    };

                    // corner case: shortened ranges
                    AdjustForShortenedRange(t, date, dmy, monthStyle);

                    dates.Add(date);
                } // rng
                else
                {
                    // ante/post can appear only before single-point dates
                    string pointText = part;
                    Match m = _antePostRegex.Match(part);
                    bool min = true, max = true;
                    if (m.Success)
                    {
                        pointText = part.Substring(m.Length);
                        if (string.Equals(m.Groups[1].Value, "ante",
                            StringComparison.InvariantCultureIgnoreCase))
                        {
                            min = false;
                        }
                        else
                        {
                            max = false;
                        }
                    }

                    ArchiveDatePoint point = ParsePoint(pointText, dmy, monthStyle);
                    if (point != null)
                    {
                        ArchiveDate date = new ArchiveDate();
                        if (min && max)
                        {
                            date.A = point;
                            date.B = point.Clone();
                        }
                        else
                        {
                            if (min) date.A = point;
                            else date.B = point;
                        }
                        dates.Add(date);
                    }
                } // !rng
            }

            return dates;
        }
    }

    /// <summary>
    /// The style used in expressing months in archive dates.
    /// </summary>
    public enum DateMonthStyle
    {
        /// <summary>Undefined (=use default)</summary>
        Undefined = 0,

        /// <summary>Numeric: N or NN (1-12)</summary>
        Numeric,

        /// <summary>Short name (e.g. <c>gen.</c>)</summary>
        ShortName,

        /// <summary>Full name (e.g. <c>gennaio</c></summary>
        FullName
    }
}
