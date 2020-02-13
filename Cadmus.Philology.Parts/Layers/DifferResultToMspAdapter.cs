using DiffMatchPatch;
using Fusi.Tools.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cadmus.Philology.Parts.Layers
{
    /// <summary>
    /// Adapter for Google diff-match-patch (C# Differ) library.
    /// </summary>
    public sealed class DifferResultToMspAdapter :
        IDiffResultToMspAdapter<IList<Diff>>
    {
        /// <summary>
        /// Gets or sets a value indicating whether move operation is disabled.
        /// </summary>
        public bool IsMovDisabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether swap operation is disabled.
        /// </summary>
        // public bool IsSwpDisabled { get; set; }

        private void MapDiffsWithReplacements(IList<Diff> diffs,
            IList<Tuple<Diff, MspOperation>> output)
        {
            int start = 1;
            for (int i = 0; i < diffs.Count; i++)
            {
                if (diffs[i].operation == Operation.DELETE
                    && i + 1 < diffs.Count
                    && diffs[i + 1].operation == Operation.INSERT)
                {
                    output.Add(
                        Tuple.Create(diffs[i], new MspOperation
                        {
                            Operator = MspOperator.Replace,
                            RangeA = new TextRange(start, diffs[i].text.Length),
                            ValueA = diffs[i].text,
                            ValueB = diffs[i + 1].text
                        }));
                    start += diffs[i].text.Length;
                    i++;
                }
                else
                {
                    output.Add(Tuple.Create(diffs[i], (MspOperation)null));
                    if (diffs[i].operation == Operation.DELETE
                        || diffs[i].operation == Operation.EQUAL)
                    {
                        start += diffs[i].text.Length;
                    }
                }
            }
        }

        private void DetectMoves(List<Tuple<Diff, MspOperation>> mspDiffs)
        {
            // first look for INS..DEL
            for (int i = 0; i < mspDiffs.Count; i++)
            {
                // for each INS:
                if (mspDiffs[i].Item2?.Operator == MspOperator.Insert)
                {
                    MspOperation ins = mspDiffs[i].Item2;

                    // find a DEL with the same value
                    var nextDel = mspDiffs
                        .Skip(i + 1)
                        .FirstOrDefault(t =>
                            t.Item2?.Operator == MspOperator.Delete
                            && t.Item2.ValueA == ins.ValueB);

                    // if found, assume a MOV from DEL to INS
                    if (nextDel != null)
                    {
                        int nextDelIndex = mspDiffs.IndexOf(nextDel);
                        MspOperation del = mspDiffs[nextDelIndex].Item2;

                        mspDiffs[nextDelIndex] = Tuple.Create(mspDiffs[nextDelIndex].Item1,
                            new MspOperation
                            {
                                Operator = MspOperator.Move,
                                RangeA = del.RangeA,
                                RangeB = ins.RangeA,
                                ValueA = del.ValueA
                            });
                        mspDiffs.RemoveAt(i--);
                    }
                }
            }

            // then look for DEL..INS
            for (int i = 0; i < mspDiffs.Count; i++)
            {
                // for each DEL:
                if (mspDiffs[i].Item2?.Operator == MspOperator.Delete)
                {
                    // find an INS with the same value
                    MspOperation del = mspDiffs[i].Item2;
                    var nextIns = mspDiffs
                        .Skip(i + 1)
                        .FirstOrDefault(t =>
                            t.Item2?.Operator == MspOperator.Insert
                            && t.Item2.ValueB == del.ValueA);

                    // if found, assume a MOV from DEL to INS
                    if (nextIns != null)
                    {
                        MspOperation ins = nextIns.Item2;
                        int nextInsIndex = mspDiffs.IndexOf(nextIns);

                        mspDiffs[i] = Tuple.Create(mspDiffs[i].Item1,
                            new MspOperation
                            {
                                Operator = MspOperator.Move,
                                RangeA = del.RangeA,
                                RangeB = ins.RangeA,
                                ValueA = del.ValueA
                            });
                        mspDiffs.RemoveAt(nextInsIndex);
                    }
                }
            }
        }

        //private static string GetReversedString(string s) =>
        //    s.Aggregate(new StringBuilder(), (j, k) => j.Insert(0, k)).ToString();

        //private void DetectSwaps(List<Tuple<Diff, MspOperation>> mspDiffs)
        //{
        //    // TODO: eventually add swap. Cases are like this:
        //    // XaYb -> YaXb
        //    // del Xa ()
        //    // equ Y (Y)
        //    // ins aX (YaX)
        //    // equ b (YaXb)
        //}

        /// <summary>
        /// Adapt the result into a set of misspelling operations.
        /// </summary>
        /// <param name="result">result</param>
        /// <returns>
        /// zero or more misspelling operations
        /// </returns>
        /// <exception cref="ArgumentNullException">result</exception>
        public IList<MspOperation> Adapt(IList<Diff> result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            List<Tuple<Diff, MspOperation>> mspDiffs =
                new List<Tuple<Diff, MspOperation>>();

            MapDiffsWithReplacements(result, mspDiffs);

            int index = 0;
            for (int i = 0; i < mspDiffs.Count; i++)
            {
                if (mspDiffs[i].Item2 != null)
                {
                    index += mspDiffs[i].Item2.RangeA.Length;
                    continue;
                }

                Tuple<Diff, MspOperation> t = mspDiffs[i];
                switch (t.Item1.operation)
                {
                    case Operation.EQUAL:
                        index += t.Item1.text.Length;
                        break;

                    case Operation.DELETE:
                        mspDiffs[i] = Tuple.Create(t.Item1,
                            new MspOperation
                            {
                                Operator = MspOperator.Delete,
                                RangeA = new TextRange(index + 1, t.Item1.text.Length),
                                ValueA = t.Item1.text
                            });
                        index += t.Item1.text.Length;
                        break;

                    case Operation.INSERT:
                        mspDiffs[i] = Tuple.Create(t.Item1,
                            new MspOperation
                            {
                                Operator = MspOperator.Insert,
                                RangeA = new TextRange(index + 1, 0),
                                ValueB = t.Item1.text
                            });
                        break;
                }
            }

            if (!IsMovDisabled) DetectMoves(mspDiffs);
            // if (!IsSwpDisabled) DetectSwaps(mspDiffs);

            return mspDiffs.Where(t => t.Item2 != null)
                .Select(t => t.Item2)
                .ToList();
        }
    }
}
