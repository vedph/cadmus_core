using Cadmus.Core;
using Cadmus.Parts.Layers;
using Cadmus.Seed.Parts.General;
using Cadmus.Seed.Parts.Layers;
using Fusi.Tools.Config;
using System;
using System.Reflection;
using Xunit;

namespace Cadmus.Seed.Parts.Test.Layers
{
    public sealed class TiledTextLayerPartSeederTest
    {
        private static readonly PartSeederFactory _factory;
        private static readonly SeedOptions _seedOptions;
        private static readonly IItem _item;

        static TiledTextLayerPartSeederTest()
        {
            _factory = TestHelper.GetFactory();
            _seedOptions = _factory.GetSeedOptions();
            _item = _factory.GetItemSeeder().GetItem(1, "facet");
        }

        [Fact]
        public void TypeHasTagAttribute()
        {
            Type t = typeof(TiledTextLayerPartSeeder);
            TagAttribute attr = t.GetTypeInfo().GetCustomAttribute<TagAttribute>();
            Assert.NotNull(attr);
            Assert.Equal("seed.net.fusisoft.tiled-text-layer", attr.Tag);
        }

        [Fact]
        public void Seed_NoOptions_Null()
        {
            TiledTextLayerPartSeeder seeder = new TiledTextLayerPartSeeder();
            seeder.SetSeedOptions(_seedOptions);

            IPart part = seeder.GetPart(_item, null, _factory);

            Assert.Null(part);
        }

        [Fact]
        public void Seed_InvalidOptions_Null()
        {
            TiledTextLayerPartSeeder seeder = new TiledTextLayerPartSeeder();
            seeder.SetSeedOptions(_seedOptions);
            seeder.Configure(new TiledTextLayerPartSeederOptions
            {
                MaxFragmentCount = 0
            });

            IPart part = seeder.GetPart(_item, null, _factory);

            Assert.Null(part);
        }

        [Fact]
        public void Seed_OptionsNoText_Null()
        {
            TiledTextLayerPartSeeder seeder = new TiledTextLayerPartSeeder();
            seeder.SetSeedOptions(_seedOptions);
            seeder.Configure(new TiledTextLayerPartSeederOptions
            {
                MaxFragmentCount = 3
            });

            IPart part = seeder.GetPart(_item, "fr.net.fusisoft.comment", _factory);

            Assert.Null(part);
        }

        [Fact]
        public void Seed_Options_Ok()
        {
            TiledTextLayerPartSeeder seeder = new TiledTextLayerPartSeeder();
            seeder.SetSeedOptions(_seedOptions);
            seeder.Configure(new TiledTextLayerPartSeederOptions
            {
                MaxFragmentCount = 3
            });

            // item with text
            IItem item = _factory.GetItemSeeder().GetItem(1, "facet");
            TiledTextPartSeeder textSeeder = new TiledTextPartSeeder();
            textSeeder.SetSeedOptions(_seedOptions);
            item.Parts.Add(textSeeder.GetPart(_item, null, _factory));

            IPart part = seeder.GetPart(item, "fr.net.fusisoft.comment", _factory);

            Assert.NotNull(part);

            TiledTextLayerPart<CommentLayerFragment> lp =
                part as TiledTextLayerPart<CommentLayerFragment>;
            Assert.NotNull(lp);
            Assert.NotEmpty(lp.Fragments);
        }
    }
}
