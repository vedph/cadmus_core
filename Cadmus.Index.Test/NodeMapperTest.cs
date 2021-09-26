using Cadmus.Index.Graph;
using Xunit;

namespace Cadmus.Index.Test
{
    public sealed class NodeMapperTest
    {
        [Fact]
        public void ParseItemTitle_Simple_Ok()
        {
            var tpu = NodeMapper.ParseItemTitle("A simple title");
            Assert.Equal("A simple title", tpu.Item1);
            Assert.Null(tpu.Item2);
            Assert.Null(tpu.Item3);
        }

        [Fact]
        public void ParseItemTitle_WithUid_Ok()
        {
            var tpu = NodeMapper.ParseItemTitle("A simple title [#x:the_uid]");
            Assert.Equal("A simple title", tpu.Item1);
            Assert.Null(tpu.Item2);
            Assert.Equal("x:the_uid", tpu.Item3);
        }

        [Fact]
        public void ParseItemTitle_WithPrefix_Ok()
        {
            var tpu = NodeMapper.ParseItemTitle("A simple title [@x:artists/]");
            Assert.Equal("A simple title", tpu.Item1);
            Assert.Equal("x:artists/", tpu.Item2);
            Assert.Null(tpu.Item3);
        }

        [Fact]
        public void ParsePinName_Simple_Ok()
        {
            string[] comps = NodeMapper.ParsePinName("name");

            Assert.Single(comps);
            Assert.Equal("name", comps[0]);
        }

        [Fact]
        public void ParsePinName_Composite1_Ok()
        {
            string[] comps = NodeMapper.ParsePinName("eid@alpha");

            Assert.Equal(2, comps.Length);
            Assert.Equal("eid", comps[0]);
            Assert.Equal("alpha", comps[1]);
        }

        [Fact]
        public void ParsePinName_Composite2_Ok()
        {
            string[] comps = NodeMapper.ParsePinName("eid@alpha@beta");

            Assert.Equal(3, comps.Length);
            Assert.Equal("eid", comps[0]);
            Assert.Equal("alpha", comps[1]);
            Assert.Equal("beta", comps[2]);
        }
    }
}
