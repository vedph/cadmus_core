using Cadmus.Index.Graph;
using System.Collections.Generic;
using Xunit;

namespace Cadmus.Index.Test
{
    public sealed class CrudGrouperTest
    {
        private static bool HaveSameId(Guy a, Guy b) => a.Id == b.Id;

        [Fact]
        public void Group_Added_Ok()
        {
            CrudGrouper<Guy> grouper = new CrudGrouper<Guy>();

            grouper.Group(new List<Guy>
                {
                    new Guy(1, "alpha"),
                    new Guy(2, "beta")
                },
                new List<Guy>
                {
                    new Guy(1, "alpha")
                },
                HaveSameId);

            Assert.Single(grouper.Added);
            Assert.Equal("beta", grouper.Added[0].Name);

            Assert.Empty(grouper.Deleted);

            // existing items are in the updated group, we mind only for identity
            Assert.Single(grouper.Updated);
            Assert.Equal("alpha", grouper.Updated[0].Name);
        }

        [Fact]
        public void Group_Updated_Ok()
        {
            CrudGrouper<Guy> grouper = new CrudGrouper<Guy>();

            grouper.Group(new List<Guy>
                {
                    new Guy(1, "alpha")
                },
                new List<Guy>
                {
                    new Guy(1, "ALPHA")
                },
                HaveSameId);

            Assert.Single(grouper.Updated);
            Assert.Equal("alpha", grouper.Updated[0].Name);
            Assert.Empty(grouper.Added);
            Assert.Empty(grouper.Deleted);
        }

        [Fact]
        public void Group_Deleted_Ok()
        {
            CrudGrouper<Guy> grouper = new CrudGrouper<Guy>();

            grouper.Group(new List<Guy>
                {
                    new Guy(1, "alpha")
                },
                new List<Guy>
                {
                    new Guy(1, "alpha"),
                    new Guy(2, "beta")
                },
                HaveSameId);

            Assert.Single(grouper.Deleted);
            Assert.Equal("beta", grouper.Deleted[0].Name);

            Assert.Empty(grouper.Added);

            Assert.Single(grouper.Updated);
            Assert.Equal("alpha", grouper.Updated[0].Name);
        }

        [Fact]
        public void Group_AllTogether_Ok()
        {
            CrudGrouper<Guy> grouper = new CrudGrouper<Guy>();

            grouper.Group(new List<Guy>
                {
                    new Guy(1, "ALPHA"),
                    new Guy(3, "gamma"),
                },
                new List<Guy>
                {
                    new Guy(1, "alpha"),
                    new Guy(2, "beta")
                },
                HaveSameId);

            Assert.Single(grouper.Added);
            Assert.Equal("gamma", grouper.Added[0].Name);

            Assert.Single(grouper.Updated);
            Assert.Equal("ALPHA", grouper.Updated[0].Name);

            Assert.Single(grouper.Deleted);
            Assert.Equal("beta", grouper.Deleted[0].Name);
        }
    }

    internal sealed class Guy
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Guy()
        {
        }

        public Guy(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            return $"#{Id} {Name}";
        }
    }
}
