using System;
using System.Collections.Generic;
using DiffPlex.Model;
using Fusi.Tools.Text;

namespace Cadmus.Philology.Parts.Layers
{
    /// <summary>
    /// Simple adapter for diffing results, making use of higher level notions of
    /// replacing, moving, and swapping characters in a word.
    /// </summary>
    public sealed class SimpleResultToMspAdapter : IDiffResultToMspAdapter
    {
        /// <summary>
        /// Adapt the result into a set of misspelling operations.
        /// </summary>
        /// <param name="result">result</param>
        /// <returns>zero or more misspelling operations</returns>
        /// <exception cref="ArgumentNullException">null result</exception>
        public IList<MspOperation> Adapt(DiffResult result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            // create the A and B strings, assuming that in char diffing each piece
            // is a single char.
            string a = string.Concat(result.PiecesOld);
            string b = string.Concat(result.PiecesNew);

            List<MspOperation> operations = new List<MspOperation>();
            int i = 0;
            while (i < result.DiffBlocks.Count)
            {
                DiffBlock block = result.DiffBlocks[i];

                // REPLACE: a-del.count > 0 && b-ins.count > 0
                if (block.DeleteCountA > 0 && block.InsertCountB > 0)
                {
                    operations.Add(new MspOperation
                    {
                        Operator = MspOperator.Replace,
                        RangeA = new TextRange(block.DeleteStartA, block.DeleteCountA),
                        RangeB = new TextRange(block.InsertStartB, block.InsertCountB),
                        ValueA = a.Substring(block.DeleteStartA, block.DeleteCountA),
                        ValueB = b.Substring(block.InsertStartB, block.InsertCountB)
                    });
                    i++;
                    continue;
                } // rep

                // DELETE: a-del.count > 0 && b-ins.count = 0
                if (block.DeleteCountA > 0 && block.InsertCountB == 0)
                {
                    // MOVE corner case: delete+insert
                    DiffBlock next = i + 1 < result.DiffBlocks.Count ?
                        result.DiffBlocks[i + 1] : null;
                    if (next?.DeleteCountA == 0
                        && next.InsertCountB > 0
                        && b.Substring(block.InsertStartB, block.InsertCountB)
                        == a.Substring(next.DeleteStartA, next.DeleteCountA))
                    {
                        operations.Add(new MspOperation
                        {
                            Operator = MspOperator.Move,
                            RangeA = new TextRange(
                                block.DeleteStartA, block.DeleteCountA),
                            RangeB = new TextRange(
                                next.InsertStartB, next.InsertCountB),
                            ValueA = a.Substring(
                                block.DeleteStartA, block.DeleteCountA),
                            ValueB = null
                        });
                        i += 2;
                        continue;
                    } // mov

                    operations.Add(new MspOperation
                    {
                        Operator = MspOperator.Delete,
                        RangeA = new TextRange(
                            block.DeleteStartA, block.DeleteCountA),
                        RangeB = new TextRange(
                            block.InsertStartB, block.InsertCountB),
                        ValueA = a.Substring(
                            block.DeleteStartA, block.DeleteCountA),
                        ValueB = null
                    });
                    i++;
                    continue;
                } // del

                // INSERT: a-del.count = 0 && b-ins.count > 0
                if (block.DeleteCountA == 0 && block.InsertCountB > 0)
                {
                    // SWAP corner case: insert+delete
                    DiffBlock next = i + 1 < result.DiffBlocks.Count ?
                        result.DiffBlocks[i + 1] : null;
                    string inserted;

                    if (next?.DeleteCountA > 0
                        && next.InsertCountB == 0
                        && Math.Abs(block.DeleteStartA - next.DeleteStartA) == 1
                        && (inserted = b.Substring(
                            block.InsertStartB, block.InsertCountB))
                        == a.Substring(next.DeleteStartA, next.DeleteCountA))
                    {
                        operations.Add(new MspOperation
                        {
                            Operator = MspOperator.Swap,
                            RangeA = new TextRange(
                                block.DeleteStartA, inserted.Length),
                            RangeB = new TextRange(
                                next.DeleteStartA, block.InsertCountB),
                            ValueA = a.Substring(
                                block.DeleteStartA, inserted.Length),
                            ValueB = inserted
                        });
                        i += 2;
                        continue;
                    } // swap

                    operations.Add(new MspOperation
                    {
                        Operator = MspOperator.Insert,
                        RangeA = new TextRange(
                            block.DeleteStartA, block.DeleteCountA),
                        RangeB = new TextRange(
                            block.InsertStartB, block.InsertCountB),
                        ValueA = null,
                        ValueB = b.Substring(
                            block.InsertStartB, block.InsertCountB)
                    });
                    i++;
                    continue;
                } // ins

                i++;
            }

            return operations;
        }
    }
}
