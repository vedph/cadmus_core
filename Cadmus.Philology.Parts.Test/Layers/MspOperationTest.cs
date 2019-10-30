using Cadmus.Philology.Parts.Layers;
using Fusi.Tools.Text;
using Xunit;

namespace Cadmus.Philology.Parts.Test.Layers
{
    public sealed class MspOperationTest
    {
        #region ToString
        [Theory]
        [InlineData(null, null, "@2×3=")]
        [InlineData("tag", null, "@2×3= [tag]")]
        [InlineData(null, "note", "@2×3= {note}")]
        [InlineData("tag", "note", "@2×3= [tag] {note}")]
        public void ToString_Delete_Ok(string tag, string note, string expected)
        {
            MspOperation op = new MspOperation
            {
                Operator = MspOperator.Delete,
                RangeA = TextRange.Parse("2x3"),
                Tag = tag,
                Note = note
            };
            Assert.Equal(expected, op.ToString());
        }

        [Theory]
        [InlineData(null, null, "\"old\"@2×3=")]
        [InlineData("tag", null, "\"old\"@2×3= [tag]")]
        [InlineData(null, "note", "\"old\"@2×3= {note}")]
        [InlineData("tag", "note", "\"old\"@2×3= [tag] {note}")]
        public void ToString_DeleteWithValueA_Ok(string tag, string note, string expected)
        {
            MspOperation op = new MspOperation
            {
                Operator = MspOperator.Delete,
                RangeA = TextRange.Parse("2x3"),
                ValueA = "old",
                Tag = tag,
                Note = note
            };
            Assert.Equal(expected, op.ToString());
        }

        [Theory]
        [InlineData(null, null, "@2×3=\"new\"")]
        [InlineData("tag", null, "@2×3=\"new\" [tag]")]
        [InlineData(null, "note", "@2×3=\"new\" {note}")]
        [InlineData("tag", "note", "@2×3=\"new\" [tag] {note}")]
        public void ToString_Replace_Ok(string tag, string note, string expected)
        {
            MspOperation op = new MspOperation
            {
                Operator = MspOperator.Replace,
                RangeA = TextRange.Parse("2x3"),
                ValueB = "new",
                Tag = tag,
                Note = note
            };
            Assert.Equal(expected, op.ToString());
        }

        [Theory]
        [InlineData(null, null, "\"old\"@2×3=\"new\"")]
        [InlineData("tag", null, "\"old\"@2×3=\"new\" [tag]")]
        [InlineData(null, "note", "\"old\"@2×3=\"new\" {note}")]
        [InlineData("tag", "note", "\"old\"@2×3=\"new\" [tag] {note}")]
        public void ToString_ReplaceWithValueA_Ok(string tag, string note, string expected)
        {
            MspOperation op = new MspOperation
            {
                Operator = MspOperator.Replace,
                RangeA = TextRange.Parse("2x3"),
                ValueA = "old",
                ValueB = "new",
                Tag = tag,
                Note = note
            };
            Assert.Equal(expected, op.ToString());
        }

        [Theory]
        [InlineData(null, null, "@2×0=\"new\"")]
        [InlineData("tag", null, "@2×0=\"new\" [tag]")]
        [InlineData(null, "note", "@2×0=\"new\" {note}")]
        [InlineData("tag", "note", "@2×0=\"new\" [tag] {note}")]
        public void ToString_Insert_Ok(string tag, string note, string expected)
        {
            MspOperation op = new MspOperation
            {
                Operator = MspOperator.Insert,
                RangeA = TextRange.Parse("2x0"),
                ValueB = "new",
                Tag = tag,
                Note = note
            };
            Assert.Equal(expected, op.ToString());
        }

        [Theory]
        [InlineData(null, null, "\"old\"@2×0=\"new\"")]
        [InlineData("tag", null, "\"old\"@2×0=\"new\" [tag]")]
        [InlineData(null, "note", "\"old\"@2×0=\"new\" {note}")]
        [InlineData("tag", "note", "\"old\"@2×0=\"new\" [tag] {note}")]
        public void ToString_InsertWithValueA_Ok(string tag, string note, string expected)
        {
            MspOperation op = new MspOperation
            {
                Operator = MspOperator.Insert,
                RangeA = TextRange.Parse("2x0"),
                ValueA = "old",
                ValueB = "new",
                Tag = tag,
                Note = note
            };
            Assert.Equal(expected, op.ToString());
        }

        [Theory]
        [InlineData(null, null, "@2×3>@6×0")]
        [InlineData("tag", null, "@2×3>@6×0 [tag]")]
        [InlineData(null, "note", "@2×3>@6×0 {note}")]
        [InlineData("tag", "note", "@2×3>@6×0 [tag] {note}")]
        public void ToString_Move_Ok(string tag, string note, string expected)
        {
            MspOperation op = new MspOperation
            {
                Operator = MspOperator.Move,
                RangeA = TextRange.Parse("2x3"),
                RangeB = TextRange.Parse("6x0"),
                Tag = tag,
                Note = note
            };
            Assert.Equal(expected, op.ToString());
        }

        [Theory]
        [InlineData(null, null, "\"old\"@2×3>@6×0")]
        [InlineData("tag", null, "\"old\"@2×3>@6×0 [tag]")]
        [InlineData(null, "note", "\"old\"@2×3>@6×0 {note}")]
        [InlineData("tag", "note", "\"old\"@2×3>@6×0 [tag] {note}")]
        public void ToString_MoveWithValueA_Ok(string tag, string note, string expected)
        {
            MspOperation op = new MspOperation
            {
                Operator = MspOperator.Move,
                RangeA = TextRange.Parse("2x3"),
                RangeB = TextRange.Parse("6x0"),
                ValueA = "old",
                Tag = tag,
                Note = note
            };
            Assert.Equal(expected, op.ToString());
        }

        [Theory]
        [InlineData(null, null, "@2×3~@6×3")]
        [InlineData("tag", null, "@2×3~@6×3 [tag]")]
        [InlineData(null, "note", "@2×3~@6×3 {note}")]
        [InlineData("tag", "note", "@2×3~@6×3 [tag] {note}")]
        public void ToString_Swap_Ok(string tag, string note, string expected)
        {
            MspOperation op = new MspOperation
            {
                Operator = MspOperator.Swap,
                RangeA = TextRange.Parse("2x3"),
                RangeB = TextRange.Parse("6x3"),
                Tag = tag,
                Note = note
            };
            Assert.Equal(expected, op.ToString());
        }

        [Theory]
        [InlineData(null, null, "\"old\"@2×3~\"new\"@6×3")]
        [InlineData("tag", null, "\"old\"@2×3~\"new\"@6×3 [tag]")]
        [InlineData(null, "note", "\"old\"@2×3~\"new\"@6×3 {note}")]
        [InlineData("tag", "note", "\"old\"@2×3~\"new\"@6×3 [tag] {note}")]
        public void ToString_SwapWithValuesAB_Ok(string tag, string note, string expected)
        {
            MspOperation op = new MspOperation
            {
                Operator = MspOperator.Swap,
                RangeA = TextRange.Parse("2x3"),
                RangeB = TextRange.Parse("6x3"),
                ValueA = "old",
                ValueB = "new",
                Tag = tag,
                Note = note
            };
            Assert.Equal(expected, op.ToString());
        }
        #endregion

        #region Parse
        [Fact]
        public void Parse_Null_Null()
        {
            MspOperation op = MspOperation.Parse(null);
            Assert.Null(op);
        }

        [Fact]
        public void Parse_Empty_Null()
        {
            MspOperation op = MspOperation.Parse("");
            Assert.Null(op);
        }

        [Fact]
        public void Parse_Whitespaces_Null()
        {
            MspOperation op = MspOperation.Parse("  \n  ");
            Assert.Null(op);
        }

        [Fact]
        public void Parse_Delete_Ok()
        {
            MspOperation op = MspOperation.Parse("@2x1=");

            // op: delete
            Assert.Equal(MspOperator.Delete, op.Operator);
            // ranges: A, !B
            Assert.Equal("2", op.RangeA.ToString());
            Assert.Equal(TextRange.Empty, op.RangeB);
            // values: none
            Assert.Null(op.ValueA);
            Assert.Null(op.ValueB);
            // tag and notes: none
            Assert.Null(op.Tag);
            Assert.Null(op.Note);
        }

        [Fact]
        public void Parse_DeleteWithEmptyValueB_Ok()
        {
            MspOperation op = MspOperation.Parse("@2x1=\"\"");

            // op: delete
            Assert.Equal(MspOperator.Delete, op.Operator);
            // ranges: A, !B
            Assert.Equal("2", op.RangeA.ToString());
            Assert.Equal(TextRange.Empty, op.RangeB);
            // values: none
            Assert.Null(op.ValueA);
            Assert.Null(op.ValueB);
            // tag and notes: none
            Assert.Null(op.Tag);
            Assert.Null(op.Note);
        }

        [Fact]
        public void Parse_DeleteWithValueA_Ok()
        {
            MspOperation op = MspOperation.Parse("\"a\"@2x1=");

            // op: delete
            Assert.Equal(MspOperator.Delete, op.Operator);
            // ranges: A, !B
            Assert.Equal("2", op.RangeA.ToString());
            Assert.Equal(TextRange.Empty, op.RangeB);
            // values: A
            Assert.Equal("a", op.ValueA);
            Assert.Null(op.ValueB);
            // tag and notes: none
            Assert.Null(op.Tag);
            Assert.Null(op.Note);
        }
        #endregion
    }
}
