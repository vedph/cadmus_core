using Cadmus.Core;
using Cadmus.Parts.General;
using Cadmus.Seed.Parts.General;
using Fusi.Tools.Config;
using System;
using System.Reflection;
using Xunit;

namespace Cadmus.Seed.Parts.Test.General
{
    public sealed class HistoricalDatePartSeederTest
    {
        private static readonly PartSeederFactory _factory;
        private static readonly SeedOptions _seedOptions;
        private static readonly IItem _item;

        static HistoricalDatePartSeederTest()
        {
            _factory = TestHelper.GetFactory();
            _seedOptions = _factory.GetSeedOptions();
            _item = _factory.GetItemSeeder().GetItem(1, "facet");
        }

        [Fact]
        public void TypeHasTagAttribute()
        {
            Type t = typeof(HistoricalDatePartSeeder);
            TagAttribute attr = t.GetTypeInfo().GetCustomAttribute<TagAttribute>();
            Assert.NotNull(attr);
            Assert.Equal("seed.it.vedph.historical-date", attr.Tag);
        }

        [Fact]
        public void Seed_Optionless_Ok()
        {
            HistoricalDatePartSeeder seeder = new HistoricalDatePartSeeder();
            seeder.SetSeedOptions(_seedOptions);

            IPart part = seeder.GetPart(_item, null, _factory);

            Assert.NotNull(part);

            HistoricalDatePart hp = part as HistoricalDatePart;
            Assert.NotNull(hp);

            TestHelper.AssertPartMetadata(hp);
        }
    }
}
