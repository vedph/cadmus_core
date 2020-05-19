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
    public sealed class ChronologyLayerFragmentSeederTest
    {
        private static readonly PartSeederFactory _factory;
        private static readonly SeedOptions _seedOptions;
        private static readonly IItem _item;

        static ChronologyLayerFragmentSeederTest()
        {
            _factory = TestHelper.GetFactory();
            _seedOptions = _factory.GetSeedOptions();
            _item = _factory.GetItemSeeder().GetItem(1, "facet");
        }

        [Fact]
        public void TypeHasTagAttribute()
        {
            Type t = typeof(ChronologyLayerFragmentSeeder);
            TagAttribute attr = t.GetTypeInfo().GetCustomAttribute<TagAttribute>();
            Assert.NotNull(attr);
            Assert.Equal("seed.fr.net.fusisoft.chronology", attr.Tag);
        }

        [Fact]
        public void GetFragmentType_Ok()
        {
            ChronologyLayerFragmentSeeder seeder = new ChronologyLayerFragmentSeeder();
            Assert.Equal(typeof(ChronologyLayerFragment), seeder.GetFragmentType());
        }

        [Fact]
        public void Seed_WithOptions_Ok()
        {
            ChronologyLayerFragmentSeeder seeder = new ChronologyLayerFragmentSeeder();
            seeder.SetSeedOptions(_seedOptions);
            seeder.Configure(new ChronologyLayerFragmentSeederOptions
            {
                Tags = new[]
                {
                    "battle",
                    "priesthood",
                    "consulship"
                }
            });

            ITextLayerFragment fragment = seeder.GetFragment(_item, "1.1", "alpha");

            Assert.NotNull(fragment);

            ChronologyLayerFragment fr = fragment as ChronologyLayerFragment;
            Assert.NotNull(fr);

            Assert.Equal("1.1", fr.Location);
            Assert.NotNull(fr.Label);
            Assert.False(fr.Date.A.IsUndefined());
            Assert.NotNull(fr.EventId);
            Assert.NotNull(fr.Tag);
        }
    }
}
