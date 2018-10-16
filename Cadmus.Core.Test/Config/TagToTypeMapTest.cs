using Cadmus.Core.Config;
using Cadmus.Parts.General;
using Xunit;

namespace Cadmus.Core.Test.Config
{
    public sealed class TagToTypeMapTest
    {
        [Fact]
        public void Get_Note_Ok()
        {
            TagToTypeMap map = new TagToTypeMap();
            map.Add(new[] { typeof(NotePart).Assembly });

            Assert.NotNull(map.Get("note"));
        }

        [Fact]
        public void Get_CommentFragment_Ok()
        {
            TagToTypeMap map = new TagToTypeMap();
            map.Add(new[] { typeof(NotePart).Assembly });

            Assert.NotNull(map.Get("token-text-layer:fr-comment"));
        }
    }
}
