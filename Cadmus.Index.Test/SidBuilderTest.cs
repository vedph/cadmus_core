using Cadmus.Index.Graph;
using Xunit;

namespace Cadmus.Index.Test
{
    public sealed class SidBuilderTest
    {
        private const string GUID = "72137e9b-3c6b-4788-9fe7-5b4869972a6f";

        [Fact]
        public void Build_User_Null()
        {
            string sid = SidBuilder.Build(NodeSourceType.User, GUID);

            Assert.Null(sid);
        }

        [Fact]
        public void Build_Item_Ok()
        {
            string sid = SidBuilder.Build(NodeSourceType.Item, GUID);

            Assert.Equal(GUID, sid);
        }

        [Fact]
        public void Build_ItemFacet_Ok()
        {
            string sid = SidBuilder.Build(NodeSourceType.ItemFacet, GUID);

            Assert.Equal(GUID + "/facet", sid);
        }

        [Fact]
        public void Build_ItemSimpleGroup_Ok()
        {
            string sid = SidBuilder.Build(NodeSourceType.ItemGroup, GUID);

            Assert.Equal(GUID + "/group", sid);
        }

        [Fact]
        public void Build_ItemCompositeGroup_Ok()
        {
            string sid = SidBuilder.Build(NodeSourceType.ItemGroup, GUID, 2);

            Assert.Equal(GUID + "/group/2", sid);
        }

        [Fact]
        public void Build_Part_Ok()
        {
            string sid = SidBuilder.Build(NodeSourceType.Pin, GUID, 0, null,
                "name", "Dan");

            Assert.Equal(GUID + "/name", sid);
        }

        [Fact]
        public void Build_PartWithRole_Ok()
        {
            string sid = SidBuilder.Build(NodeSourceType.Pin, GUID, 0, "role",
                "name", "Dan");

            Assert.Equal(GUID + ":role/name", sid);
        }

        [Fact]
        public void Build_PartWithSimpleEid_Ok()
        {
            string sid = SidBuilder.Build(NodeSourceType.Pin, GUID, 0, null,
                "eid", "angel-1v");

            Assert.Equal(GUID + "/eid/angel-1v", sid);
        }

        [Fact]
        public void Build_PartWithSimpleEidN_Ok()
        {
            string sid = SidBuilder.Build(NodeSourceType.Pin, GUID, 0, null,
                "eid2", "angel-1v");

            Assert.Equal(GUID + "/eid2/angel-1v", sid);
        }

        [Fact]
        public void Build_PartWithCompositeEid_Ok()
        {
            string sid = SidBuilder.Build(NodeSourceType.Pin, GUID, 0, null,
                "eid@some", "angel-1v");

            Assert.Equal(GUID + "/eid@some/angel-1v", sid);
        }

        [Fact]
        public void Build_PartWithCompositeEidN_Ok()
        {
            string sid = SidBuilder.Build(NodeSourceType.Pin, GUID, 0, null,
                "eid2@some", "angel-1v");

            Assert.Equal(GUID + "/eid2@some/angel-1v", sid);
        }
    }
}
