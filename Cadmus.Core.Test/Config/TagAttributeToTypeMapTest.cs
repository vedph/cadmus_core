using Cadmus.Core.Config;
using Cadmus.Parts.General;
using Xunit;

namespace Cadmus.Core.Test.Config
{
    public sealed class TagAttributeToTypeMapTest
    {
        [Fact]
        public void Get_Note_Ok()
        {
            TagAttributeToTypeMap map = new TagAttributeToTypeMap();
            map.Add(new[] { typeof(NotePart).Assembly });

            Assert.NotNull(map.Get("note"));
        }

        [Fact]
        public void Get_CommentFragment_Ok()
        {
            TagAttributeToTypeMap map = new TagAttributeToTypeMap();
            map.Add(new[] { typeof(NotePart).Assembly });

            Assert.NotNull(map.Get("token-text-layer:fr.comment"));
        }
    }
}
