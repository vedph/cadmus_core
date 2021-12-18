using Cadmus.Bricks;
using Cadmus.Core;
using Cadmus.Parts.General;
using Cadmus.Parts.Layers;
using Cadmus.Refs.Bricks;
using Cadmus.Seed.Parts.General;
using Cadmus.Seed.Parts.Layers;
using Fusi.Tools.Config;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Cadmus.Parts.Test.Layers
{
    public sealed class CommentLayerFragmentTest
    {
        private static CommentLayerFragment GetFragment()
        {
            CommentLayerFragmentSeeder seeder = new CommentLayerFragmentSeeder();
            seeder.Configure(new CommentPartSeederOptions());
            IItem item = new Item
            {
                FacetId = "default",
                CreatorId = "zeus",
                UserId = "zeus",
                Description = "Test item",
                Title = "Test Item",
                SortKey = ""
            };
            return (CommentLayerFragment)
                seeder.GetFragment(item, "1.2", "hello world");
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
            Assert.Equal(fr.References.Count, fr2.References.Count);
            Assert.Equal(fr.ExternalIds.Count, fr2.ExternalIds.Count);
            Assert.Equal(fr.Categories.Count, fr2.Categories.Count);
            Assert.Equal(fr.Keywords.Count, fr2.Keywords.Count);
        }

        [Fact]
        public void GetDataPins_Ok()
        {
            CommentLayerFragment fr = new CommentLayerFragment
            {
                Tag = "tag"
            };
            for (int n = 1; n <= 3; n++)
            {
                bool even = n % 2 == 0;

                fr.References.Add(new DocReference
                {
                    Citation = $"w{n}"
                });
                fr.ExternalIds.Add(new ExternalId { Value = $"i{n}" });
                fr.Categories.Add($"c{n}");
                fr.Keywords.Add(new IndexKeyword
                {
                    IndexId = even ? "even" : "odd",
                    Language = "eng",
                    Value = $"k{(char)('a' - 1 + n)}"
                });
            }

            List<DataPin> pins = fr.GetDataPins().ToList();
            Assert.Equal(13, pins.Count);

            // fr.tag
            DataPin pin = pins.Find(p => p.Name == "fr.tag");
            Assert.Equal("tag", pin.Value);

            // fr.key.odd.eng=ka
            pin = pins.Find(p => p.Name == "fr.key.odd.eng" && p.Value == "ka");

            // fr.key.even.eng=kb
            pin = pins.Find(p => p.Name == "fr.key.even.eng" && p.Value == "kb");

            // fr.key.odd.eng=kc
            pin = pins.Find(p => p.Name == "fr.key.odd.eng" && p.Value == "kc");

            for (int n = 1; n <= 3; n++)
            {
                // ref
                pin = pins.Find(p => p.Name == "fr.ref" && p.Value == $"w{n}");
                Assert.NotNull(pin);

                // id
                pin = pins.Find(p => p.Name == "fr.id" && p.Value == $"i{n}");
                Assert.NotNull(pin);

                // cat
                pin = pins.Find(p => p.Name == "fr.cat" && p.Value == $"c{n}");
                Assert.NotNull(pin);
            }
        }
    }
}
