using Cadmus.Index.Graph;
using Xunit;

namespace Cadmus.Index.Test
{
    public sealed class GraphPinFilterTest
    {
        [Theory]
        [InlineData("name", null, null)]
        [InlineData("name", "it.vedph.x", null)]
        [InlineData("name", "it.vedph.x", "role")]
        [InlineData("name", null, "role")]
        public void IsMatch_NoFilter_True(string pinName, string partTypeId,
            string partRoleId)
        {
            GraphPinFilter filter = new GraphPinFilter();
            Assert.True(filter.IsMatch(pinName, partTypeId, partRoleId));
        }

        [Theory]
        [InlineData("")]
        [InlineData("pr")]
        [InlineData("pro")]
        [InlineData("xyz")]
        public void IsMatch_NonMatchingPrefix_False(string pinName)
        {
            GraphPinFilter filter = new GraphPinFilter
            {
                Prefix = "pre"
            };
            Assert.False(filter.IsMatch(pinName));
        }

        [Fact]
        public void IsMatch_MatchingPrefix_True()
        {
            GraphPinFilter filter = new GraphPinFilter
            {
                Prefix = "pre"
            };
            Assert.True(filter.IsMatch("pre"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("prox")]
        [InlineData("past")]
        [InlineData("xyz")]
        public void IsMatch_NonMatchingSuffix_False(string pinName)
        {
            GraphPinFilter filter = new GraphPinFilter
            {
                Suffix = "post"
            };
            Assert.False(filter.IsMatch(pinName));
        }

        [Fact]
        public void IsMatch_MatchingSuffix_True()
        {
            GraphPinFilter filter = new GraphPinFilter
            {
                Prefix = "post"
            };
            Assert.True(filter.IsMatch("post"));
        }

        [Theory]
        [InlineData("ax")]
        [InlineData("ad")]
        public void IsMatch_NonMatchingPattern_False(string pinName)
        {
            GraphPinFilter filter = new GraphPinFilter
            {
                Pattern = "ab*c"
            };
            Assert.False(filter.IsMatch(pinName));
        }

        [Theory]
        [InlineData("ac")]
        [InlineData("abc")]
        [InlineData("abbc")]
        [InlineData("xac")]
        [InlineData("xabc")]
        [InlineData("xabbc")]
        [InlineData("acx")]
        [InlineData("abcx")]
        [InlineData("abbcx")]
        public void IsMatch_MatchingPattern_True(string pinName)
        {
            GraphPinFilter filter = new GraphPinFilter
            {
                Pattern = "ab*c"
            };
            Assert.True(filter.IsMatch(pinName));
        }

        [Fact]
        public void IsMatch_MatchingAffixesAndPattern_True()
        {
            GraphPinFilter filter = new GraphPinFilter
            {
                Prefix = "he",
                Suffix = "!",
                Pattern = "l+"
            };
            Assert.True(filter.IsMatch("helloworld!"));
        }

        [Fact]
        public void IsMatch_NonMatchingPrefixWithSuffixPattern_False()
        {
            GraphPinFilter filter = new GraphPinFilter
            {
                Prefix = "he",
                Suffix = "!",
                Pattern = "l+"
            };
            Assert.False(filter.IsMatch("elloworld!"));
        }

        [Fact]
        public void IsMatch_NonMatchingSuffixWithPreffixPattern_False()
        {
            GraphPinFilter filter = new GraphPinFilter
            {
                Prefix = "he",
                Suffix = "!",
                Pattern = "l+"
            };
            Assert.False(filter.IsMatch("helloworld"));
        }

        [Fact]
        public void IsMatch_NonMatchingPatternWithAffixes_False()
        {
            GraphPinFilter filter = new GraphPinFilter
            {
                Prefix = "he",
                Suffix = "!",
                Pattern = "b+"
            };
            Assert.False(filter.IsMatch("helloworld!"));
        }
    }
}
