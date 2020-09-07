using Xunit;

namespace Cadmus.Core.Test
{
    public sealed class PinTextFilterTest
    {
        [Fact]
        public void Apply_Null_Null()
        {
            string filtered = PinTextFilter.Apply(null);
            Assert.Null(filtered);
        }

        [Fact]
        public void Apply_Empty_Empty()
        {
            string filtered = PinTextFilter.Apply("");
            Assert.Equal("", filtered);
        }

        [Theory]
        [InlineData(" ", "")]
        [InlineData("  ", "")]
        [InlineData(" \t ", "")]
        [InlineData(" abc", "abc")]
        [InlineData("abc ", "abc")]
        [InlineData("ab c \td", "ab c d")]
        public void Apply_Whitespaces_Ok(string input, string expected)
        {
            string filtered = PinTextFilter.Apply(input);
            Assert.Equal(expected, filtered);
        }

        [Theory]
        [InlineData(" ", "")]
        [InlineData("  ", "")]
        [InlineData(" \t ", "")]
        [InlineData(" àbc", "abc")]
        [InlineData("Ábç ", "abc")]
        public void Apply_Diacritics_Ok(string input, string expected)
        {
            string filtered = PinTextFilter.Apply(input);
            Assert.Equal(expected, filtered);
        }

        [Theory]
        [InlineData(true, "3ab12cd4", "3ab12cd4")]
        [InlineData(false, "3ab12cd4", "abcd")]
        public void Apply_Digits_Ok(bool digits, string input, string expected)
        {
            string filtered = PinTextFilter.Apply(input, digits);
            Assert.Equal(expected, filtered);
        }
    }
}
