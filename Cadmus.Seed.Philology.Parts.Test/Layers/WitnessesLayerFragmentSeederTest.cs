using Cadmus.Core;
using Cadmus.Core.Layers;
using Cadmus.Philology.Parts.Layers;
using Cadmus.Seed.Parts.Test;
using Cadmus.Seed.Philology.Parts.Layers;
using System;
using Xunit;

namespace Cadmus.Seed.Philology.Parts.Test.Layers
{
    public sealed class WitnessesLayerFragmentSeederTest
    {
        private static readonly PartSeederFactory _factory;
        private static readonly SeedOptions _seedOptions;
        private static readonly IItem _item;

        static WitnessesLayerFragmentSeederTest()
        {
            _factory = TestHelper.GetFactory();
            _seedOptions = _factory.GetSeedOptions();
            _item = _factory.GetItemSeeder().GetItem(1, "facet");
        }

        [Fact]
        public void Seed_NoOptions_Null()
        {
            WitnessesLayerFragmentSeeder seeder = new WitnessesLayerFragmentSeeder();
            seeder.SetSeedOptions(_seedOptions);

            Assert.Null(seeder.GetFragment(_item, "1.1", "alpha"));
        }

        [Fact]
        public void Seed_NoIds_Null()
        {
            WitnessesLayerFragmentSeeder seeder = new WitnessesLayerFragmentSeeder();
            seeder.SetSeedOptions(_seedOptions);
            seeder.Configure(new WitnessesLayerFragmentSeederOptions
            {
                Ids = Array.Empty<string>()  // invalid
            });

            Assert.Null(seeder.GetFragment(_item, "1.1", "alpha"));
        }

        [Fact]
        public void Seed_ValidOptions_Ok()
        {
            WitnessesLayerFragmentSeeder seeder = new WitnessesLayerFragmentSeeder();
            seeder.SetSeedOptions(_seedOptions);
            string[] ids = new[]
                {
                    "alpha",
                    "beta",
                    "gamma"
                };
            seeder.Configure(new WitnessesLayerFragmentSeederOptions
            {
                Ids = ids
            });

            ITextLayerFragment fragment = seeder.GetFragment(_item, "1.1", "alpha");

            Assert.NotNull(fragment);

            WitnessesLayerFragment fr = fragment as WitnessesLayerFragment;
            Assert.NotNull(fr);

            Assert.Equal("1.1", fr.Location);
            Assert.NotEmpty(fr.Witnesses);
            foreach (Witness witness in fr.Witnesses)
            {
                Assert.True(Array.IndexOf(ids, witness.Id) > -1);
                Assert.NotNull(witness.Citation);
                Assert.NotNull(witness.Text);
            }
        }
    }
}
