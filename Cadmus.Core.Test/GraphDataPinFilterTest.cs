using System.Collections.Generic;
using Xunit;

namespace Cadmus.Core.Test
{
    public sealed class GraphDataPinFilterTest
    {
        [Fact]
        public void Apply_Ok()
        {
            GraphDataPinFilter filter = new GraphDataPinFilter();

            // whitelist for graph: g-
            filter.GraphPinFilter = new DataPinFilter();
            filter.GraphPinFilter.Clauses.Add(new DataPinFilterClause
            {
                Prefix = "g-"
            });

            // blacklist for index: g-
            filter.NonGraphPinFilter = new DataPinFilter
            {
                IsBlack = true
            };
            filter.NonGraphPinFilter.Clauses.Add(new DataPinFilterClause
            {
                Prefix = "g-"
            });

            // add some pins
            Assert.False(filter.Apply(new DataPin
            {
                Name = "g-alpha",
                Value = "a"
            }));
            Assert.True(filter.Apply(new DataPin
            {
                Name = "beta",
                Value = "b"
            }));
            Assert.True(filter.Apply(new DataPin
            {
                Name = "gamma",
                Value = "c"
            }));

            Assert.Single(filter.GraphPins);
            Assert.Equal("g-alpha", filter.GraphPins[0].Name);
        }

        [Fact]
        public void GetSortedDataPins_NoEid_Ok()
        {
            GraphDataPinFilter filter = new GraphDataPinFilter();
            filter.Apply(new DataPin
            {
                Name = "beta",
                Value = "b"
            });
            filter.Apply(new DataPin
            {
                Name = "alpha",
                Value = "a"
            });

            IList<DataPin> pins = filter.GetSortedGraphPins();

            Assert.Equal(2, pins.Count);
            Assert.Equal("beta", pins[0].Name);
            Assert.Equal("alpha", pins[1].Name);
        }

        [Fact]
        public void GetSortedDataPins_Eid_Ok()
        {
            GraphDataPinFilter filter = new GraphDataPinFilter();
            filter.Apply(new DataPin
            {
                Name = "beta",
                Value = "b"
            });
            filter.Apply(new DataPin
            {
                Name = "eid",
                Value = "a"
            });

            IList<DataPin> pins = filter.GetSortedGraphPins();

            Assert.Equal(2, pins.Count);
            Assert.Equal("eid", pins[0].Name);
            Assert.Equal("beta", pins[1].Name);
        }

        [Fact]
        public void GetSortedDataPins_EidN_Ok()
        {
            GraphDataPinFilter filter = new GraphDataPinFilter();
            filter.Apply(new DataPin
            {
                Name = "eid2",
                Value = "c"
            });
            filter.Apply(new DataPin
            {
                Name = "beta",
                Value = "b"
            });
            filter.Apply(new DataPin
            {
                Name = "eid3",
                Value = "d"
            });
            filter.Apply(new DataPin
            {
                Name = "eid",
                Value = "a"
            });

            IList<DataPin> pins = filter.GetSortedGraphPins();

            Assert.Equal(4, pins.Count);
            Assert.Equal("eid", pins[0].Name);
            Assert.Equal("eid2", pins[1].Name);
            Assert.Equal("eid3", pins[2].Name);
            Assert.Equal("beta", pins[3].Name);
        }
    }
}
