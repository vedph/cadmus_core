using Cadmus.Core;
using Cadmus.Parts.General;
using Cadmus.Seed.Parts.General;
using System;
using Xunit;

namespace Cadmus.Seed.Parts.Test
{
    public sealed class KeywordsPartSeederTest
    {
        private static readonly PartSeederFactory _factory;
        private static readonly SeedOptions _seedOptions;
        private static readonly IItem _item;

        static KeywordsPartSeederTest()
        {
            _factory = TestHelper.GetFactory();
            _seedOptions = _factory.GetSeedOptions();
            _item = _factory.GetItemSeeder().GetItem(1, "facet");
        }

        [Fact]
        public void Seed_NoOptions_Null()
        {
            KeywordsPartSeeder seeder = new KeywordsPartSeeder();
            seeder.SetSeedOptions(_seedOptions);

            Assert.Null(seeder.GetPart(_item, null, _factory));
        }

        [Fact]
        public void Seed_NoLanguages_Null()
        {
            KeywordsPartSeeder seeder = new KeywordsPartSeeder();
            seeder.SetSeedOptions(_seedOptions);
            seeder.Configure(new KeywordsPartSeederOptions
            {
                Languages = Array.Empty<string>()  // invalid
            });

            Assert.Null(seeder.GetPart(_item, null, _factory));
        }

        [Fact]
        public void Seed_ValidOptions_Ok()
        {
            KeywordsPartSeeder seeder = new KeywordsPartSeeder();
            seeder.SetSeedOptions(_seedOptions);
            seeder.Configure(new KeywordsPartSeederOptions
            {
                Languages = new[]
                {
                    "eng",
                    "ita",
                    "deu"
                }
            });

            IPart part = seeder.GetPart(_item, null, _factory);

            Assert.NotNull(part);

            KeywordsPart cp = part as KeywordsPart;
            Assert.NotNull(cp);

            TestHelper.AssertPartMetadata(cp);
            Assert.NotEmpty(cp.Keywords);
        }
    }
}
