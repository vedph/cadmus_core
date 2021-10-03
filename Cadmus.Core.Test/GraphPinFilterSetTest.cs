using Xunit;

namespace Cadmus.Core.Test
{
    public sealed class DataPinFilterClauseSetTest
    {
        [Fact]
        public void IsMatch_AnyMatchingInWhite_True()
        {
            DataPinFilterClauseSet set = new DataPinFilterClauseSet();
            set.Clauses.Add(new DataPinFilterClause
            {
                Prefix = "pre"
            });
            set.Clauses.Add(new DataPinFilterClause
            {
                Suffix = "post"
            });
            Assert.True(set.IsMatch("pre-match"));
            Assert.True(set.IsMatch("match-post"));
        }

        [Fact]
        public void IsMatch_NoneMatchingInWhite_False()
        {
            DataPinFilterClauseSet set = new DataPinFilterClauseSet();
            set.Clauses.Add(new DataPinFilterClause
            {
                Prefix = "pre"
            });
            set.Clauses.Add(new DataPinFilterClause
            {
                Suffix = "post"
            });
            Assert.False(set.IsMatch("alpha"));
            Assert.False(set.IsMatch("beta"));
        }

        [Fact]
        public void IsMatch_AnyMatchingInBlack_False()
        {
            DataPinFilterClauseSet set = new DataPinFilterClauseSet
            {
                IsBlack = true
            };
            set.Clauses.Add(new DataPinFilterClause
            {
                Prefix = "pre"
            });
            set.Clauses.Add(new DataPinFilterClause
            {
                Suffix = "post"
            });
            Assert.False(set.IsMatch("pre-match"));
            Assert.False(set.IsMatch("match-post"));
        }

        [Fact]
        public void IsMatch_NoneMatchingInBlack_True()
        {
            DataPinFilterClauseSet set = new DataPinFilterClauseSet
            {
                IsBlack = true
            };
            set.Clauses.Add(new DataPinFilterClause
            {
                Prefix = "pre"
            });
            set.Clauses.Add(new DataPinFilterClause
            {
                Suffix = "post"
            });
            Assert.True(set.IsMatch("alpha"));
            Assert.True(set.IsMatch("beta"));
        }
    }
}
