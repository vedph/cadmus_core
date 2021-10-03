using Xunit;

namespace Cadmus.Core.Test
{
    public sealed class DataPinFilterClauseTest
    {
        [Theory]
        [InlineData("name", null, null)]
        [InlineData("name", "it.vedph.x", null)]
        [InlineData("name", "it.vedph.x", "role")]
        [InlineData("name", null, "role")]
        public void IsMatch_NoFilter_True(string pinName, string partTypeId,
            string partRoleId)
        {
            DataPinFilterClause clause = new DataPinFilterClause();
            Assert.True(clause.IsMatch(pinName, partTypeId, partRoleId));
        }

        [Theory]
        [InlineData("")]
        [InlineData("pr")]
        [InlineData("pro")]
        [InlineData("xyz")]
        public void IsMatch_NonMatchingPrefix_False(string pinName)
        {
            DataPinFilterClause clause = new DataPinFilterClause
            {
                Prefix = "pre"
            };
            Assert.False(clause.IsMatch(pinName));
        }

        [Fact]
        public void IsMatch_MatchingPrefix_True()
        {
            DataPinFilterClause clause = new DataPinFilterClause
            {
                Prefix = "pre"
            };
            Assert.True(clause.IsMatch("pre"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("prox")]
        [InlineData("past")]
        [InlineData("xyz")]
        public void IsMatch_NonMatchingSuffix_False(string pinName)
        {
            DataPinFilterClause clause = new DataPinFilterClause
            {
                Suffix = "post"
            };
            Assert.False(clause.IsMatch(pinName));
        }

        [Fact]
        public void IsMatch_MatchingSuffix_True()
        {
            DataPinFilterClause clause = new DataPinFilterClause
            {
                Prefix = "post"
            };
            Assert.True(clause.IsMatch("post"));
        }

        [Theory]
        [InlineData("ax")]
        [InlineData("ad")]
        public void IsMatch_NonMatchingPattern_False(string pinName)
        {
            DataPinFilterClause clause = new DataPinFilterClause
            {
                Pattern = "ab*c"
            };
            Assert.False(clause.IsMatch(pinName));
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
            DataPinFilterClause clause = new DataPinFilterClause
            {
                Pattern = "ab*c"
            };
            Assert.True(clause.IsMatch(pinName));
        }

        [Fact]
        public void IsMatch_MatchingAffixesAndPattern_True()
        {
            DataPinFilterClause clause = new DataPinFilterClause
            {
                Prefix = "he",
                Suffix = "!",
                Pattern = "l+"
            };
            Assert.True(clause.IsMatch("helloworld!"));
        }

        [Fact]
        public void IsMatch_NonMatchingPrefixWithSuffixPattern_False()
        {
            DataPinFilterClause clause = new DataPinFilterClause
            {
                Prefix = "he",
                Suffix = "!",
                Pattern = "l+"
            };
            Assert.False(clause.IsMatch("elloworld!"));
        }

        [Fact]
        public void IsMatch_NonMatchingSuffixWithPreffixPattern_False()
        {
            DataPinFilterClause clause = new DataPinFilterClause
            {
                Prefix = "he",
                Suffix = "!",
                Pattern = "l+"
            };
            Assert.False(clause.IsMatch("helloworld"));
        }

        [Fact]
        public void IsMatch_NonMatchingPatternWithAffixes_False()
        {
            DataPinFilterClause clause = new DataPinFilterClause
            {
                Prefix = "he",
                Suffix = "!",
                Pattern = "b+"
            };
            Assert.False(clause.IsMatch("helloworld!"));
        }
    }
}
