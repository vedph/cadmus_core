using Cadmus.Philology.Parts.Layers;
using Fusi.Tools.Text;
using System.Text;
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

        private string AppendTagAndNote(string op, string tag, string note)
        {
            StringBuilder sb = new StringBuilder(op);
            if (tag != null) sb.Append(" [").Append(tag).Append(']');
            if (note != null) sb.Append(" {").Append(note).Append('}');
            return sb.ToString();
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("tag", null)]
        [InlineData(null, "note")]
        [InlineData("tag", "note")]
        public void Parse_Delete_Ok(string tag, string note)
        {
            string text = AppendTagAndNote("@2x1=", tag, note);
            MspOperation op = MspOperation.Parse(text);

            // op: delete
            Assert.Equal(MspOperator.Delete, op.Operator);
            // ranges: A
            Assert.Equal("2", op.RangeA.ToString());
            Assert.Equal(TextRange.Empty, op.RangeB);
            // values: none
            Assert.Null(op.ValueA);
            Assert.Null(op.ValueB);
            // tag and notes
            Assert.Equal(tag, op.Tag);
            Assert.Equal(note, op.Note);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("tag", null)]
        [InlineData(null, "note")]
        [InlineData("tag", "note")]
        public void Parse_DeleteWithEmptyValueB_Ok(string tag, string note)
        {
            string text = AppendTagAndNote("@2x1=\"\"", tag, note);
            MspOperation op = MspOperation.Parse(text);

            // op: delete
            Assert.Equal(MspOperator.Delete, op.Operator);
            // ranges: A
            Assert.Equal("2", op.RangeA.ToString());
            Assert.Equal(TextRange.Empty, op.RangeB);
            // values: none
            Assert.Null(op.ValueA);
            Assert.Null(op.ValueB);
            // tag and notes
            Assert.Equal(tag, op.Tag);
            Assert.Equal(note, op.Note);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("tag", null)]
        [InlineData(null, "note")]
        [InlineData("tag", "note")]
        public void Parse_DeleteWithValueA_Ok(string tag, string note)
        {
            string text = AppendTagAndNote("\"a\"@2x1=", tag, note);
            MspOperation op = MspOperation.Parse(text);

            // op: delete
            Assert.Equal(MspOperator.Delete, op.Operator);
            // ranges: A
            Assert.Equal("2", op.RangeA.ToString());
            Assert.Equal(TextRange.Empty, op.RangeB);
            // values: A
            Assert.Equal("a", op.ValueA);
            Assert.Null(op.ValueB);
            // tag and notes
            Assert.Equal(tag, op.Tag);
            Assert.Equal(note, op.Note);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("tag", null)]
        [InlineData(null, "note")]
        [InlineData("tag", "note")]

        public void Parse_Insert_Ok(string tag, string note)
        {
            string text = AppendTagAndNote("@2x0=\"s\"", tag, note);
            MspOperation op = MspOperation.Parse(text);

            // op: delete
            Assert.Equal(MspOperator.Insert, op.Operator);
            // ranges: A
            Assert.Equal("2×0", op.RangeA.ToString());
            Assert.Equal(TextRange.Empty, op.RangeB);
            // values: B
            Assert.Equal("s", op.ValueB);
            Assert.Null(op.ValueA);
            // tag and notes
            Assert.Equal(tag, op.Tag);
            Assert.Equal(note, op.Note);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("tag", null)]
        [InlineData(null, "note")]
        [InlineData("tag", "note")]
        public void Parse_Replace_Ok(string tag, string note)
        {
            string text = AppendTagAndNote("@2x1=\"b\"", tag, note);
            MspOperation op = MspOperation.Parse(text);

            // op: replace
            Assert.Equal(MspOperator.Replace, op.Operator);
            // ranges: A
            Assert.Equal("2", op.RangeA.ToString());
            Assert.Equal(TextRange.Empty, op.RangeB);
            // values: B
            Assert.Equal("b", op.ValueB);
            Assert.Null(op.ValueA);
            // tag and notes
            Assert.Equal(tag, op.Tag);
            Assert.Equal(note, op.Note);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("tag", null)]
        [InlineData(null, "note")]
        [InlineData("tag", "note")]
        public void Parse_ReplaceWithValueA_Ok(string tag, string note)
        {
            string text = AppendTagAndNote("\"a\"@2x1=\"b\"", tag, note);
            MspOperation op = MspOperation.Parse(text);

            // op: replace
            Assert.Equal(MspOperator.Replace, op.Operator);
            // ranges: A
            Assert.Equal("2", op.RangeA.ToString());
            Assert.Equal(TextRange.Empty, op.RangeB);
            // values: A B
            Assert.Equal("a", op.ValueA);
            Assert.Equal("b", op.ValueB);
            // tag and notes
            Assert.Equal(tag, op.Tag);
            Assert.Equal(note, op.Note);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("tag", null)]
        [InlineData(null, "note")]
        [InlineData("tag", "note")]
        public void Parse_Move_Ok(string tag, string note)
        {
            string text = AppendTagAndNote("@2x1>@4", tag, note);
            MspOperation op = MspOperation.Parse(text);

            // op: move
            Assert.Equal(MspOperator.Move, op.Operator);
            // ranges: A
            Assert.Equal("2", op.RangeA.ToString());
            Assert.Equal("4×0", op.RangeB.ToString());
            // values: none
            Assert.Null(op.ValueA);
            Assert.Null(op.ValueB);
            // tag and notes
            Assert.Equal(tag, op.Tag);
            Assert.Equal(note, op.Note);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("tag", null)]
        [InlineData(null, "note")]
        [InlineData("tag", "note")]
        public void Parse_MoveWithValueA_Ok(string tag, string note)
        {
            string text = AppendTagAndNote("\"a\"@2x1>@4", tag, note);
            MspOperation op = MspOperation.Parse(text);

            // op: move
            Assert.Equal(MspOperator.Move, op.Operator);
            // ranges: A
            Assert.Equal("2", op.RangeA.ToString());
            Assert.Equal("4×0", op.RangeB.ToString());
            // values: A
            Assert.Equal("a", op.ValueA);
            Assert.Null(op.ValueB);
            // tag and notes
            Assert.Equal(tag, op.Tag);
            Assert.Equal(note, op.Note);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("tag", null)]
        [InlineData(null, "note")]
        [InlineData("tag", "note")]
        public void Parse_Swap_Ok(string tag, string note)
        {
            string text = AppendTagAndNote("@2x1~@4x1", tag, note);
            MspOperation op = MspOperation.Parse(text);

            // op: move
            Assert.Equal(MspOperator.Swap, op.Operator);
            // ranges: A
            Assert.Equal("2", op.RangeA.ToString());
            Assert.Equal("4", op.RangeB.ToString());
            // values: none
            Assert.Null(op.ValueA);
            Assert.Null(op.ValueB);
            // tag and notes
            Assert.Equal(tag, op.Tag);
            Assert.Equal(note, op.Note);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("tag", null)]
        [InlineData(null, "note")]
        [InlineData("tag", "note")]
        public void Parse_SwapWithValuesAB_Ok(string tag, string note)
        {
            string text = AppendTagAndNote("\"a\"@2x1~\"b\"@4x1", tag, note);
            MspOperation op = MspOperation.Parse(text);

            // op: move
            Assert.Equal(MspOperator.Swap, op.Operator);
            // ranges: A
            Assert.Equal("2", op.RangeA.ToString());
            Assert.Equal("4", op.RangeB.ToString());
            // values: A B
            Assert.Equal("a", op.ValueA);
            Assert.Equal("b", op.ValueB);
            // tag and notes
            Assert.Equal(tag, op.Tag);
            Assert.Equal(note, op.Note);
        }
        #endregion
    }
}
