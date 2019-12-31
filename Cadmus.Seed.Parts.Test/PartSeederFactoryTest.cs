using Cadmus.Core;
using Cadmus.Core.Config;
using Cadmus.Parts.General;
using Cadmus.Seed.Parts.General;
using SimpleInjector;
using System;
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
        public void GetPartSeeders_Ok()
        {
            Dictionary<string, IPartSeeder> seeders = _factory.GetPartSeeders();

            Assert.NotNull(seeders);
            Assert.Equal(4, seeders.Count);
        }
    }
}
