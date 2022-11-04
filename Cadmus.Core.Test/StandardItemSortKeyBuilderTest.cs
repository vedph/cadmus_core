using Xunit;

namespace Cadmus.Core.Test
{
    public sealed class StandardItemSortKeyBuilderTest
    {
        [Theory]
        [InlineData((string?)null, "")]
        [InlineData("", "")]
        [InlineData(" abc", "abc")]
        [InlineData("abc ", "abc")]
        [InlineData(" abc ", "abc")]
        [InlineData(" abc\ndef\tghi  jkl  ", "abc def ghi jkl")]
        [InlineData("abc, 123; def.", "abc 123 def")]
        [InlineData("Abc", "abc")]
        [InlineData("aéÀ", "aea")]
        [InlineData("D'Annunzio", "d'annunzio")]
        [InlineData("μῆνιν ἄειδε, θεά", "μηνιν αειδε θεα")]
        public void BuildKey_Filtering_Ok(string title, string expected)
        {
            IItemSortKeyBuilder builder = new StandardItemSortKeyBuilder();

            string key = builder.BuildKey(new Item
            {
                Title = title
            }, null);

            Assert.Equal(expected, key);
        }

        // TODO: other tests
    }
}
