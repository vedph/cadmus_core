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

            var ops = adapter.Adapt(new diff_match_patch().diff_main("abc", "abc"));

            Assert.Empty(ops);
        }

        [Fact]
        public void Adapt_DelInitial_Ok()
        {
            DifferResultToMspAdapter adapter = new DifferResultToMspAdapter();

            var ops = adapter.Adapt(new diff_match_patch().diff_main("ischola", "schola"));

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

            var ops = adapter.Adapt(new diff_match_patch().diff_main("ahenus", "aenus"));

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

            var ops = adapter.Adapt(new diff_match_patch().diff_main("hocc", "hoc"));

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

            var ops = adapter.Adapt(new diff_match_patch().diff_main("XabYcdZ", "abcd"));

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

            var ops = adapter.Adapt(new diff_match_patch().diff_main("eros", "heros"));

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

            var ops = adapter.Adapt(new diff_match_patch().diff_main("meses", "menses"));

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

            var ops = adapter.Adapt(new diff_match_patch().diff_main("viru", "virum"));

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

            var ops = adapter.Adapt(new diff_match_patch().diff_main("abcd", "XabYcdZ"));

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

            var ops = adapter.Adapt(new diff_match_patch().diff_main("bixit", "vixit"));

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

            var ops = adapter.Adapt(new diff_match_patch().diff_main("gaudis", "gaudes"));

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

            var ops = adapter.Adapt(new diff_match_patch().diff_main("victo", "victu"));

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

            var ops = adapter.Adapt(new diff_match_patch().diff_main("vicsit", "vixit"));

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

            var ops = adapter.Adapt(new diff_match_patch().diff_main("abcde", "XbYdZ"));

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

        [Fact]
        public void Adapt_InsRepDel_Ok()
        {
            DifferResultToMspAdapter adapter = new DifferResultToMspAdapter();

            var ops = adapter.Adapt(new diff_match_patch().diff_main("bxdf", "AbCd"));

            Assert.Equal(3, ops.Count);
            // ins
            MspOperation op = ops[0];
            Assert.Equal(MspOperator.Insert, op.Operator);
            Assert.Equal("1×0", op.RangeA.ToString());
            Assert.Equal("A", op.ValueB);
            // rep
            op = ops[1];
            Assert.Equal(MspOperator.Replace, op.Operator);
            Assert.Equal("2", op.RangeA.ToString());
            Assert.Equal("x", op.ValueA);
            Assert.Equal("C", op.ValueB);
            // del
            op = ops[2];
            Assert.Equal(MspOperator.Delete, op.Operator);
            Assert.Equal("4", op.RangeA.ToString());
            Assert.Equal("f", op.ValueA);
        }

        [Fact]
        public void Adapt_MovFromStart_Ok()
        {
            DifferResultToMspAdapter adapter = new DifferResultToMspAdapter();

            var ops = adapter.Adapt(new diff_match_patch().diff_main("Xab", "abX"));

            Assert.Single(ops);
            // mov
            MspOperation op = ops[0];
            Assert.Equal(MspOperator.Move, op.Operator);
            Assert.Equal("1", op.RangeA.ToString());
            Assert.Equal("4×0", op.RangeB.ToString());
            Assert.Equal("X", op.ValueA);
        }

        [Fact]
        public void Adapt_MovFromInner_Ok()
        {
            DifferResultToMspAdapter adapter = new DifferResultToMspAdapter();

            var ops = adapter.Adapt(new diff_match_patch().diff_main("aXbc", "abcX"));

            Assert.Single(ops);
            // mov
            MspOperation op = ops[0];
            Assert.Equal(MspOperator.Move, op.Operator);
            Assert.Equal("2", op.RangeA.ToString());
            Assert.Equal("5×0", op.RangeB.ToString());
            Assert.Equal("X", op.ValueA);
        }

        [Fact]
        public void Adapt_MovFromEnd_Ok()
        {
            DifferResultToMspAdapter adapter = new DifferResultToMspAdapter();

            var ops = adapter.Adapt(new diff_match_patch().diff_main("abX", "Xab"));

            Assert.Single(ops);
            // mov
            MspOperation op = ops[0];
            Assert.Equal(MspOperator.Move, op.Operator);
            Assert.Equal("3", op.RangeA.ToString());
            Assert.Equal("1×0", op.RangeB.ToString());
            Assert.Equal("X", op.ValueA);
        }

        //[Fact]
        //public void Adapt_SwpAtStart_Ok()
        //{
        //    DifferResultToMspAdapter adapter = new DifferResultToMspAdapter();

        //    var ops = adapter.Adapt(new diff_match_patch().diff_main("XYab", "YXab"));

        //    Assert.Single(ops);
        //    // swp
        //    MspOperation op = ops[0];
        //    Assert.Equal(MspOperator.Swap, op.Operator);
        //    Assert.Equal("1", op.RangeA.ToString());
        //    Assert.Equal("2", op.RangeB.ToString());
        //    Assert.Equal("X", op.ValueA);
        //    Assert.Equal("Y", op.ValueB);
        //}

        //[Fact]
        //public void Adapt_SwpAtEnd_Ok()
        //{
        //    DifferResultToMspAdapter adapter = new DifferResultToMspAdapter();

        //    var ops = adapter.Adapt(new diff_match_patch().diff_main("abXY", "abYX"));

        //    Assert.Single(ops);
        //    // swp
        //    MspOperation op = ops[0];
        //    Assert.Equal(MspOperator.Swap, op.Operator);
        //    Assert.Equal("3", op.RangeA.ToString());
        //    Assert.Equal("4", op.RangeB.ToString());
        //    Assert.Equal("X", op.ValueA);
        //    Assert.Equal("Y", op.ValueB);
        //}

        //[Fact]
        //public void Adapt_SwpAtInner_Ok()
        //{
        //    DifferResultToMspAdapter adapter = new DifferResultToMspAdapter();

        //    var ops = adapter.Adapt(new diff_match_patch().diff_main("abXYcd", "abYXcd"));

        //    Assert.Single(ops);
        //    // swp
        //    MspOperation op = ops[0];
        //    Assert.Equal(MspOperator.Swap, op.Operator);
        //    Assert.Equal("3", op.RangeA.ToString());
        //    Assert.Equal("4", op.RangeB.ToString());
        //    Assert.Equal("X", op.ValueA);
        //    Assert.Equal("Y", op.ValueB);
        //}

        //[Fact]
        //public void Adapt_SwpNotInContact_Ok()
        //{
        //    DifferResultToMspAdapter adapter = new DifferResultToMspAdapter();

        //    var ops = adapter.Adapt(new diff_match_patch().diff_main("XaYb", "YaXb"));

        //    Assert.Single(ops);
        //    // swp
        //    MspOperation op = ops[0];
        //    Assert.Equal(MspOperator.Swap, op.Operator);
        //    Assert.Equal("1", op.RangeA.ToString());
        //    Assert.Equal("3", op.RangeB.ToString());
        //    Assert.Equal("X", op.ValueA);
        //    Assert.Equal("Y", op.ValueB);
        //}
    }
}
