using Cadmus.Core;
using Cadmus.Core.Layers;
using Cadmus.Philology.Parts.Layers;
using Cadmus.Seed.Parts.Test;
using Cadmus.Seed.Philology.Parts.Layers;
using Fusi.Tools.Config;
using System;
using System.Reflection;
using Xunit;

namespace Cadmus.Seed.Philology.Parts.Test.Layers
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
        public void TypeHasTagAttribute()
        {
            Type t = typeof(ApparatusLayerFragmentSeeder);
            TagAttribute attr = t.GetTypeInfo().GetCustomAttribute<TagAttribute>();
            Assert.NotNull(attr);
            Assert.Equal("seed.fr.it.vedph.apparatus", attr.Tag);
        }

        [Fact]
        public void GetFragmentType_Ok()
        {
            ApparatusLayerFragmentSeeder seeder = new ApparatusLayerFragmentSeeder();
            Assert.Equal(typeof(ApparatusLayerFragment), seeder.GetFragmentType());
        }

        [Fact]
        public void Seed_WithOptions_Ok()
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
            Assert.NotEmpty(fr.Entries);
        }
    }
}
