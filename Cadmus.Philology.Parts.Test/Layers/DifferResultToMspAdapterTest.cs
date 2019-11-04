using Cadmus.Philology.Parts.Layers;
using DiffMatchPatch;
using Xunit;

namespace Cadmus.Philology.Parts.Test.Layers
{
    public sealed class DifferResultToMspAdapterTest
    {
        [Fact]
        public void Adapt_Equal_Ok()
        {
            DifferResultToMspAdapter adapter = new DifferResultToMspAdapter();

            var ops = adapter.Adapt(new Differ().Diffs("abc", "abc"));

            Assert.Empty(ops);
        }

        [Fact]
        public void Adapt_DelInitial_Ok()
        {
            DifferResultToMspAdapter adapter = new DifferResultToMspAdapter();

            var ops = adapter.Adapt(new Differ().Diffs("ischola", "schola"));

            Assert.Single(ops);
            // del
            MspOperation op = ops[0];
            Assert.Equal(MspOperator.Delete, op.Operator);
            Assert.Equal("1", op.RangeA.ToString());
            Assert.Equal("i", op.ValueA);
        }

        [Fact]
        public void Adapt_DelInternal_Ok()
        {
            DifferResultToMspAdapter adapter = new DifferResultToMspAdapter();

            var ops = adapter.Adapt(new Differ().Diffs("ahenus", "aenus"));

            Assert.Single(ops);
            // del
            MspOperation op = ops[0];
            Assert.Equal(MspOperator.Delete, op.Operator);
            Assert.Equal("2", op.RangeA.ToString());
            Assert.Equal("h", op.ValueA);
        }

        [Fact]
        public void Adapt_DelFinal_Ok()
        {
            DifferResultToMspAdapter adapter = new DifferResultToMspAdapter();

            var ops = adapter.Adapt(new Differ().Diffs("hocc", "hoc"));

            Assert.Single(ops);
            // del
            MspOperation op = ops[0];
            Assert.Equal(MspOperator.Delete, op.Operator);
            Assert.Equal("4", op.RangeA.ToString());
            Assert.Equal("c", op.ValueA);
        }

        [Fact]
        public void Adapt_DelMultiple_Ok()
        {
            DifferResultToMspAdapter adapter = new DifferResultToMspAdapter();

            var ops = adapter.Adapt(new Differ().Diffs("XabYcdZ", "abcd"));

            Assert.Equal(3, ops.Count);
            // X
            MspOperation op = ops[0];
            Assert.Equal(MspOperator.Delete, op.Operator);
            Assert.Equal("1", op.RangeA.ToString());
            Assert.Equal("X", op.ValueA);
            // Y
            op = ops[1];
            Assert.Equal(MspOperator.Delete, op.Operator);
            Assert.Equal("4", op.RangeA.ToString());
            Assert.Equal("Y", op.ValueA);
            // Z
            op = ops[2];
            Assert.Equal(MspOperator.Delete, op.Operator);
            Assert.Equal("7", op.RangeA.ToString());
            Assert.Equal("Z", op.ValueA);
        }

        [Fact]
        public void Adapt_InsInitial_Ok()
        {
            DifferResultToMspAdapter adapter = new DifferResultToMspAdapter();

            var ops = adapter.Adapt(new Differ().Diffs("eros", "heros"));

            Assert.Single(ops);
            // ins
            MspOperation op = ops[0];
            Assert.Equal(MspOperator.Insert, op.Operator);
            Assert.Equal("1×0", op.RangeA.ToString());
            Assert.Equal("h", op.ValueB);
        }

        [Fact]
        public void Adapt_InsInternal_Ok()
        {
            DifferResultToMspAdapter adapter = new DifferResultToMspAdapter();

            var ops = adapter.Adapt(new Differ().Diffs("meses", "menses"));

            Assert.Single(ops);
            // ins
            MspOperation op = ops[0];
            Assert.Equal(MspOperator.Insert, op.Operator);
            Assert.Equal("3×0", op.RangeA.ToString());
            Assert.Equal("n", op.ValueB);
        }

        [Fact]
        public void Adapt_InsFinal_Ok()
        {
            DifferResultToMspAdapter adapter = new DifferResultToMspAdapter();

            var ops = adapter.Adapt(new Differ().Diffs("viru", "virum"));

            Assert.Single(ops);
            // ins
            MspOperation op = ops[0];
            Assert.Equal(MspOperator.Insert, op.Operator);
            Assert.Equal("5×0", op.RangeA.ToString());
            Assert.Equal("m", op.ValueB);
        }

        [Fact]
        public void Adapt_InsMultiple_Ok()
        {
            DifferResultToMspAdapter adapter = new DifferResultToMspAdapter();

            var ops = adapter.Adapt(new Differ().Diffs("abcd", "XabYcdZ"));

            Assert.Equal(3, ops.Count);
            // X
            MspOperation op = ops[0];
            Assert.Equal(MspOperator.Insert, op.Operator);
            Assert.Equal("1×0", op.RangeA.ToString());
            Assert.Equal("X", op.ValueB);
            // Y
            op = ops[1];
            Assert.Equal(MspOperator.Insert, op.Operator);
            Assert.Equal("3×0", op.RangeA.ToString());
            Assert.Equal("Y", op.ValueB);
            // Z
            op = ops[2];
            Assert.Equal(MspOperator.Insert, op.Operator);
            Assert.Equal("5×0", op.RangeA.ToString());
            Assert.Equal("Z", op.ValueB);
        }

        [Fact]
        public void Adapt_RepInitial_Ok()
        {
            DifferResultToMspAdapter adapter = new DifferResultToMspAdapter();

            var ops = adapter.Adapt(new Differ().Diffs("bixit", "vixit"));

            Assert.Single(ops);
            // rep
            MspOperation op = ops[0];
            Assert.Equal(MspOperator.Replace, op.Operator);
            Assert.Equal("1", op.RangeA.ToString());
            Assert.Equal("b", op.ValueA);
            Assert.Equal("v", op.ValueB);
        }

        [Fact]
        public void Adapt_RepInternal_Ok()
        {
            DifferResultToMspAdapter adapter = new DifferResultToMspAdapter();

            var ops = adapter.Adapt(new Differ().Diffs("gaudis", "gaudes"));

            Assert.Single(ops);
            // rep
            MspOperation op = ops[0];
            Assert.Equal(MspOperator.Replace, op.Operator);
            Assert.Equal("5", op.RangeA.ToString());
            Assert.Equal("i", op.ValueA);
            Assert.Equal("e", op.ValueB);
        }

        [Fact]
        public void Adapt_RepFinal_Ok()
        {
            DifferResultToMspAdapter adapter = new DifferResultToMspAdapter();

            var ops = adapter.Adapt(new Differ().Diffs("victo", "victu"));

            Assert.Single(ops);
            // rep
            MspOperation op = ops[0];
            Assert.Equal(MspOperator.Replace, op.Operator);
            Assert.Equal("5", op.RangeA.ToString());
            Assert.Equal("o", op.ValueA);
            Assert.Equal("u", op.ValueB);
        }

        [Fact]
        public void Adapt_RepWithShorter_Ok()
        {
            DifferResultToMspAdapter adapter = new DifferResultToMspAdapter();

            var ops = adapter.Adapt(new Differ().Diffs("vicsit", "vixit"));

            Assert.Single(ops);
            // rep
            MspOperation op = ops[0];
            Assert.Equal(MspOperator.Replace, op.Operator);
            Assert.Equal("3×2", op.RangeA.ToString());
            Assert.Equal("cs", op.ValueA);
            Assert.Equal("x", op.ValueB);
        }

        [Fact]
        public void Adapt_RepMultiple_Ok()
        {
            DifferResultToMspAdapter adapter = new DifferResultToMspAdapter();

            var ops = adapter.Adapt(new Differ().Diffs("abcde", "XbYdZ"));

            Assert.Equal(3, ops.Count);
            // X
            MspOperation op = ops[0];
            Assert.Equal(MspOperator.Replace, op.Operator);
            Assert.Equal("1", op.RangeA.ToString());
            Assert.Equal("a", op.ValueA);
            Assert.Equal("X", op.ValueB);
            // Y
            op = ops[1];
            Assert.Equal(MspOperator.Replace, op.Operator);
            Assert.Equal("3", op.RangeA.ToString());
            Assert.Equal("c", op.ValueA);
            Assert.Equal("Y", op.ValueB);
            // Z
            op = ops[2];
            Assert.Equal(MspOperator.Replace, op.Operator);
            Assert.Equal("5", op.RangeA.ToString());
            Assert.Equal("e", op.ValueA);
            Assert.Equal("Z", op.ValueB);
        }
    }
}
