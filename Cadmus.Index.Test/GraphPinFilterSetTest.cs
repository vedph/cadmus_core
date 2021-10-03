using Cadmus.Index.Graph;
using Xunit;

namespace Cadmus.Index.Test
{
    public sealed class GraphPinFilterSetTest
    {
        [Fact]
        public void IsMatch_AnyMatchingInWhite_True()
        {
            GraphPinFilterSet set = new GraphPinFilterSet();
            set.Filters.Add(new GraphPinFilter
            {
                Prefix = "pre"
            });
            set.Filters.Add(new GraphPinFilter
            {
                Suffix = "post"
            });
            Assert.True(set.IsMatch("pre-match"));
            Assert.True(set.IsMatch("match-post"));
        }

        [Fact]
        public void IsMatch_NoneMatchingInWhite_False()
        {
            GraphPinFilterSet set = new GraphPinFilterSet();
            set.Filters.Add(new GraphPinFilter
            {
                Prefix = "pre"
            });
            set.Filters.Add(new GraphPinFilter
            {
                Suffix = "post"
            });
            Assert.False(set.IsMatch("alpha"));
            Assert.False(set.IsMatch("beta"));
        }

        [Fact]
        public void IsMatch_AnyMatchingInBlack_False()
        {
            GraphPinFilterSet set = new GraphPinFilterSet
            {
                IsBlack = true
            };
            set.Filters.Add(new GraphPinFilter
            {
                Prefix = "pre"
            });
            set.Filters.Add(new GraphPinFilter
            {
                Suffix = "post"
            });
            Assert.False(set.IsMatch("pre-match"));
            Assert.False(set.IsMatch("match-post"));
        }

        [Fact]
        public void IsMatch_NoneMatchingInBlack_True()
        {
            GraphPinFilterSet set = new GraphPinFilterSet
            {
                IsBlack = true
            };
            set.Filters.Add(new GraphPinFilter
            {
                Prefix = "pre"
            });
            set.Filters.Add(new GraphPinFilter
            {
                Suffix = "post"
            });
            Assert.True(set.IsMatch("alpha"));
            Assert.True(set.IsMatch("beta"));
        }
    }
}
