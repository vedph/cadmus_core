using Fusi.Tools.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Cadmus.Philology.Parts.Properties;
using System.Text;
using System;

namespace Cadmus.Philology.Parts.Layers
{
    /// <summary>
    /// Misspelling operation. A misspelling operation defines a single
    /// modification required to build an an output word form B from an input
    /// word form A.
    /// </summary>
    /// <remarks>
    /// <para>
    /// One or more of such operations describe the relationships between these
    /// forms in terms of the transform operations required to generate B from A,
    /// and are useful to express the details of such transformation. This is
    /// useful when dealing with a misspelled form A vs its standard orthography
    /// version B.</para>
    /// <para>Operations types:</para>
    /// <list type="bullet">
    /// <item>
    /// <term>delete:</term>
    /// <description> <c>"A"@NxN=</c> where <c>A</c> is optional: e.g.
    /// <c>@2x1=</c>: A-range (whose length is always &gt; 0), without
    /// B-value (which by definition is zero). The A-value can be optionally
    /// provided.
    /// </description>
    /// </item>
    /// <item>
    /// <term>insert</term>
    /// <description>@2x0=b: range A (whose length is always 0) with value B.
    /// </description>
    /// </item>
    /// <item>
    /// <term>replace</term>
    /// <description>@2x1=b: range A (whose length is always &gt; 0)
    /// with value B.
    /// </description>
    /// </item>
    /// <item>
    /// <term>move</term>
    /// <description>@2x1>@4: range A (whose length is always &gt; 0), range B
    /// (whose length is always 0).</description>
    /// </item>
    /// <item>
    /// <term>swap</term>
    /// <description>@2x1~@4x1: range A (whose length is always &gt; 0), range B
    /// (whose length is always &gt; 0).</description>
    /// </item>
    /// </list>
    /// All the operations also have the value A, which just labels the grabbed
    /// input text (except when inserting), and optionally have a tag (in []
    /// after the operation) and a note (all what follows the tag, or the
    /// operation when there is no tag).
    /// </remarks>
    public sealed class MspOperation
    {
        private static readonly Regex _tagRegex = new Regex("^[-0-9a-zA-Z_.]+$");

        private static readonly Regex _opRegex = new Regex(
            @"(?:""(?<va>[^""]+)"")?" +
            @"\@(?<ras>\d+)(?:[x×](?<ral>\d+))?" +
            @"(?<op>[=>~])" +
            @"(?:""(?<vb>[^""]+)"")?" +
            @"(?:\@(?<rbs>\d+)(?:[x×](?<rbl>\d+))?)?" +
            @"(?:\s*\[(?<tag>[^]]+)\])?" +
            @"(?:\s*\{(?<note>[^)]+)\})?");

        private MspOperator _operator;
        private string _tag;
        private string _note;
        private string _valueA;
        private string _valueB;

        #region Properties
        /// <summary>
        /// Operator.
        /// </summary>
        public MspOperator Operator
        {
            get { return _operator; }
            set
            {
                if (_operator == value) return;
                _operator = value;

                // coerce incompatible properties to the new operator
                switch (value)
                {
                    case MspOperator.Delete:
                        // RAL>0, VB=null
                        ValueB = null;
                        break;
                    case MspOperator.Insert:
                        // RAL=0, VB!=null
                        if (RangeA.Length > 0)
                            RangeA = new TextRange(RangeA.Start, 0);
                        break;
                    case MspOperator.Move:
                        // RAL>0, RBL=0, VB=null
                        if (RangeB.Length > 0)
                            RangeB = new TextRange(RangeB.Start, 0);
                        ValueB = null;
                        break;
                }
            }
        }

        /// <summary>
        /// The text range referred to the input text. This is required.
        /// </summary>
        public TextRange RangeA { get; set; }

        /// <summary>
        /// The text range referred to the output text. This is required only
        /// for move and swap.
        /// </summary>
        public TextRange RangeB { get; set; }

        /// <summary>
        /// The portion of input text (if any) grabbed by this operation.
        /// This is not an operational datum, but is used to label the grabbed
        /// input text, so that the operation is more readable for human users.
        /// </summary>
        /// <remarks>This value should include letters only; anyway, double-quotes
        /// characters are removed if present as they are reserved to be used
        /// as value delimiters. If empty, the value is coerced to null.</remarks>
        public string ValueA
        {
            get { return _valueA; }
            set
            {
                _valueA = string.IsNullOrEmpty(value) ? null : value.Replace("\"", "");
            }
        }

        /// <summary>
        /// The portion of output text (if any) of this operation. This is
        /// required only for insert and replace. It is present as a label
        /// for swap.
        /// </summary>
        /// <remarks>This value should include letters only; anyway, double-quotes
        /// characters are removed if present as they are reserved to be used
        /// as value delimiters. If empty, the value is coerced to null.</remarks>
        public string ValueB
        {
            get { return _valueB; }
            set
            {
                _valueB = string.IsNullOrEmpty(value) ? null : value.Replace("\"", "");
            }
        }

        /// <summary>
        /// An optional tag used to group and categorize misspellings operations.
        /// E.g. you might want to categorize an operation like
        /// <c>vowels.itacism</c>. A tag can include only letters A-Z or a-z,
        /// digits 0-9, underscore, dash, and dot.
        /// </summary>
        /// <exception cref="ArgumentException">Invalid tag.</exception>
        public string Tag
        {
            get { return _tag; }
            set
            {
                if (value != null && !_tagRegex.IsMatch(value))
                    throw new ArgumentException(nameof(value));
                _tag = value;
            }
        }

        /// <summary>
        /// An optional free short note to this operation. The note should
        /// not include braces, which are automatically dropped when setting
        /// this property; also, note's spaces are normalized using
        /// <see cref="SanitizeNote(string)"/>.
        /// </summary>
        public string Note
        {
            get { return _note; }
            set { _note = SanitizeNote(value); }
        }
        #endregion

        /// <summary>
        /// Sanitizes the text representing a <see cref="Note"/> so
        /// that it does not include braces (<c>{</c> and <c>}</c>).
        /// Also, the whitespaces are all flattened to simple spaces, and
        /// normalized (no whitespaces at start/end and no sequence of whitespaces).
        /// If the resulting sanitized string is empty, or it was null when
        /// received, null is returned.
        /// </summary>
        /// <param name="note">The note.</param>
        /// <returns>The sanitized note, eventually null.</returns>
        public static string SanitizeNote(string note)
        {
            if (string.IsNullOrWhiteSpace(note)) return null;

            StringBuilder sb = new StringBuilder(note);
            sb.Replace("{", "");
            sb.Replace("}", "");

            // flatten and normalize whitespaces
            bool wsAtRight = true;
            for (int i = sb.Length - 1; i > -1; i--)
            {
                if (char.IsWhiteSpace(sb[i]))
                {
                    if (wsAtRight)
                    {
                        sb.Remove(i, 1);
                    }
                    else
                    {
                        sb[i] = ' ';
                        wsAtRight = true;
                    }
                    continue;
                }
                wsAtRight = false;
            }
            if (sb.Length > 0 && sb[0] == ' ') sb.Remove(0, 1);

            return sb.Length > 0 ? sb.ToString() : null;
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns>Error message(s), or null if valid</returns>
        public string[] Validate()
        {
            List<string> errors = new List<string>();

            switch (_operator)
            {
                case MspOperator.Delete:
                    // RAL>0, VB=null
                    if (RangeA.Length == 0) errors.Add(Resources.MspDeleteWithRal0);
                    if (ValueB != null) errors.Add(Resources.MspDeleteWithVb);
                    break;
                case MspOperator.Insert:
                    // RAL=0, VB!=null
                    if (RangeA.Length > 0) errors.Add(Resources.MspInsertWithRalNon0);
                    if (ValueB == null) errors.Add(Resources.MspInsertWithoutVb);
                    break;
                case MspOperator.Replace:
                    // RAL>0, VB!=null
                    if (RangeA.Length == 0) errors.Add(Resources.MspReplaceWithRal0);
                    if (ValueB == null) errors.Add(Resources.MspReplaceWithoutVb);
                    break;
                case MspOperator.Move:
                    // RAL>0, RBL=0, VB=null
                    if (RangeA.Length == 0) errors.Add(Resources.MspMoveWithRal0);
                    if (RangeB.Length > 0) errors.Add(Resources.MspMoveWithRblNon0);
                    if (ValueB != null) errors.Add(Resources.MspMoveWithVb);
                    break;
                case MspOperator.Swap:
                    // RAL>0, RBL>0
                    if (RangeA.Length == 0) errors.Add(Resources.MspSwapWithRal0);
                    if (RangeB.Length == 0) errors.Add(Resources.MspSwapWithRbl0);
                    break;
            }
            return errors.Count == 0? null : errors.ToArray();
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            // ["A"]
            if (ValueA != null) sb.Append('"').Append(ValueA).Append('"');
            // @N[xN]
            sb.Append('@').Append(RangeA);

            // operator
            switch (_operator)
            {
                case MspOperator.Delete:
                    sb.Append('=');
                    break;

                case MspOperator.Replace:
                case MspOperator.Insert:
                    sb.Append("=\"").Append(ValueB).Append('"');
                    break;

                case MspOperator.Move:
                    sb.Append(">@").Append(RangeB);
                    break;

                case MspOperator.Swap:
                    sb.Append('~');
                    if (ValueB != null) sb.Append('"').Append(ValueB).Append('"');
                    sb.Append('@').Append(RangeB);
                    break;
            }

            // [tag]
            if (_tag != null) sb.Append(" [").Append(_tag).Append(']');
            // {note}
            if (_note != null) sb.Append(" {").Append(_note).Append('}');

            return sb.ToString();
        }

        private static int ParseRangeNumber(string text)
        {
            if (string.IsNullOrEmpty(text)) return 1;
            return int.Parse(text, CultureInfo.InvariantCulture);
        }

        private static void DetermineOperator(string text, MspOperation operation)
        {
            switch (text)
            {
                case "=":
                    if (operation.ValueB == null)
                    {
                        operation.Operator = MspOperator.Delete;
                        break;
                    }
                    operation.Operator = operation.RangeA.Length == 0?
                        MspOperator.Insert : MspOperator.Replace;
                    break;
                case ">":
                    operation.Operator = MspOperator.Move;
                    break;
                case "~":
                    operation.Operator = MspOperator.Swap;
                    break;
            }
        }

        /// <summary>
        /// Parses the specified text representing a misspelling transform
        /// operation.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>operation, or null if invalid text</returns>
        public static MspOperation Parse(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            Match m = _opRegex.Match(text);
            if (!m.Success) return null;

            MspOperation operation = new MspOperation
            {
                RangeA = new TextRange(
                    ParseRangeNumber(m.Groups["ras"].Value),
                    ParseRangeNumber(m.Groups["ral"].Value)),
                ValueA = m.Groups["va"].Length > 0 ? m.Groups["va"].Value : null,
                RangeB = new TextRange(
                    ParseRangeNumber(m.Groups["rbs"].Value),
                    ParseRangeNumber(m.Groups["rbl"].Value)),
                ValueB = m.Groups["vb"].Length > 0 ? m.Groups["vb"].Value : null,
                Tag = m.Groups["tag"].Length > 0 ? m.Groups["tag"].Value : null,
                Note = m.Groups["note"].Length > 0 ? m.Groups["note"].Value : null
            };
            DetermineOperator(m.Groups["op"].Value, operation);

            // range B is allowed only for move/swap
            if (operation._operator != MspOperator.Move
                && operation._operator != MspOperator.Swap)
            {
                operation.RangeB = TextRange.Empty;
            }

            // value B is allowed only for insert/replace/swap
            if (operation._operator != MspOperator.Insert
                && operation._operator != MspOperator.Replace
                && operation._operator != MspOperator.Swap)
            {
                operation.ValueB = null;
            }

            return operation;
        }
    }

    /// <summary>
    /// Misspelling operation operator type.
    /// </summary>
    public enum MspOperator
    {
        /// <summary>Delete</summary>
        Delete = 0,

        /// <summary>Replace</summary>
        Replace,

        /// <summary>Insert</summary>
        Insert,

        /// <summary>Move</summary>
        Move,

        /// <summary>Swap</summary>
        Swap
    }
}
