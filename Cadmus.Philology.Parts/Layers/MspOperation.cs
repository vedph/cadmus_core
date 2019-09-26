using Fusi.Tools.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Cadmus.Philology.Parts.Properties;

namespace Cadmus.Philology.Parts.Layers
{
    /// <summary>
    /// Misspelling operation. A misspelling operation defines a single modification
    /// required to build an an output word form (B) from an input word form (A).
    /// One or more of such operations describe the relationships between these forms
    /// in terms of the transform operations required to generate B from A, and
    /// are useful to express the details of such transformation. This is useful
    /// when dealing with a misspelled form A vs its standard orthography version B.
    /// </summary>
    /// <remarks>
    /// Operations types:
    /// <list type="bullet">
    /// <item>
    /// <term>delete</term>
    /// <description>@2x1=: range A (whose length is always &gt; 0) without value B.
    /// </description>
    /// </item>
    /// <item>
    /// <term>insert</term>
    /// <description>@2x0=b: range A (whose length is always 0) with value B.</description>
    /// </item>
    /// <item>
    /// <term>replace</term>
    /// <description>@2x1=b: range A (whose length is always &gt; 0) with value B.
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
    /// All the operations also have the value A, which just labels the grabbed input
    /// text (except when inserting), and optionally have a tag (in [] after the operation)
    /// and a note (all what follows the tag, or the operation when there is no tag).
    /// </remarks>
    public sealed class MspOperation
    {
        private static readonly Regex _opRegex = new Regex(
            @"(?<va>[^@]+)?" +
            @"@(?:(?<raa>\d+)(?:[x×](?<ral>\d+))?)" +
            @"(?<op>[=>~])" +
            @"(?<vb>[^@\s]+)?" +
            @"(?:@(?<rba>\d+)(?:[x×](?<rbl>\d+))?)?" +
            @"(?:\s*\[(?<t>[^]]+)\])?" +
            @"(?:\s*(?<n>[^[].*))?");

        private MspOperator _operator;

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
        public string ValueA { get; set; }

        /// <summary>
        /// The portion of output text (if any) of this operation. This is required
        /// only for insert and replace. It is present as a label for swap.
        /// </summary>
        public string ValueB { get; set; }

        /// <summary>
        /// An optional tag used to group and categorize misspellings operations.
        /// E.g. you might want to categorize an operation like <c>vowels.itacism</c>.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// An optional free short note to this operation.
        /// </summary>
        public string Note { get; set; }

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
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            switch (_operator)
            {
                case MspOperator.Delete:
                    return $"{ValueA}@{RangeA}=";
                case MspOperator.Replace:
                    return $"{ValueA}@{RangeA}={ValueB}";
                case MspOperator.Insert:
                    return $"@{RangeA.Start}×0={ValueB}";
                case MspOperator.Move:
                    return $"{ValueA}@{RangeA}>@{RangeB.Start}×0";
                case MspOperator.Swap:
                    return $"{ValueA}@{RangeA}~{ValueB}@{RangeB}";
                default:
                    return "";
            }
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
        /// Parses the specified text representing a misspelling transform operation.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>operation, or null if invalid text</returns>
        public static MspOperation Parse(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;

            Match m = _opRegex.Match(text);
            if (!m.Success) return null;

            MspOperation operation = new MspOperation
            {
                RangeA = new TextRange(
                    ParseRangeNumber(m.Groups["raa"].Value),
                    ParseRangeNumber(m.Groups["ral"].Value)),
                ValueA = m.Groups["va"].Length > 0 ? m.Groups["va"].Value : null,
                RangeB = new TextRange(
                    ParseRangeNumber(m.Groups["rba"].Value),
                    ParseRangeNumber(m.Groups["rbl"].Value)),
                ValueB = m.Groups["vb"].Length > 0 ? m.Groups["vb"].Value : null,
                Tag = m.Groups["t"].Length > 0 ? m.Groups["t"].Value : null,
                Note = m.Groups["n"].Length > 0 ? m.Groups["n"].Value : null
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
