using Cadmus.Philology.Parts.Layers;
using Fusi.Tools.Text;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cadmus.Philology.Parts.Test.Layers
{
    public sealed class MspOperationTest
    {
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

            // delete
            Assert.Equal(MspOperator.Delete, op.Operator);
            // ranges: A only
            Assert.Equal("2x1", op.RangeA.ToString());
            Assert.Equal(TextRange.Empty, op.RangeB);
            // values: none
            Assert.Null(op.ValueA);
            Assert.Null(op.ValueB);
            // TODO:
        }
    }
}
