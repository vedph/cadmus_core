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
    public sealed class QuotationsLayerFragmentSeederTest
    {
        private static readonly PartSeederFactory _factory;
        private static readonly SeedOptions _seedOptions;
        private static readonly IItem _item;

        static QuotationsLayerFragmentSeederTest()
        {
            _factory = TestHelper.GetFactory();
            _seedOptions = _factory.GetSeedOptions();
            _item = _factory.GetItemSeeder().GetItem(1, "facet");
        }

        [Fact]
        public void TypeHasTagAttribute()
        {
            Type t = typeof(QuotationsLayerFragmentSeeder);
            TagAttribute attr = t.GetTypeInfo().GetCustomAttribute<TagAttribute>();
            Assert.NotNull(attr);
            Assert.Equal("seed.fr.net.fusisoft.quotations", attr.Tag);
        }

        [Fact]
        public void GetFragmentType_Ok()
        {
            QuotationsLayerFragmentSeeder seeder = new QuotationsLayerFragmentSeeder();
            Assert.Equal(typeof(QuotationsLayerFragment), seeder.GetFragmentType());
        }

        [Fact]
        public void Seed_WithOptions_Ok()
        {
            QuotationsLayerFragmentSeeder seeder = new QuotationsLayerFragmentSeeder();
            seeder.SetSeedOptions(_seedOptions);
            seeder.Configure(new QuotationLayerFragmentSeederOptions
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

            QuotationsLayerFragment fr = fragment as QuotationsLayerFragment;
            Assert.NotNull(fr);

            Assert.Equal("1.1", fr.Location);
            Assert.NotEmpty(fr.Entries);
        }
    }
}
