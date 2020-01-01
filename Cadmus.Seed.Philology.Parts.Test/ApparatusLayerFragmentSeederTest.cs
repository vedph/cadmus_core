using Cadmus.Core;
using Cadmus.Core.Layers;
using Cadmus.Philology.Parts.Layers;
using Cadmus.Seed.Parts.Test;
using Cadmus.Seed.Philology.Parts.Layers;
using System;
using Xunit;

namespace Cadmus.Seed.Philology.Parts.Test
{
    public sealed class ApparatusLayerFragmentSeederTest
    {
        private static readonly PartSeederFactory _factory;
        private static readonly SeedOptions _seedOptions;
        private static readonly IItem _item;

        static ApparatusLayerFragmentSeederTest()
        {
            _factory = TestHelper.GetFactory();
            _seedOptions = _factory.GetSeedOptions();
            _item = _factory.GetItemSeeder().GetItem(1, "facet");
        }

        [Fact]
        public void Seed_NoOptions_Null()
        {
            ApparatusLayerFragmentSeeder seeder = new ApparatusLayerFragmentSeeder();
            seeder.SetSeedOptions(_seedOptions);

            Assert.Null(seeder.GetFragment(_item, "1.1", "alpha"));
        }

        [Fact]
        public void Seed_NoAuthors_Null()
        {
            ApparatusLayerFragmentSeeder seeder = new ApparatusLayerFragmentSeeder();
            seeder.SetSeedOptions(_seedOptions);
            seeder.Configure(new ApparatusLayerFragmentSeederOptions
            {
                Authors = Array.Empty<string>()  // invalid
            });

            Assert.Null(seeder.GetFragment(_item, "1.1", "alpha"));
        }

        [Fact]
        public void Seed_ValidOptions_Ok()
        {
            ApparatusLayerFragmentSeeder seeder = new ApparatusLayerFragmentSeeder();
            seeder.SetSeedOptions(_seedOptions);
            seeder.Configure(new ApparatusLayerFragmentSeederOptions
            {
                Authors = new[]
                {
                    "alpha",
                    "beta",
                    "gamma"
                }
            });

            ITextLayerFragment fragment = seeder.GetFragment(_item, "1.1", "alpha");

            Assert.NotNull(fragment);

            ApparatusLayerFragment fr = fragment as ApparatusLayerFragment;
            Assert.NotNull(fr);

            Assert.Equal("1.1", fr.Location);
            Assert.NotEmpty(fr.Authors);
            if (fr.Type != LemmaVariantType.Note)
                Assert.NotNull(fr.Value);
        }
    }
}
