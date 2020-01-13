using Cadmus.Parts.General;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cadmus.Core.Test
{
    public sealed class PartBaseTest
    {
        [Fact]
        public void Ctor_Id_Set()
        {
            NotePart part = new NotePart();

            Assert.Matches(
                "^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-" +
                "[0-9a-fA-F]{12}$", part.Id);
        }

        [Fact]
        public void Ctor_TypeId_Set()
        {
            NotePart part = new NotePart();

            Assert.Equal("net.fusisoft.note", part.TypeId);
        }

        [Fact]
        public void Ctor_Times_Set()
        {
            DateTime now = DateTime.UtcNow;
            NotePart part = new NotePart();

            Assert.True(part.TimeCreated >= now);
            Assert.True(part.TimeModified>= now);
        }

        [Fact]
        public void BuildProviderId_NoRole_EqualsTypeId()
        {
            string id = PartBase.BuildProviderId("net.fusisoft.note", null);
            Assert.Equal("net.fusisoft.note", id);
        }

        [Fact]
        public void BuildProviderId_RoleNonFr_EqualsTypeId()
        {
            string id = PartBase.BuildProviderId("net.fusisoft.date", "copy");
            Assert.Equal("net.fusisoft.date", id);
        }

        [Fact]
        public void BuildProviderId_RoleFr_EqualsTypeIdAndRoleId()
        {
            string id = PartBase.BuildProviderId(
                "net.fusisoft.token-text-layer", "fr.comment");
            Assert.Equal("net.fusisoft.token-text-layer:fr.comment", id);
        }

        [Fact]
        public void BuildProviderId_RoleFrPlusDot_EqualsTypeIdAndRoleId()
        {
            string id = PartBase.BuildProviderId("net.fusisoft.token-text-layer",
                "fr.net.fusisoft.comment:scholarly");
            Assert.Equal("net.fusisoft.token-text-layer:fr.net.fusisoft.comment", id);
        }

        [Fact]
        public void CreateDataPin_Note_CreatedWithIds()
        {
            NotePart part = new NotePart
            {
                ItemId = Guid.NewGuid().ToString(),
                RoleId = "role",
                Tag = "scholarly",
                Text = "The note's text."
            };

            List<DataPin> pins = part.GetDataPins().ToList();

            Assert.Single(pins);
            DataPin pin = pins[0];
            Assert.Equal(part.ItemId, pin.ItemId);
            Assert.Equal(part.Id, pin.PartId);
            Assert.Equal(part.RoleId, pin.RoleId);
            Assert.Equal("tag", pin.Name);
            Assert.Equal("scholarly", pin.Value);
        }
    }
}
