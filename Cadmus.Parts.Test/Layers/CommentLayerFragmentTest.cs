using Cadmus.Core;
using Cadmus.Parts.Layers;
using Fusi.Tools.Config;
using System.Reflection;
using Xunit;

namespace Cadmus.Parts.Test.Layers
{
    public sealed class CommentLayerFragmentTest
    {
        private static CommentLayerFragment GetFragment()
        {
            return new CommentLayerFragment
            {
                Location = "1.23",
                Tag = "some-tag",
                Text = "This is a comment\n.End."
            };
        }

        [Fact]
        public void Fragment_Has_Tag()
        {
            TagAttribute attr = typeof(CommentLayerFragment).GetTypeInfo()
                .GetCustomAttribute<TagAttribute>();
            string typeId = attr != null ? attr.Tag : GetType().FullName;
            Assert.NotNull(typeId);
            Assert.StartsWith(PartBase.FR_PREFIX, typeId);
        }

        [Fact]
        public void Fragment_Is_Serializable()
        {
            CommentLayerFragment fr = GetFragment();

            string json = TestHelper.SerializeFragment(fr);
            CommentLayerFragment fr2 =
                TestHelper.DeserializeFragment<CommentLayerFragment>(json);

            Assert.Equal(fr.Location, fr2.Location);
            Assert.Equal(fr.Tag, fr2.Tag);
            Assert.Equal(fr.Text, fr2.Text);
        }
    }
}
