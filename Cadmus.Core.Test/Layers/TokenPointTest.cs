using Cadmus.Core.Layers;
using Xunit;

namespace Cadmus.Core.Test.Layers
{
    public sealed class TokenPointTest
    {
        #region Parse
        [Fact]
        public void Parse_YX_Ok()
        {
            TokenTextPoint pt = TokenTextPoint.Parse("12.34");

            Assert.Equal(12, pt.Y);
            Assert.Equal(34, pt.X);
            Assert.Equal(0, pt.At);
            Assert.Equal(0, pt.Run);
        }

        [Fact]
        public void Parse_YXAt_Ok()
        {
            TokenTextPoint pt = TokenTextPoint.Parse("12.34@3");

            Assert.Equal(12, pt.Y);
            Assert.Equal(34, pt.X);
            Assert.Equal(3, pt.At);
            Assert.Equal(1, pt.Run);
        }

        [Fact]
        public void Parse_YXAtRun_Ok()
        {
            TokenTextPoint pt = TokenTextPoint.Parse("12.34@3x2");

            Assert.Equal(12, pt.Y);
            Assert.Equal(34, pt.X);
            Assert.Equal(3, pt.At);
            Assert.Equal(2, pt.Run);
        }
        #endregion

        #region Compare
        // TODO Equals

        [Fact]
        public void CompareTo_YXEqual_0()
        {
            TokenTextPoint a = TokenTextPoint.Parse("1.2");
            TokenTextPoint b = TokenTextPoint.Parse("1.2");

            Assert.Equal(0, a.CompareTo(b));
        }

        [Fact]
        public void CompareTo_YXAtRunEqual_0()
        {
            TokenTextPoint a = TokenTextPoint.Parse("1.2@3x4");
            TokenTextPoint b = TokenTextPoint.Parse("1.2@3x4");

            Assert.Equal(0, a.CompareTo(b));
        }

        [Fact]
        public void CompareTo_YGreater_Gt0()
        {
            TokenTextPoint a = TokenTextPoint.Parse("2.3");
            TokenTextPoint b = TokenTextPoint.Parse("1.4");

            Assert.True(a.CompareTo(b) > 0);
        }

        [Fact]
        public void CompareTo_XGreater_Gt0()
        {
            TokenTextPoint a = TokenTextPoint.Parse("1.3");
            TokenTextPoint b = TokenTextPoint.Parse("1.2");

            Assert.True(a.CompareTo(b) > 0);
        }

        [Fact]
        public void CompareTo_AtGreater_Gt0()
        {
            TokenTextPoint a = TokenTextPoint.Parse("1.3@4");
            TokenTextPoint b = TokenTextPoint.Parse("1.3@1");

            Assert.True(a.CompareTo(b) > 0);
        }

        [Fact]
        public void CompareTo_RunGreater_Gt0()
        {
            TokenTextPoint a = TokenTextPoint.Parse("1.3@4x2");
            TokenTextPoint b = TokenTextPoint.Parse("1.3@4x1");

            Assert.True(a.CompareTo(b) > 0);
        }

        [Fact]
        public void CompareTo_YLess_Lt0()
        {
            TokenTextPoint a = TokenTextPoint.Parse("1.4");
            TokenTextPoint b = TokenTextPoint.Parse("2.3");

            Assert.True(a.CompareTo(b) < 0);
        }

        [Fact]
        public void CompareTo_XLess_Lt0()
        {
            TokenTextPoint a = TokenTextPoint.Parse("1.2");
            TokenTextPoint b = TokenTextPoint.Parse("1.3");

            Assert.True(a.CompareTo(b) < 0);
        }

        [Fact]
        public void CompareTo_AtLess_Lt0()
        {
            TokenTextPoint a = TokenTextPoint.Parse("1.3@1");
            TokenTextPoint b = TokenTextPoint.Parse("1.3@4");

            Assert.True(a.CompareTo(b) < 0);
        }

        [Fact]
        public void CompareTo_RunLess_Lt0()
        {
            TokenTextPoint a = TokenTextPoint.Parse("1.3@4x1");
            TokenTextPoint b = TokenTextPoint.Parse("1.3@4x2");

            Assert.True(a.CompareTo(b) < 0);
        }

        // TODO IntegralCompareTo
        #endregion
    }
}
