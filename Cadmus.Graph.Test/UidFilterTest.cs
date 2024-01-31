using Xunit;

namespace Cadmus.Graph.Test;

public sealed class UidFilterTest
{
    [Theory]
    [InlineData("", "_")]
    [InlineData("Hello, 1 World!", "hello_1_world")]
    [InlineData("ciáo MÓNDO", "ciao_mondo")]
    [InlineData("http://www.some-ontology/guys#543-21&x=1", "http://www.some-ontology/guys#543-21&x=1")]
    public void Filter_Ok(string text, string expected)
    {
        string actual = UidFilter.Apply(text);
        Assert.Equal(expected, actual);
    }
}
