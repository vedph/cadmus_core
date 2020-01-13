using Cadmus.Parts.General;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cadmus.Parts.Test.General
{
    public sealed class HierarchyPartTest
    {
        private const string PARENT_GUID = "33358d39-e1c1-4e85-a6cb-012962a3cf10";
        private const string CHILD1_GUID = "eb3d5571-4b71-4c1b-8226-3d8389841273";
        private const string CHILD2_GUID = "c9e3a3df-d858-4552-baaa-61f5860c7ca1";

        private static HierarchyPart GetPart()
        {
            HierarchyPart part = new HierarchyPart
            {
                ItemId = Guid.NewGuid().ToString(),
                RoleId = "some-role",
                CreatorId = "zeus",
                UserId = "another",
                ParentId = PARENT_GUID,
                Y = 1,
                X = 2,
                Tag = "some-tag"
            };
            part.ChildrenIds.Add(CHILD1_GUID);
            part.ChildrenIds.Add(CHILD2_GUID);
            return part;
        }

        [Fact]
        public void Part_Is_Serializable()
        {
            HierarchyPart part = GetPart();

            string json = TestHelper.SerializePart(part);
            HierarchyPart part2 = TestHelper.DeserializePart<HierarchyPart>(json);

            Assert.Equal(part.Id, part2.Id);
            Assert.Equal(part.TypeId, part2.TypeId);
            Assert.Equal(part.ItemId, part2.ItemId);
            Assert.Equal(part.RoleId, part2.RoleId);
            Assert.Equal(part.CreatorId, part2.CreatorId);
            Assert.Equal(part.UserId, part2.UserId);

            Assert.Equal(part.Y, part2.Y);
            Assert.Equal(part.X, part2.X);
            Assert.Equal(part.Tag, part2.Tag);
            Assert.Equal(part.ParentId, part2.ParentId);
            Assert.Equal(part.ChildrenIds.Count, part2.ChildrenIds.Count);
            Assert.Contains(CHILD1_GUID, part2.ChildrenIds);
            Assert.Contains(CHILD2_GUID, part2.ChildrenIds);
        }

        [Fact]
        public void GetDataPins_NoTag_2()
        {
            HierarchyPart part = GetPart();
            part.Tag = null;

            Dictionary<string, string> pins = part.GetDataPins()
                .ToDictionary(p => p.Name, p => p.Value);

            Assert.Equal(2, pins.Count);
            Assert.True(pins.ContainsKey("y"));
            Assert.Equal("1", pins["y"]);
            Assert.True(pins.ContainsKey("x"));
            Assert.Equal("2", pins["x"]);
        }

        [Fact]
        public void GetDataPins_Tag_3()
        {
            HierarchyPart part = GetPart();

            Dictionary<string, string> pins = part.GetDataPins()
                .ToDictionary(p => p.Name, p => p.Value);

            Assert.Equal(3, pins.Count);
            Assert.True(pins.ContainsKey("y"));
            Assert.Equal("1", pins["y"]);
            Assert.True(pins.ContainsKey("x"));
            Assert.Equal("2", pins["x"]);
            Assert.True(pins.ContainsKey("tag"));
            Assert.Equal("some-tag", pins["tag"]);
        }
    }
}
