﻿using Cadmus.Core;
using Cadmus.Core.Layers;
using Cadmus.Parts.Layers;
using Cadmus.Seed.Parts.Layers;
using Fusi.Tools.Config;
using System;
using System.Reflection;
using Xunit;

namespace Cadmus.Seed.Parts.Test.Layers
{
    public sealed class CommentLayerFragmentSeederTest
    {
        private static readonly PartSeederFactory _factory;
        private static readonly SeedOptions _seedOptions;
        private static readonly IItem _item;

        static CommentLayerFragmentSeederTest()
        {
            _factory = TestHelper.GetFactory();
            _seedOptions = _factory.GetSeedOptions();
            _item = _factory.GetItemSeeder().GetItem(1, "facet");
        }

        [Fact]
        public void TypeHasTagAttribute()
        {
            Type t = typeof(CommentLayerFragmentSeeder);
            TagAttribute attr = t.GetTypeInfo().GetCustomAttribute<TagAttribute>();
            Assert.NotNull(attr);
            Assert.Equal("seed.fr.it.vedph.comment", attr.Tag);
        }

        [Fact]
        public void GetFragmentType_Ok()
        {
            CommentLayerFragmentSeeder seeder = new CommentLayerFragmentSeeder();
            Assert.Equal(typeof(CommentLayerFragment), seeder.GetFragmentType());
        }

        [Fact]
        public void Seed_WithoutTags_NullTag()
        {
            CommentLayerFragmentSeeder seeder = new CommentLayerFragmentSeeder();
            seeder.SetSeedOptions(_seedOptions);

            ITextLayerFragment fragment = seeder.GetFragment(_item, "1.1", "alpha");

            Assert.NotNull(fragment);

            CommentLayerFragment fr = fragment as CommentLayerFragment;
            Assert.NotNull(fr);

            Assert.Equal("1.1", fr.Location);
            Assert.Null(fr.Tag);
            Assert.NotNull(fr.Text);
        }

        [Fact]
        public void Seed_WithTags_Ok()
        {
            CommentLayerFragmentSeeder seeder = new CommentLayerFragmentSeeder();
            seeder.SetSeedOptions(_seedOptions);
            seeder.Configure(new CommentLayerFragmentSeederOptions
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

            CommentLayerFragment fr = fragment as CommentLayerFragment;
            Assert.NotNull(fr);

            Assert.Equal("1.1", fr.Location);
            Assert.NotNull(fr.Tag);
            Assert.NotNull(fr.Text);
        }
    }
}
