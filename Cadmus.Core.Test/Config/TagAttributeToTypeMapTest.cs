using Cadmus.Core.Config;
using Cadmus.General.Parts;
using System;
using Xunit;

namespace Cadmus.Core.Test.Config
{
    public sealed class TagAttributeToTypeMapTest
    {
        private static TagAttributeToTypeMap GetMap()
        {
            TagAttributeToTypeMap map = new();
            map.Add(new[] { typeof(NotePart).Assembly });
            return map;
        }

        [Fact]
        public void Get_NotExistingPart_Null()
        {
            TagAttributeToTypeMap map = GetMap();

            Type t = map.Get("not-existing");

            Assert.Null(t);
        }

        [Fact]
        public void Get_NotePart_Ok()
        {
            TagAttributeToTypeMap map = GetMap();

            Type t = map.Get("it.vedph.note");

            Assert.Equal(typeof(NotePart), t);
        }

        [Fact]
        public void Get_CommentLayerPart_Ok()
        {
            TagAttributeToTypeMap map = GetMap();

            Type t = map.Get(
                "it.vedph.token-text-layer:fr.it.vedph.comment");

            Assert.Equal(typeof(TokenTextLayerPart<CommentLayerFragment>), t);
        }
    }
}
