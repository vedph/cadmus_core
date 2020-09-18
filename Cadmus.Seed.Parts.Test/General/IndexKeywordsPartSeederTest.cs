using Cadmus.Core;
using Cadmus.Parts.General;
using Cadmus.Seed.Parts.General;
using Fusi.Tools.Config;
using System;
using System.Reflection;
using Xunit;

namespace Cadmus.Seed.Parts.Test.General
{
    public sealed class IndexKeywordsPartSeederTest
    {
        private static readonly PartSeederFactory _factory;
        private static readonly SeedOptions _seedOptions;
        private static readonly IItem _item;

        static IndexKeywordsPartSeederTest()
        {
            _factory = TestHelper.GetFactory();
            _seedOptions = _factory.GetSeedOptions();
            _item = _factory.GetItemSeeder().GetItem(1, "facet");
        }

        [Fact]
        public void TypeHasTagAttribute()
        {
            Type t = typeof(IndexKeywordsPartSeeder);
            TagAttribute attr = t.GetTypeInfo().GetCustomAttribute<TagAttribute>();
            Assert.NotNull(attr);
            Assert.Equal("seed.it.vedph.index-keywords", attr.Tag);
        }

        [Fact]
        public void Seed_NoOptions_Null()
        {
            IndexKeywordsPartSeeder seeder = new IndexKeywordsPartSeeder();
            seeder.SetSeedOptions(_seedOptions);

            Assert.Null(seeder.GetPart(_item, null, _factory));
        }

        [Fact]
        public void Seed_NoLanguages_Null()
        {
            IndexKeywordsPartSeeder seeder = new IndexKeywordsPartSeeder();
            seeder.SetSeedOptions(_seedOptions);
            seeder.Configure(new IndexKeywordsPartSeederOptions
            {
                IndexIds = new[]
                {
                    "",
                    "person-names"
                },
                Languages = Array.Empty<string>()  // invalid
            });

            Assert.Null(seeder.GetPart(_item, null, _factory));
        }

        [Fact]
        public void Seed_ValidOptions_Ok()
        {
            IndexKeywordsPartSeeder seeder = new IndexKeywordsPartSeeder();
            seeder.SetSeedOptions(_seedOptions);
            seeder.Configure(new IndexKeywordsPartSeederOptions
            {
                IndexIds = new[]
                {
                    "",
                    "person-names"
                },
                Languages = new[]
                {
                    "eng",
                    "ita",
                    "deu"
                }
            });

            IPart part = seeder.GetPart(_item, null, _factory);

            Assert.NotNull(part);

            IndexKeywordsPart cp = part as IndexKeywordsPart;
            Assert.NotNull(cp);

            TestHelper.AssertPartMetadata(cp);
            Assert.NotEmpty(cp.Keywords);
        }
    }
}
