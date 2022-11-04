using Cadmus.Core.Config;
using Cadmus.General.Parts;
using System;
using System.Reflection;
using Xunit;

namespace Cadmus.Core.Test.Config
{
    public sealed class StandardPartTypeProviderTest
    {
        private static IPartTypeProvider GetProvider()
        {
            TagAttributeToTypeMap map = new();
            map.Add(new Assembly[] { typeof(NotePart).Assembly });
            return new StandardPartTypeProvider(map);
        }

        [Fact]
        public void Get_NotExistingPart_Null()
        {
            Type? t = GetProvider().Get("not-existing");

            Assert.Null(t);
        }

        [Fact]
        public void Get_NotePart_Ok()
        {
            Type? t = GetProvider().Get("it.vedph.note");

            Assert.Equal(typeof(NotePart), t);
        }

        [Fact]
        public void Get_CommentLayerPart_Ok()
        {
            Type? t = GetProvider().Get(
                "it.vedph.token-text-layer:fr.it.vedph.comment");

            Assert.Equal(typeof(TokenTextLayerPart<CommentLayerFragment>), t);
        }
    }
}
