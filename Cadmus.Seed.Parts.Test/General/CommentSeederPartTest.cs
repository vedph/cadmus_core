using Cadmus.Core;
using Cadmus.Parts.General;
using Cadmus.Seed.Parts.General;
using Fusi.Tools.Config;
using System;
using System.Reflection;
using Xunit;

namespace Cadmus.Seed.Parts.Test.General
{
    public sealed class CommentPartSeederTest
    {
        private static readonly PartSeederFactory _factory;
        private static readonly SeedOptions _seedOptions;
        private static readonly IItem _item;

        static CommentPartSeederTest()
        {
            _factory = TestHelper.GetFactory();
            _seedOptions = _factory.GetSeedOptions();
            _item = _factory.GetItemSeeder().GetItem(1, "facet");
        }

        [Fact]
        public void TypeHasTagAttribute()
        {
            Type t = typeof(CommentPartSeeder);
            TagAttribute attr = t.GetTypeInfo().GetCustomAttribute<TagAttribute>();
            Assert.NotNull(attr);
            Assert.Equal("seed.it.vedph.comment", attr.Tag);
        }

        [Fact]
        public void Seed_ValidOptions_Ok()
        {
            CommentPartSeeder seeder = new CommentPartSeeder();
            seeder.SetSeedOptions(_seedOptions);
            seeder.Configure(new CommentPartSeederOptions());

            IPart part = seeder.GetPart(_item, null, _factory);

            Assert.NotNull(part);

            CommentPart cp = part as CommentPart;
            Assert.NotNull(cp);

            TestHelper.AssertPartMetadata(cp);
            Assert.NotEmpty(cp.References);
            Assert.NotEmpty(cp.ExternalIds);
            Assert.NotEmpty(cp.Categories);
            Assert.NotEmpty(cp.Keywords);
        }
    }
}
