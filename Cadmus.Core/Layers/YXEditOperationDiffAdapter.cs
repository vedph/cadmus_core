using DiffMatchPatch;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cadmus.Core.Layers
{
    /// <summary>
    /// YX-coordinates-based edit operation diff adapter.
    /// This adapter is used to produce a list of <see cref="YXEditOperation"/>'s
    /// from a list of <see cref="Diff"/> operations.
    /// </summary>
    public sealed class YXEditOperationDiffAdapter :
        IEditOperationDiffAdapter<YXEditOperation>
    {
        /// <summary>
        /// Gets or sets a value indicating whether the move edit operation
        /// is enabled. Default is true.
        /// </summary>
        public bool IsMoveEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the replace edit operation
        /// is enabled. Default is true.
        /// </summary>
        public bool IsReplaceEnabled { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="YXEditOperationDiffAdapter"/>
        /// class.
        /// </summary>
        public YXEditOperationDiffAdapter()
        {
            IsMoveEnabled = true;
            IsReplaceEnabled = true;
        }

        private string GetEditOperation(Operation op)
        {
            switch (op)
            {
                case Operation.EQUAL: return YXEditOperation.EQU;
                case Operation.INSERT: return YXEditOperation.INS;
                case Operation.DELETE: return YXEditOperation.DEL;
                default: return null;
            }
        }

        private static int LocateNextOperationWithValue(
            IList<YXEditOperation> operations,
            int start,
            string value)
        {
            int i = start;
            while (i < operations.Count && operations[i].Value != value)
                i++;
            return i == operations.Count ? -1 : i;
        }

        private static int LocatePrevOperationWithValue(
            IList<YXEditOperation> operations,
            int start,
            string value)
        {
            int i = start;
            while (i > -1 && operations[i].Value != value)
                i--;
            return i;
        }

        private void DetectForwardMoves(IList<YXEditOperation> operations)
        {
            if (operations.Count == 0) return;
            int maxGroupId = operations.Max(o => o.GroupId);

            for (int i = 0; i < operations.Count - 1; i++)
            {
                if (operations[i].Operator == YXEditOperation.DEL)
                {
                    int j = LocateNextOperationWithValue(
                        operations, i + 1, operations[i].Value);
                    if (j > -1)
                    {
                        operations[i].Operator = YXEditOperation.MVD;
                        operations[i].GroupId = ++maxGroupId;
                        operations[j].Operator = YXEditOperation.MVI;
                        operations[j].GroupId = maxGroupId;
                    }
                }
            }
        }

        private void DetectBackwardMoves(IList<YXEditOperation> operations)
        {
            if (operations.Count == 0) return;
            int maxGroupId = operations.Max(o => o.GroupId);

            for (int i = operations.Count - 1; i > 0; i--)
            {
                if (operations[i].Operator == YXEditOperation.DEL)
                {
                    int j = LocatePrevOperationWithValue(
                    operations, i - 1, operations[i].Value);
                    if (j > -1)
                    {
                        operations[i].Operator = YXEditOperation.MVD;
                        operations[i].GroupId = ++maxGroupId;
                        operations[j].Operator = YXEditOperation.MVI;
                        operations[j].GroupId = maxGroupId;
                    }
                }
            }
        }

        private void DetectReplacememts(IList<YXEditOperation> operations)
        {
            for (int i = operations.Count - 1; i > 0; i--)
            {
                if (operations[i].Location == operations[i - 1].Location
                    && operations[i - 1].Operator == YXEditOperation.DEL
                    && operations[i].Operator == YXEditOperation.INS)
                {
                    operations[i - 1].Operator = YXEditOperation.REP;
                    operations[i - 1].OldValue = operations[i - 1].Value;
                    operations[i - 1].Value = operations[i].Value;
                    operations.RemoveAt(i);
                }
            }
        }

        private static YXEditOperation JoinOperations(int first, int last,
            IList<YXEditOperation> operations)
        {
            YXEditOperation joined = new YXEditOperation
            {
                Operator = YXEditOperation.REP,
                Location = operations[first].Location,
                OldLocation = operations[first].OldLocation
            };
            StringBuilder value = new StringBuilder();
            StringBuilder oldValue = new StringBuilder();

            for (int i = first; i <= last; i++)
            {
                switch (operations[i].Operator)
                {
                    case YXEditOperation.EQU:
                        value.Append(operations[i].Value);
                        oldValue.Append(operations[i].Value);
                        break;
                    case YXEditOperation.DEL:
                        oldValue.Append(operations[i].Value);
                        break;
                    case YXEditOperation.INS:
                        value.Append(operations[i].Value);
                        break;
                }
            }

            joined.Value = value.ToString();
            joined.OldValue = oldValue.ToString();

            return joined;
        }

        private static void JoinSplitTokens(IList<YXEditOperation> operations)
        {
            int i = operations.Count - 1;
            while (i > 0)
            {
                if (operations[i].OldLocation == operations[i - 1].OldLocation
                    && operations[i].Location == operations[i - 1].Location)
                {
                    string loc = operations[i].Location;
                    string oldLoc = operations[i].OldLocation;
                    int j = i - 1;
                    while (j > 0
                        && operations[j - 1].OldLocation == oldLoc
                        && operations[j - 1].Location == loc)
                    {
                        j--;
                    }

                    if (i - j > 1)
                    {
                        YXEditOperation joined = JoinOperations(j, i, operations);
                        for (int del = i; del >= j; del--) operations.RemoveAt(del);
                        operations.Insert(j, joined);
                        i = j - 1;
                    }
                    else i--;
                }
                else i--;
            }
        }

        /// <summary>
        /// Adapts the specified diffs list into a list of
        /// <see cref="YXEditOperation"/>'s.
        /// </summary>
        /// <param name="diffs">The diffs.</param>
        /// <returns>The edit operations.</returns>
        public IList<YXEditOperation> Adapt(IList<Diff> diffs)
        {
            List<YXEditOperation> operations = new List<YXEditOperation>();

            int y = 1, x = 1, oy = 1, ox = 1;
            StringBuilder token = new StringBuilder();

            foreach (Diff diff in diffs)
            {
                foreach (char c in diff.text)
                {
                    switch (c)
                    {
                        case ' ':
                            // space ends the current token
                            if (token.Length > 0)
                            {
                                operations.Add(new YXEditOperation
                                {
                                    OldLocation = $"{oy}.{ox}",
                                    Location = $"{y}.{x}",
                                    Operator = GetEditOperation(diff.operation),
                                    Value = token.ToString()
                                });
                                token.Clear();
                            }
                            // if this token was not removed, inc new-x
                            if (diff.operation != Operation.DELETE) x++;
                            // if this token was not inserted, inc old-x
                            if (diff.operation != Operation.INSERT) ox++;
                            break;

                        case '\r':
                            break;
                        case '\n':
                            // CR+LF or just LF end the current line;
                            // save a pending token
                            if (token.Length > 0)
                            {
                                operations.Add(new YXEditOperation
                                {
                                    OldLocation = $"{oy}.{ox}",
                                    Location = $"{y}.{x}",
                                    Operator = GetEditOperation(diff.operation),
                                    Value = token.ToString()
                                });
                            }
                            token.Clear();

                            // next line
                            if (diff.operation != Operation.DELETE)
                            {
                                y++;
                                x = 1;
                            }
                            if (diff.operation != Operation.INSERT)
                            {
                                oy++;
                                ox = 1;
                            }
                            break;
                        default:
                            token.Append(c);
                            break;
                    }
                }

                // add the last pending token if any
                if (token.Length > 0)
                {
                    operations.Add(new YXEditOperation
                    {
                        OldLocation = $"{oy}.{ox}",
                        Location = $"{y}.{x}",
                        Operator = GetEditOperation(diff.operation),
                        Value = token.ToString()
                    });
                    token.Clear();
                }
            }

            // ignore sub-token changes, joining all of them under a token-level
            // replacement
            JoinSplitTokens(operations);

            if (IsMoveEnabled)
            {
                DetectForwardMoves(operations);
                DetectBackwardMoves(operations);
            }

            if (IsReplaceEnabled) DetectReplacememts(operations);

            return operations;
        }
    }
}
