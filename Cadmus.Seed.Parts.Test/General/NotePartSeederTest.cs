using Cadmus.Core;
using Cadmus.Parts.General;
using Cadmus.Seed.Parts.General;
using Fusi.Tools.Config;
using System;
using System.Reflection;
using Xunit;

namespace Cadmus.Seed.Parts.Test.General
{
    public sealed class NotePartSeederTest
    {
        private static readonly PartSeederFactory _factory;
        private static readonly SeedOptions _seedOptions;
        private static readonly IItem _item;

        static NotePartSeederTest()
        {
            _factory = TestHelper.GetFactory();
            _seedOptions = _factory.GetSeedOptions();
            _item = _factory.GetItemSeeder().GetItem(1, "facet");
        }

        [Fact]
        public void TypeHasTagAttribute()
        {
            Type t = typeof(NotePartSeeder);
            TagAttribute attr = t.GetTypeInfo().GetCustomAttribute<TagAttribute>();
            Assert.NotNull(attr);
            Assert.Equal("seed.it.vedph.note", attr.Tag);
        }

        [Fact]
        public void Seed_NoOptions_NullTag()
        {
            NotePartSeeder seeder = new NotePartSeeder();
            seeder.SetSeedOptions(_seedOptions);

            IPart part = seeder.GetPart(_item, null, _factory);

            Assert.NotNull(part);

            NotePart np = part as NotePart;
            Assert.NotNull(np);

            TestHelper.AssertPartMetadata(np);
            Assert.Null(np.Tag);
            Assert.NotNull(np.Text);
        }

        [Fact]
        public void Seed_Options_Tag()
        {
            NotePartSeeder seeder = new NotePartSeeder();
            seeder.SetSeedOptions(_seedOptions);
            seeder.Configure(new NotePartSeederOptions
            {
                Tags = new[] { "alpha", "beta", "gamma" }
            });

            IPart part = seeder.GetPart(_item, null, _factory);

            Assert.NotNull(part);

            NotePart np = part as NotePart;
            Assert.NotNull(np);

            TestHelper.AssertPartMetadata(np);
            Assert.NotNull(np.Tag);
            Assert.NotNull(np.Text);
        }
    }
}
