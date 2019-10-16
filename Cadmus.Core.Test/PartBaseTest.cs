using Xunit;

namespace Cadmus.Core.Test
{
    public sealed class PartBaseTest
    {
        [Fact]
        public void BuildProviderId_NoRole_EqualsTypeId()
        {
            string id = PartBase.BuildProviderId("note", null);
            Assert.Equal("note", id);
        }

        [Fact]
        public void BuildProviderId_RoleNonFr_EqualsTypeId()
        {
            string id = PartBase.BuildProviderId("date", "copy");
            Assert.Equal("date", id);
        }

        [Fact]
        public void BuildProviderId_RoleFr_EqualsTypeIdAndRoleId()
        {
            string id = PartBase.BuildProviderId("token-text-layer", "fr-comment");
            Assert.Equal("token-text-layer:fr-comment", id);
        }

        [Fact]
        public void BuildProviderId_RoleFrPlusDot_EqualsTypeIdAndRoleId()
        {
            string id = PartBase.BuildProviderId("token-text-layer",
                "fr-comment.scholarly");
            Assert.Equal("token-text-layer:fr-comment", id);
        }
    }
}
