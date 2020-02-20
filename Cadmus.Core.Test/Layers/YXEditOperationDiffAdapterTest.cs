using Cadmus.Core.Layers;
using DiffMatchPatch;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cadmus.Core.Test.Layers
{
    public sealed class YXEditOperationDiffAdapterTest
    {
        private static IList<YXEditOperation> GetOperations(string a, string b)
        {
            diff_match_patch dmp = new diff_match_patch();
            List<Diff> diffs = dmp.diff_main(a, b);
            dmp.diff_cleanupSemanticLossless(diffs);
            YXEditOperationDiffAdapter adapter = new YXEditOperationDiffAdapter();

            return adapter.Adapt(diffs);
        }

        [Fact]
        public void Adapt_AllEqual_Ok()
        {
            const string a = "alpha beta\ngamma";
            const string b = a;

            IList<YXEditOperation> operations = GetOperations(a, b);

            Assert.Equal(3, operations.Count);
            Assert.True(operations.All(o => o.Operator == YXEditOperation.EQU));

            YXEditOperation op = operations[0];
            Assert.Equal("alpha", op.Value);
            Assert.Equal("1.1", op.Location);
            Assert.Equal("1.1", op.OldLocation);

            op = operations[1];
            Assert.Equal("beta", op.Value);
            Assert.Equal("1.2", op.Location);
            Assert.Equal("1.2", op.OldLocation);

            op = operations[2];
            Assert.Equal("gamma", op.Value);
            Assert.Equal("2.1", op.Location);
            Assert.Equal("2.1", op.OldLocation);
        }

        [Fact]
        public void Adapt_1TokenTo2_Ok()
        {
            const string a = "alpha";
            const string b = "x y";

            IList<YXEditOperation> operations = GetOperations(a, b);

            Assert.Equal(2, operations.Count);

            YXEditOperation op = operations[0];
            Assert.Equal(YXEditOperation.REP, op.Operator);
            Assert.Equal("alpha", op.OldValue);
            Assert.Equal("x", op.Value);
            Assert.Equal("1.1", op.Location);
            Assert.Equal("1.1", op.OldLocation);

            op = operations[1];
            Assert.Equal(YXEditOperation.INS, op.Operator);
            Assert.Equal("y", op.Value);
            Assert.Equal("1.2", op.Location);
            Assert.Equal("1.1", op.OldLocation);
        }

        [Fact]
        public void Adapt_2TokenTo1_Ok()
        {
            const string a = "alpha beta";
            const string b = "x";

            IList<YXEditOperation> operations = GetOperations(a, b);

            Assert.Equal(2, operations.Count);

            YXEditOperation op = operations[0];
            Assert.Equal(YXEditOperation.DEL, op.Operator);
            Assert.Equal("alpha", op.Value);
            Assert.Equal("1.1", op.Location);
            Assert.Equal("1.1", op.OldLocation);

            op = operations[1];
            Assert.Equal(YXEditOperation.REP, op.Operator);
            Assert.Equal("beta", op.OldValue);
            Assert.Equal("x", op.Value);
            Assert.Equal("1.1", op.Location);
            Assert.Equal("1.2", op.OldLocation);
        }

        #region Horizontal, Delete
        [Fact]
        public void Adapt_HrzDelFirst_Ok()
        {
            const string a = "alpha beta gamma\ndelta";
            const string b = "beta gamma\ndelta";

            IList<YXEditOperation> operations = GetOperations(a, b);

            Assert.Equal(4, operations.Count);

            YXEditOperation op = operations[0];
            Assert.Equal(YXEditOperation.DEL, op.Operator);
            Assert.Equal("alpha", op.Value);
            Assert.Equal("1.1", op.Location);
            Assert.Equal("1.1", op.OldLocation);

            op = operations[1];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("beta", op.Value);
            Assert.Equal("1.1", op.Location);
            Assert.Equal("1.2", op.OldLocation);

            op = operations[2];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("gamma", op.Value);
            Assert.Equal("1.2", op.Location);
            Assert.Equal("1.3", op.OldLocation);

            op = operations[3];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("delta", op.Value);
            Assert.Equal("2.1", op.Location);
            Assert.Equal("2.1", op.OldLocation);
        }

        [Fact]
        public void Adapt_HrzDelMid_Ok()
        {
            const string a = "alpha beta gamma\ndelta";
            const string b = "alpha gamma\ndelta";

            IList<YXEditOperation> operations = GetOperations(a, b);

            Assert.Equal(4, operations.Count);

            YXEditOperation op = operations[0];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("alpha", op.Value);
            Assert.Equal("1.1", op.Location);
            Assert.Equal("1.1", op.OldLocation);

            op = operations[1];
            Assert.Equal(YXEditOperation.DEL, op.Operator);
            Assert.Equal("beta", op.Value);
            Assert.Equal("1.2", op.Location);
            Assert.Equal("1.2", op.OldLocation);

            op = operations[2];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("gamma", op.Value);
            Assert.Equal("1.2", op.Location);
            Assert.Equal("1.3", op.OldLocation);

            op = operations[3];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("delta", op.Value);
            Assert.Equal("2.1", op.Location);
            Assert.Equal("2.1", op.OldLocation);
        }

        [Fact]
        public void Adapt_HrzDelLast_Ok()
        {
            const string a = "alpha beta gamma\ndelta";
            const string b = "alpha beta\ndelta";

            IList<YXEditOperation> operations = GetOperations(a, b);

            Assert.Equal(4, operations.Count);

            YXEditOperation op = operations[0];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("alpha", op.Value);
            Assert.Equal("1.1", op.Location);
            Assert.Equal("1.1", op.OldLocation);

            op = operations[1];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("beta", op.Value);
            Assert.Equal("1.2", op.Location);
            Assert.Equal("1.2", op.OldLocation);

            op = operations[2];
            Assert.Equal(YXEditOperation.DEL, op.Operator);
            Assert.Equal("gamma", op.Value);
            Assert.Equal("1.2", op.Location);
            Assert.Equal("1.3", op.OldLocation);

            op = operations[3];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("delta", op.Value);
            Assert.Equal("2.1", op.Location);
            Assert.Equal("2.1", op.OldLocation);
        }
        #endregion

        #region Horizontal, Replace
        [Fact]
        public void Adapt_HrzRepFirst_Ok()
        {
            const string a = "alpha beta gamma\ndelta";
            const string b = "x beta gamma\ndelta";

            IList<YXEditOperation> operations = GetOperations(a, b);

            Assert.Equal(4, operations.Count);

            YXEditOperation op = operations[0];
            Assert.Equal(YXEditOperation.REP, op.Operator);
            Assert.Equal("x", op.Value);
            Assert.Equal("alpha", op.OldValue);
            Assert.Equal("1.1", op.Location);
            Assert.Equal("1.1", op.OldLocation);

            op = operations[1];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("beta", op.Value);
            Assert.Equal("1.2", op.Location);
            Assert.Equal("1.2", op.OldLocation);

            op = operations[2];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("gamma", op.Value);
            Assert.Equal("1.3", op.Location);
            Assert.Equal("1.3", op.OldLocation);

            op = operations[3];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("delta", op.Value);
            Assert.Equal("2.1", op.Location);
            Assert.Equal("2.1", op.OldLocation);
        }

        [Fact]
        public void Adapt_HrzRepMid_Ok()
        {
            const string a = "alpha beta gamma\ndelta";
            const string b = "alpha x gamma\ndelta";

            IList<YXEditOperation> operations = GetOperations(a, b);

            Assert.Equal(4, operations.Count);

            YXEditOperation op = operations[0];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("alpha", op.Value);
            Assert.Equal("1.1", op.Location);
            Assert.Equal("1.1", op.OldLocation);

            op = operations[1];
            Assert.Equal(YXEditOperation.REP, op.Operator);
            Assert.Equal("x", op.Value);
            Assert.Equal("beta", op.OldValue);
            Assert.Equal("1.2", op.Location);
            Assert.Equal("1.2", op.OldLocation);

            op = operations[2];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("gamma", op.Value);
            Assert.Equal("1.3", op.Location);
            Assert.Equal("1.3", op.OldLocation);

            op = operations[3];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("delta", op.Value);
            Assert.Equal("2.1", op.Location);
            Assert.Equal("2.1", op.OldLocation);
        }

        [Fact]
        public void Adapt_HrzRepLast_Ok()
        {
            const string a = "alpha beta gamma\ndelta";
            const string b = "alpha beta x\ndelta";

            IList<YXEditOperation> operations = GetOperations(a, b);

            Assert.Equal(4, operations.Count);

            YXEditOperation op = operations[0];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("alpha", op.Value);
            Assert.Equal("1.1", op.Location);
            Assert.Equal("1.1", op.OldLocation);

            op = operations[1];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("beta", op.Value);
            Assert.Equal("1.2", op.Location);
            Assert.Equal("1.2", op.OldLocation);

            op = operations[2];
            Assert.Equal(YXEditOperation.REP, op.Operator);
            Assert.Equal("x", op.Value);
            Assert.Equal("gamma", op.OldValue);
            Assert.Equal("1.3", op.Location);
            Assert.Equal("1.3", op.OldLocation);

            op = operations[3];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("delta", op.Value);
            Assert.Equal("2.1", op.Location);
            Assert.Equal("2.1", op.OldLocation);
        }
        #endregion

        #region Horizontal, Insert
        [Fact]
        public void Adapt_HrzInsFirst_Ok()
        {
            const string a = "alpha beta\ngamma";
            const string b = "x alpha beta\ngamma";

            IList<YXEditOperation> operations = GetOperations(a, b);

            Assert.Equal(4, operations.Count);

            YXEditOperation op = operations[0];
            Assert.Equal(YXEditOperation.INS, op.Operator);
            Assert.Equal("x", op.Value);
            Assert.Equal("1.1", op.Location);
            Assert.Equal("1.1", op.OldLocation);

            op = operations[1];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("alpha", op.Value);
            Assert.Equal("1.2", op.Location);
            Assert.Equal("1.1", op.OldLocation);

            op = operations[2];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("beta", op.Value);
            Assert.Equal("1.3", op.Location);
            Assert.Equal("1.2", op.OldLocation);

            op = operations[3];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("gamma", op.Value);
            Assert.Equal("2.1", op.Location);
            Assert.Equal("2.1", op.OldLocation);
        }

        [Fact]
        public void Adapt_HrzInsMid_Ok()
        {
            const string a = "alpha beta\ngamma";
            const string b = "alpha x beta\ngamma";

            IList<YXEditOperation> operations = GetOperations(a, b);

            Assert.Equal(4, operations.Count);

            YXEditOperation op = operations[0];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("alpha", op.Value);
            Assert.Equal("1.1", op.Location);
            Assert.Equal("1.1", op.OldLocation);

            op = operations[1];
            Assert.Equal(YXEditOperation.INS, op.Operator);
            Assert.Equal("x", op.Value);
            Assert.Equal("1.2", op.Location);
            Assert.Equal("1.2", op.OldLocation);

            op = operations[2];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("beta", op.Value);
            Assert.Equal("1.3", op.Location);
            Assert.Equal("1.2", op.OldLocation);

            op = operations[3];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("gamma", op.Value);
            Assert.Equal("2.1", op.Location);
            Assert.Equal("2.1", op.OldLocation);
        }

        [Fact]
        public void Adapt_HrzInsLast_Ok()
        {
            const string a = "alpha beta\ngamma";
            const string b = "alpha beta x\ngamma";

            IList<YXEditOperation> operations = GetOperations(a, b);

            Assert.Equal(4, operations.Count);

            YXEditOperation op = operations[0];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("alpha", op.Value);
            Assert.Equal("1.1", op.Location);
            Assert.Equal("1.1", op.OldLocation);

            op = operations[1];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("beta", op.Value);
            Assert.Equal("1.2", op.Location);
            Assert.Equal("1.2", op.OldLocation);

            op = operations[2];
            Assert.Equal(YXEditOperation.INS, op.Operator);
            Assert.Equal("x", op.Value);
            Assert.Equal("1.3", op.Location);
            Assert.Equal("1.2", op.OldLocation);

            op = operations[3];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("gamma", op.Value);
            Assert.Equal("2.1", op.Location);
            Assert.Equal("2.1", op.OldLocation);
        }
        #endregion

        #region Vertical, Delete
        [Fact]
        public void Adapt_VrtDelFirst_Ok()
        {
            const string a = "alpha beta\ngamma\ndelta";
            const string b = "gamma\ndelta";

            IList<YXEditOperation> operations = GetOperations(a, b);

            Assert.Equal(4, operations.Count);

            YXEditOperation op = operations[0];
            Assert.Equal(YXEditOperation.DEL, op.Operator);
            Assert.Equal("alpha", op.Value);
            Assert.Equal("1.1", op.Location);
            Assert.Equal("1.1", op.OldLocation);

            op = operations[1];
            Assert.Equal(YXEditOperation.DEL, op.Operator);
            Assert.Equal("beta", op.Value);
            Assert.Equal("1.1", op.Location);
            Assert.Equal("1.2", op.OldLocation);

            op = operations[2];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("gamma", op.Value);
            Assert.Equal("1.1", op.Location);
            Assert.Equal("2.1", op.OldLocation);

            op = operations[3];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("delta", op.Value);
            Assert.Equal("2.1", op.Location);
            Assert.Equal("3.1", op.OldLocation);
        }

        [Fact]
        public void Adapt_VrtDelMid_Ok()
        {
            const string a = "alpha beta\ngamma\ndelta";
            const string b = "alpha beta\ndelta";

            IList<YXEditOperation> operations = GetOperations(a, b);

            Assert.Equal(4, operations.Count);

            YXEditOperation op = operations[0];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("alpha", op.Value);
            Assert.Equal("1.1", op.Location);
            Assert.Equal("1.1", op.OldLocation);

            op = operations[1];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("beta", op.Value);
            Assert.Equal("1.2", op.Location);
            Assert.Equal("1.2", op.OldLocation);

            op = operations[2];
            Assert.Equal(YXEditOperation.DEL, op.Operator);
            Assert.Equal("gamma", op.Value);
            Assert.Equal("2.1", op.Location);
            Assert.Equal("2.1", op.OldLocation);

            op = operations[3];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("delta", op.Value);
            Assert.Equal("2.1", op.Location);
            Assert.Equal("3.1", op.OldLocation);
        }

        [Fact]
        public void Adapt_VrtDelLast_Ok()
        {
            const string a = "alpha beta\ngamma\ndelta";
            const string b = "alpha beta\ngamma";

            IList<YXEditOperation> operations = GetOperations(a, b);

            Assert.Equal(4, operations.Count);

            YXEditOperation op = operations[0];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("alpha", op.Value);
            Assert.Equal("1.1", op.Location);
            Assert.Equal("1.1", op.OldLocation);

            op = operations[1];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("beta", op.Value);
            Assert.Equal("1.2", op.Location);
            Assert.Equal("1.2", op.OldLocation);

            op = operations[2];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("gamma", op.Value);
            Assert.Equal("2.1", op.Location);
            Assert.Equal("2.1", op.OldLocation);

            op = operations[3];
            Assert.Equal(YXEditOperation.DEL, op.Operator);
            Assert.Equal("delta", op.Value);
            Assert.Equal("2.1", op.Location);
            Assert.Equal("3.1", op.OldLocation);
        }
        #endregion

        #region Vertical, replace
        [Fact]
        public void Adapt_VrtRepFirst_Ok()
        {
            const string a = "alpha beta\ngamma\ndelta";
            const string b = "x\ngamma\ndelta";

            IList<YXEditOperation> operations = GetOperations(a, b);

            Assert.Equal(4, operations.Count);

            YXEditOperation op = operations[0];
            Assert.Equal(YXEditOperation.DEL, op.Operator);
            Assert.Equal("alpha", op.Value);
            Assert.Equal("1.1", op.Location);
            Assert.Equal("1.1", op.OldLocation);

            op = operations[1];
            Assert.Equal(YXEditOperation.REP, op.Operator);
            Assert.Equal("x", op.Value);
            Assert.Equal("beta", op.OldValue);
            Assert.Equal("1.1", op.Location);
            Assert.Equal("1.2", op.OldLocation);

            op = operations[2];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("gamma", op.Value);
            Assert.Equal("2.1", op.Location);
            Assert.Equal("2.1", op.OldLocation);

            op = operations[3];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("delta", op.Value);
            Assert.Equal("3.1", op.Location);
            Assert.Equal("3.1", op.OldLocation);
        }

        [Fact]
        public void Adapt_VrtRepMid_Ok()
        {
            const string a = "alpha beta\ngamma\ndelta";
            const string b = "alpha beta\nx y\ndelta";

            IList<YXEditOperation> operations = GetOperations(a, b);

            Assert.Equal(5, operations.Count);

            YXEditOperation op = operations[0];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("alpha", op.Value);
            Assert.Equal("1.1", op.Location);
            Assert.Equal("1.1", op.OldLocation);

            op = operations[1];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("beta", op.Value);
            Assert.Equal("1.2", op.Location);
            Assert.Equal("1.2", op.OldLocation);

            op = operations[2];
            Assert.Equal(YXEditOperation.REP, op.Operator);
            Assert.Equal("x", op.Value);
            Assert.Equal("gamma", op.OldValue);
            Assert.Equal("2.1", op.Location);
            Assert.Equal("2.1", op.OldLocation);

            op = operations[3];
            Assert.Equal(YXEditOperation.INS, op.Operator);
            Assert.Equal("y", op.Value);
            Assert.Equal("2.2", op.Location);
            Assert.Equal("2.1", op.OldLocation);

            op = operations[4];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("delta", op.Value);
            Assert.Equal("3.1", op.Location);
            Assert.Equal("3.1", op.OldLocation);
        }

        [Fact]
        public void Adapt_VrtRepEnd_Ok()
        {
            const string a = "alpha beta\ngamma\ndelta";
            const string b = "alpha beta\ngamma";

            IList<YXEditOperation> operations = GetOperations(a, b);

            Assert.Equal(4, operations.Count);

            YXEditOperation op = operations[0];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("alpha", op.Value);
            Assert.Equal("1.1", op.Location);
            Assert.Equal("1.1", op.OldLocation);

            op = operations[1];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("beta", op.Value);
            Assert.Equal("1.2", op.Location);
            Assert.Equal("1.2", op.OldLocation);

            op = operations[2];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("gamma", op.Value);
            Assert.Equal("2.1", op.Location);
            Assert.Equal("2.1", op.OldLocation);

            op = operations[3];
            Assert.Equal(YXEditOperation.DEL, op.Operator);
            Assert.Equal("delta", op.Value);
            Assert.Equal("2.1", op.Location);
            Assert.Equal("3.1", op.OldLocation);
        }
        #endregion

        #region Vertical, insert
        [Fact]
        public void Adapt_VrtInsFirst_Ok()
        {
            const string a = "alpha beta\ngamma";
            const string b = "x\nalpha beta\ngamma";

            IList<YXEditOperation> operations = GetOperations(a, b);

            Assert.Equal(4, operations.Count);

            YXEditOperation op = operations[0];
            Assert.Equal(YXEditOperation.INS, op.Operator);
            Assert.Equal("x", op.Value);
            Assert.Equal("1.1", op.Location);
            Assert.Equal("1.1", op.OldLocation);

            op = operations[1];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("alpha", op.Value);
            Assert.Equal("2.1", op.Location);
            Assert.Equal("1.1", op.OldLocation);

            op = operations[2];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("beta", op.Value);
            Assert.Equal("2.2", op.Location);
            Assert.Equal("1.2", op.OldLocation);

            op = operations[3];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("gamma", op.Value);
            Assert.Equal("3.1", op.Location);
            Assert.Equal("2.1", op.OldLocation);
        }

        [Fact]
        public void Adapt_VrtInsMid_Ok()
        {
            const string a = "alpha beta\ngamma";
            const string b = "alpha beta\nx\ngamma";

            IList<YXEditOperation> operations = GetOperations(a, b);

            Assert.Equal(4, operations.Count);

            YXEditOperation op = operations[0];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("alpha", op.Value);
            Assert.Equal("1.1", op.Location);
            Assert.Equal("1.1", op.OldLocation);

            op = operations[1];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("beta", op.Value);
            Assert.Equal("1.2", op.Location);
            Assert.Equal("1.2", op.OldLocation);

            op = operations[2];
            Assert.Equal(YXEditOperation.INS, op.Operator);
            Assert.Equal("x", op.Value);
            Assert.Equal("2.1", op.Location);
            Assert.Equal("2.1", op.OldLocation);

            op = operations[3];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("gamma", op.Value);
            Assert.Equal("3.1", op.Location);
            Assert.Equal("2.1", op.OldLocation);
        }

        [Fact]
        public void Adapt_VrtInsLast_Ok()
        {
            const string a = "alpha beta\ngamma";
            const string b = "alpha beta\ngamma\nx";

            IList<YXEditOperation> operations = GetOperations(a, b);

            Assert.Equal(4, operations.Count);

            YXEditOperation op = operations[0];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("alpha", op.Value);
            Assert.Equal("1.1", op.Location);
            Assert.Equal("1.1", op.OldLocation);

            op = operations[1];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("beta", op.Value);
            Assert.Equal("1.2", op.Location);
            Assert.Equal("1.2", op.OldLocation);

            op = operations[2];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("gamma", op.Value);
            Assert.Equal("2.1", op.Location);
            Assert.Equal("2.1", op.OldLocation);

            op = operations[3];
            Assert.Equal(YXEditOperation.INS, op.Operator);
            Assert.Equal("x", op.Value);
            Assert.Equal("3.1", op.Location);
            Assert.Equal("2.1", op.OldLocation);
        }
        #endregion

        #region Sub-token
        [Fact]
        public void Adapt_SubtokenDelFirst_Ok()
        {
            const string a = "alpha beta";
            const string b = "xlpha beta";

            IList<YXEditOperation> operations = GetOperations(a, b);

            Assert.Equal(2, operations.Count);

            YXEditOperation op = operations[0];
            Assert.Equal(YXEditOperation.REP, op.Operator);
            Assert.Equal("alpha", op.OldValue);
            Assert.Equal("xlpha", op.Value);
            Assert.Equal("1.1", op.Location);
            Assert.Equal("1.1", op.OldLocation);

            op = operations[1];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("beta", op.Value);
            Assert.Equal("1.2", op.Location);
            Assert.Equal("1.2", op.OldLocation);
        }

        [Fact]
        public void Adapt_SubtokenDelMid_Ok()
        {
            const string a = "alpha beta";
            const string b = "alpxa beta";

            IList<YXEditOperation> operations = GetOperations(a, b);

            Assert.Equal(2, operations.Count);

            YXEditOperation op = operations[0];
            Assert.Equal(YXEditOperation.REP, op.Operator);
            Assert.Equal("alpha", op.OldValue);
            Assert.Equal("alpxa", op.Value);
            Assert.Equal("1.1", op.Location);
            Assert.Equal("1.1", op.OldLocation);

            op = operations[1];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("beta", op.Value);
            Assert.Equal("1.2", op.Location);
            Assert.Equal("1.2", op.OldLocation);
        }

        [Fact]
        public void Adapt_SubtokenDelLast_Ok()
        {
            const string a = "alpha beta";
            const string b = "alphx beta";

            IList<YXEditOperation> operations = GetOperations(a, b);

            Assert.Equal(2, operations.Count);

            YXEditOperation op = operations[0];
            Assert.Equal(YXEditOperation.REP, op.Operator);
            Assert.Equal("alpha", op.OldValue);
            Assert.Equal("alphx", op.Value);
            Assert.Equal("1.1", op.Location);
            Assert.Equal("1.1", op.OldLocation);

            op = operations[1];
            Assert.Equal(YXEditOperation.EQU, op.Operator);
            Assert.Equal("beta", op.Value);
            Assert.Equal("1.2", op.Location);
            Assert.Equal("1.2", op.OldLocation);
        }
        #endregion
    }
}
