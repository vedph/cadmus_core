using Cadmus.Core;
using Cadmus.Parts.General;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cadmus.Parts.Test.General
{
    public sealed class NotePartTest
    {
        private static NotePart GetPart()
        {
            return new NotePart
            {
                ItemId = Guid.NewGuid().ToString(),
                RoleId = "some-role",
                CreatorId = "zeus",
                UserId = "another",
                Tag = "some-tag",
                Text = "Text.\nEnd of text here."
            };
        }

        [Fact]
        public void Part_Is_Serializable()
        {
            NotePart part = GetPart();

            string json = TestHelper.SerializePart(part);
            NotePart part2 = TestHelper.DeserializePart<NotePart>(json);

            Assert.Equal(part.Id, part2.Id);
            Assert.Equal(part.TypeId, part2.TypeId);
            Assert.Equal(part.ItemId, part2.ItemId);
            Assert.Equal(part.RoleId, part2.RoleId);
            Assert.Equal(part.CreatorId, part2.CreatorId);
            Assert.Equal(part.UserId, part2.UserId);
            Assert.Equal(part.Tag, part2.Tag);
            Assert.Equal(part.Text, part2.Text);
        }

        [Fact]
        public void GetDataPins_NoTag_Empty()
        {
            NotePart part = GetPart();
            part.Tag = null;

            Assert.Empty(part.GetDataPins());
        }

        [Fact]
        public void GetDataPins_Tag_1()
        {
            NotePart part = GetPart();

            List<DataPin> pins = part.GetDataPins().ToList();
            Assert.Single(pins);

            DataPin pin = pins[0];

            Assert.Equal(part.ItemId, pin.ItemId);
            Assert.Equal(part.Id, pin.PartId);
            Assert.Equal(part.RoleId, pin.RoleId);
            Assert.Equal("tag", pin.Name);
            Assert.Equal("some-tag", pin.Value);
        }
    }
}
