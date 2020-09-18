using Cadmus.Core;
using Cadmus.Parts.Layers;
using Cadmus.Philology.Parts.Layers;
using Cadmus.Seed.Parts.General;
using Cadmus.Seed.Parts.Layers;
using Cadmus.Seed.Parts.Test;
using Fusi.Tools.Config;
using System;
using System.Reflection;
using Xunit;

namespace Cadmus.Seed.Philology.Parts.Test.Layers
{
    // this is to test the layer part with a fragment coming from this assembly

    public sealed class TokenTextLayerPartSeederTest
    {
        private static readonly PartSeederFactory _factory;
        private static readonly SeedOptions _seedOptions;
        private static readonly IItem _item;

        static TokenTextLayerPartSeederTest()
        {
            _factory = TestHelper.GetFactory();
            _seedOptions = _factory.GetSeedOptions();
            _item = _factory.GetItemSeeder().GetItem(1, "facet");
        }

        [Fact]
        public void TypeHasTagAttribute()
        {
            Type t = typeof(TokenTextLayerPartSeeder);
            TagAttribute attr = t.GetTypeInfo().GetCustomAttribute<TagAttribute>();
            Assert.NotNull(attr);
            Assert.Equal("seed.it.vedph.token-text-layer", attr.Tag);
        }

        [Fact]
        public void Seed_NoOptions_Null()
        {
            TokenTextLayerPartSeeder seeder = new TokenTextLayerPartSeeder();
            seeder.SetSeedOptions(_seedOptions);

            IPart part = seeder.GetPart(_item, null, _factory);

            Assert.Null(part);
        }

        [Fact]
        public void Seed_InvalidOptions_Null()
        {
            TokenTextLayerPartSeeder seeder = new TokenTextLayerPartSeeder();
            seeder.SetSeedOptions(_seedOptions);
            seeder.Configure(new TokenTextLayerPartSeederOptions
            {
                MaxFragmentCount = 0
            });

            IPart part = seeder.GetPart(_item, null, _factory);

            Assert.Null(part);
        }

        [Fact]
        public void Seed_OptionsNoText_Null()
        {
            TokenTextLayerPartSeeder seeder = new TokenTextLayerPartSeeder();
            seeder.SetSeedOptions(_seedOptions);
            seeder.Configure(new TokenTextLayerPartSeederOptions
            {
                MaxFragmentCount = 3
            });

            IPart part = seeder.GetPart(_item, "fr.it.vedph.quotations", _factory);

            Assert.Null(part);
        }

        [Fact]
        public void Seed_Options_Ok()
        {
            TokenTextLayerPartSeeder seeder = new TokenTextLayerPartSeeder();
            seeder.SetSeedOptions(_seedOptions);
            seeder.Configure(new TokenTextLayerPartSeederOptions
            {
                MaxFragmentCount = 3
            });

            // item with text
            IItem item = _factory.GetItemSeeder().GetItem(1, "facet");
            TokenTextPartSeeder textSeeder = new TokenTextPartSeeder();
            textSeeder.SetSeedOptions(_seedOptions);
            item.Parts.Add(textSeeder.GetPart(_item, null, _factory));

            IPart part = seeder.GetPart(item, "fr.it.vedph.quotations", _factory);

            Assert.NotNull(part);

            TokenTextLayerPart<QuotationsLayerFragment> lp =
                part as TokenTextLayerPart<QuotationsLayerFragment>;
            Assert.NotNull(lp);
            Assert.NotEmpty(lp.Fragments);
        }
    }
}
