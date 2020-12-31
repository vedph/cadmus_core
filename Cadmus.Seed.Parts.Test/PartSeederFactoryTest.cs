using Cadmus.Seed.Parts.General;
using Cadmus.Seed.Parts.Layers;
using System.Collections.Generic;
using Xunit;

namespace Cadmus.Seed.Parts.Test
{
    public class PartSeederFactoryTest
    {
        static private readonly PartSeederFactory _factory =
            TestHelper.GetFactory();

        [Fact]
        public void GetItemSeeder_Ok()
        {
            ItemSeeder seeder = _factory.GetItemSeeder();

            Assert.NotNull(seeder);
        }

        [Fact]
        public void GetItemSortKeyBuilder_NoEntry_Null()
        {
            Assert.Null(_factory.GetItemSortKeyBuilder());
        }

        [Fact]
        public void GetPartSeeders_Ok()
        {
            Dictionary<string, IPartSeeder> seeders = _factory.GetPartSeeders();

            Assert.NotNull(seeders);
            Assert.Equal(9, seeders.Count);

            string key = "it.vedph.categories";
            Assert.True(seeders.ContainsKey(key));
            Assert.NotNull(seeders[key] as CategoriesPartSeeder);

            key = "it.vedph.historical-date";
            Assert.True(seeders.ContainsKey(key));
            Assert.NotNull(seeders[key] as HistoricalDatePartSeeder);

            key = "it.vedph.keywords";
            Assert.True(seeders.ContainsKey(key));
            Assert.NotNull(seeders[key] as KeywordsPartSeeder);

            key = "it.vedph.comment";
            Assert.True(seeders.ContainsKey(key));
            Assert.NotNull(seeders[key] as CommentPartSeeder);

            key = "it.vedph.note";
            Assert.True(seeders.ContainsKey(key));
            Assert.NotNull(seeders[key] as NotePartSeeder);

            key = "it.vedph.token-text";
            Assert.True(seeders.ContainsKey(key));
            Assert.NotNull(seeders[key] as TokenTextPartSeeder);

            key = "it.vedph.token-text-layer";
            Assert.True(seeders.ContainsKey(key));
            Assert.NotNull(seeders[key] as TokenTextLayerPartSeeder);

            key = "it.vedph.token-text";
            Assert.True(seeders.ContainsKey(key));
            Assert.NotNull(seeders[key] as TokenTextPartSeeder);

            key = "it.vedph.token-text-layer";
            Assert.True(seeders.ContainsKey(key));
            Assert.NotNull(seeders[key] as TokenTextLayerPartSeeder);

            key = "it.vedph.tiled-text";
            Assert.True(seeders.ContainsKey(key));
            Assert.NotNull(seeders[key] as TiledTextPartSeeder);

            key = "it.vedph.tiled-text-layer";
            Assert.True(seeders.ContainsKey(key));
            Assert.NotNull(seeders[key] as TiledTextLayerPartSeeder);
        }

        [Fact]
        public void GetFragmentSeeder_Comment_Ok()
        {
            IFragmentSeeder seeder = _factory.GetFragmentSeeder(
                "seed.fr.it.vedph.comment");
            Assert.NotNull(seeder);
        }
    }
}
