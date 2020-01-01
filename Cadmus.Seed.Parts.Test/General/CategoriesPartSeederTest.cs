using Cadmus.Core;
using Cadmus.Parts.General;
using Cadmus.Seed.Parts.General;
using System;
using Xunit;

namespace Cadmus.Seed.Parts.Test.General
{
    public sealed class CategoriesPartSeederTest
    {
        private static readonly PartSeederFactory _factory;
        private static readonly SeedOptions _seedOptions;
        private static readonly IItem _item;

        static CategoriesPartSeederTest()
        {
            _factory = TestHelper.GetFactory();
            _seedOptions = _factory.GetSeedOptions();
            _item = _factory.GetItemSeeder().GetItem(1, "facet");
        }

        [Fact]
        public void Seed_NoOptions_Null()
        {
            CategoriesPartSeeder seeder = new CategoriesPartSeeder();
            seeder.SetSeedOptions(_seedOptions);

            Assert.Null(seeder.GetPart(_item, null, _factory));
        }

        [Fact]
        public void Seed_InvalidMax_Null()
        {
            CategoriesPartSeeder seeder = new CategoriesPartSeeder();
            seeder.SetSeedOptions(_seedOptions);
            seeder.Configure(new CategoriesPartSeederOptions
            {
                MaxCategoriesPerItem = 0,   // invalid
                Categories = new[]
                {
                    "alpha",
                    "beta",
                    "gamma"
                }
            });

            Assert.Null(seeder.GetPart(_item, null, _factory));
        }

        [Fact]
        public void Seed_NoCategories_Null()
        {
            CategoriesPartSeeder seeder = new CategoriesPartSeeder();
            seeder.SetSeedOptions(_seedOptions);
            seeder.Configure(new CategoriesPartSeederOptions
            {
                MaxCategoriesPerItem = 3,
                Categories = Array.Empty<string>()  // invalid
            });

            Assert.Null(seeder.GetPart(_item, null, _factory));
        }

        [Fact]
        public void Seed_ValidOptions_Ok()
        {
            CategoriesPartSeeder seeder = new CategoriesPartSeeder();
            seeder.SetSeedOptions(_seedOptions);
            seeder.Configure(new CategoriesPartSeederOptions
            {
                MaxCategoriesPerItem = 3,
                Categories = new[]
                {
                    "alpha",
                    "beta",
                    "gamma"
                }
            });

            IPart part = seeder.GetPart(_item, null, _factory);

            Assert.NotNull(part);

            CategoriesPart cp = part as CategoriesPart;
            Assert.NotNull(cp);

            TestHelper.AssertPartMetadata(cp);
            Assert.NotEmpty(cp.Categories);
        }
    }
}
