using System;
using Xunit;

namespace Cadmus.Core.Test
{
    public sealed class ItemTest
    {
        [Fact]
        public void Ctor_Id_Set()
        {
            Item item = new Item();

            Assert.Matches(
                "^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-" +
                "[0-9a-fA-F]{12}$", item.Id);
        }

        [Fact]
        public void Ctor_Times_Set()
        {
            DateTime now = DateTime.UtcNow;
            Item item = new Item();

            Assert.True(item.TimeCreated >= now);
            Assert.True(item.TimeModified >= now);
        }
    }
}
