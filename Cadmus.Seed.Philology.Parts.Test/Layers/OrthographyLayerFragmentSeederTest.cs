using Cadmus.Core;
using Cadmus.Core.Layers;
using Cadmus.Philology.Parts.Layers;
using Cadmus.Seed.Parts.Test;
using Cadmus.Seed.Philology.Parts.Layers;
using Xunit;

namespace Cadmus.Seed.Philology.Parts.Test.Layers
{
    public sealed class OrthographyLayerFragmentSeederTest
    {
        private static readonly PartSeederFactory _factory;
        private static readonly SeedOptions _seedOptions;
        private static readonly IItem _item;

        static OrthographyLayerFragmentSeederTest()
        {
            _factory = TestHelper.GetFactory();
            _seedOptions = _factory.GetSeedOptions();
            _item = _factory.GetItemSeeder().GetItem(1, "facet");
        }

        [Fact]
        public void Seed_WithoutTags_Ok()
        {
            OrthographyLayerFragmentSeeder seeder = new OrthographyLayerFragmentSeeder();

            ITextLayerFragment fragment = seeder.GetFragment(_item, "1.1", "alpha");

            Assert.NotNull(fragment);

            OrthographyLayerFragment fr = fragment as OrthographyLayerFragment;
            Assert.NotNull(fr);

            Assert.Equal("1.1", fr.Location);
            Assert.NotNull(fr.Standard);
            Assert.Single(fr.Operations);
            MspOperation op = MspOperation.Parse(fr.Operations[0]);
            Assert.Null(op.Tag);
        }

        [Fact]
        public void Seed_WithTags_Ok()
        {
            OrthographyLayerFragmentSeeder seeder = new OrthographyLayerFragmentSeeder();
            seeder.SetSeedOptions(_seedOptions);
            seeder.Configure(new OrthographyLayerFragmentSeederOptions
            {
                Tags = new[]
                {
                    "alpha",
                    "beta",
                    "gamma"
                }
            });

            ITextLayerFragment fragment = seeder.GetFragment(_item, "1.1", "alpha");

            Assert.NotNull(fragment);

            OrthographyLayerFragment fr = fragment as OrthographyLayerFragment;
            Assert.NotNull(fr);

            Assert.Equal("1.1", fr.Location);
            Assert.NotNull(fr.Standard);
            Assert.Single(fr.Operations);
            MspOperation op = MspOperation.Parse(fr.Operations[0]);
            Assert.NotNull(op.Tag);
        }
    }
}
