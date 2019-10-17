using Xunit;

namespace Cadmus.Core.Test
{
    public sealed class PartBaseTest
    {
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
    }
}
