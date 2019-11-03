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
        public bool IsSwpDisabled { get; set; }

        private void MapDiffsWithReplacements(IList<Diff> diffs,
            IList<Tuple<Diff, MspOperation>> output)
        {
            int start = 1;
            for (int i = 0; i < diffs.Count; i++)
            {
                if (diffs[i].Operation == Operation.Delete
                    && i + 1 < diffs.Count
                    && diffs[i + 1].Operation == Operation.Insert)
                {
                    output.Add(
                        Tuple.Create(diffs[i], new MspOperation
                        {
                            Operator = MspOperator.Replace,
                            RangeA = new TextRange(start, diffs[i].Text.Length),
                            ValueA = diffs[i].Text,
                            ValueB = diffs[i + 1].Text
                        }));
                    start += diffs[i].Text.Length;
                    i++;
                }
                else
                {
                    output.Add(Tuple.Create(diffs[i], (MspOperation)null));
                    if (diffs[i].Operation == Operation.Delete
                        || diffs[i].Operation == Operation.Equal)
                    {
                        start += diffs[i].Text.Length;
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
                    // find a DEL with the same value
                    MspOperation ins = mspDiffs[i].Item2;
                    var next = mspDiffs
                        .Skip(i + 1)
                        .FirstOrDefault(t =>
                            t.Item2?.Operator == MspOperator.Delete
                            && t.Item2.ValueA == ins.ValueB);

                    // if found, assume a MOV from DEL to INS
                    if (next != null)
                    {
                        MspOperation del = mspDiffs[i].Item2;
                        int nextIndex = mspDiffs.IndexOf(next);

                        mspDiffs[nextIndex] = Tuple.Create(mspDiffs[nextIndex].Item1,
                            new MspOperation
                            {
                                Operator = MspOperator.Move,
                                RangeA = del.RangeA,
                                RangeB = ins.RangeA,
                                ValueA = del.ValueA,
                                ValueB = ins.ValueA
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
                    var next = mspDiffs
                        .Skip(i + 1)
                        .FirstOrDefault(t =>
                            t.Item2?.Operator == MspOperator.Insert
                            && t.Item2.ValueB == del.ValueA);

                    // if found, assume a MOV from DEL to INS
                    if (next != null)
                    {
                        MspOperation ins = mspDiffs[i].Item2;
                        int nextIndex = mspDiffs.IndexOf(next);

                        mspDiffs[i] = Tuple.Create(mspDiffs[i].Item1,
                            new MspOperation
                            {
                                Operator = MspOperator.Move,
                                RangeA = del.RangeA,
                                RangeB = ins.RangeA,
                                ValueA = del.ValueA,
                                ValueB = ins.ValueA
                            });
                        mspDiffs.RemoveAt(nextIndex);
                    }
                }
            }
        }

        private void DetectSwaps(List<Tuple<Diff, MspOperation>> mspDiffs)
        {
            // a swap is defined by a pair of REP's, where A/B
            // text values are swapped
            for (int i = 0; i < mspDiffs.Count; i++)
            {
                if (mspDiffs[i].Item2?.Operator == MspOperator.Replace)
                {
                    // this is the leftmost REP
                    MspOperation rep1 = mspDiffs[i].Item2;

                    var next = mspDiffs.Skip(i + 1).FirstOrDefault(t =>
                        t.Item2?.Operator == MspOperator.Replace
                        && t.Item2.ValueA == rep1.ValueB
                        && t.Item2.ValueB == rep1.ValueA);

                    if (next != null)
                    {
                        // this is the rightmost REP
                        MspOperation rep2 = next.Item2;

                        // leftmost REP becomes SWP
                        mspDiffs[i] = Tuple.Create(mspDiffs[i].Item1,
                            new MspOperation
                            {
                                Operator = MspOperator.Swap,
                                RangeA = rep2.RangeA,
                                RangeB = rep1.RangeB,
                                ValueA = rep2.ValueA,
                                ValueB = rep1.ValueB
                            });

                        // rightmost REP is removed
                        mspDiffs.Remove(next);
                    }
                }
            }
        }

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
                if (mspDiffs[i].Item2 != null) continue;

                Tuple<Diff, MspOperation> t = mspDiffs[i];
                switch (t.Item1.Operation)
                {
                    case Operation.Equal:
                        index += t.Item1.Text.Length;
                        break;

                    case Operation.Delete:
                        mspDiffs[i] = Tuple.Create(t.Item1,
                            new MspOperation
                            {
                                Operator = MspOperator.Delete,
                                RangeA = new TextRange(index + 1, t.Item1.Text.Length),
                                ValueA = t.Item1.Text
                            });
                        index += t.Item1.Text.Length;
                        break;

                    case Operation.Insert:
                        mspDiffs[i] = Tuple.Create(t.Item1,
                            new MspOperation
                            {
                                Operator = MspOperator.Insert,
                                RangeA = new TextRange(index + 1, 0),
                                ValueB = t.Item1.Text
                            });
                        break;
                }
            }

            if (!IsMovDisabled) DetectMoves(mspDiffs);
            if (!IsSwpDisabled) DetectSwaps(mspDiffs);

            return mspDiffs.Where(t => t.Item2 != null)
                .Select(t => t.Item2)
                .ToList();
        }
    }
}
