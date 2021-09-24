using Cadmus.Index.Graph;
using System;
using Xunit;

namespace Cadmus.Index.Test
{
    public sealed class NodeMappingVariableSetTest
    {
        [Fact]
        public void LoadFrom_NoVariable_Empty()
        {
            NodeMappingVariableSet nmv = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "literal"
                });

            Assert.Equal(0, nmv.Count);
        }

        [Fact]
        public void LoadFrom_PlaceholderInPrefix_Ok()
        {
            NodeMappingVariableSet nmv = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "literal",
                    Prefix = "x://{facet-id}s/"
                });

            Assert.Equal(1, nmv.Count);
            NodeMappingVariable v = nmv.GetVariable("facet-id");
            Assert.NotNull(v);
            Assert.Equal("facet-id", v.Name);
            Assert.Equal(0, v.Argument);
            Assert.Null(v.Value);
        }

        [Fact]
        public void LoadFrom_PlaceholderWithArgument_Ok()
        {
            NodeMappingVariableSet nmv = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "{pin-eid:2}",
                });

            Assert.Equal(1, nmv.Count);
            NodeMappingVariable v = nmv.GetVariable("pin-eid:2");
            Assert.NotNull(v);
            Assert.Equal("pin-eid", v.Name);
            Assert.Equal(2, v.Argument);
            Assert.Null(v.Value);
        }

        [Fact]
        public void LoadFrom_PlaceholdersInPrefix_Ok()
        {
            NodeMappingVariableSet nmv = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "literal",
                    Prefix = "x://{facet-id}s/{group-id}/"
                });

            Assert.Equal(2, nmv.Count);

            NodeMappingVariable v = nmv.GetVariable("facet-id");
            Assert.NotNull(v);
            Assert.Equal("facet-id", v.Name);
            Assert.Equal(0, v.Argument);
            Assert.Null(v.Value);

            v = nmv.GetVariable("group-id");
            Assert.NotNull(v);
            Assert.Equal("group-id", v.Name);
            Assert.Equal(0, v.Argument);
            Assert.Null(v.Value);
        }
    }
}
