using Cadmus.Core;
using Cadmus.Index.Graph;
using Xunit;

namespace Cadmus.Index.Test
{
    public sealed class NodeMappingVariableSetTest
    {
        #region LoadFrom
        [Fact]
        public void LoadFrom_NoVariable_Empty()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "literal"
                });

            Assert.Equal(0, set.Count);
        }

        [Fact]
        public void LoadFrom_PlaceholderInPrefix_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "literal",
                    Prefix = "x:{facet-id}s/"
                });

            Assert.Equal(1, set.Count);
            NodeMappingVariable v = set.GetVariable("facet-id");
            Assert.NotNull(v);
            Assert.Equal("facet-id", v.Name);
            Assert.False(v.HasArguments);
            Assert.Null(v.Value);
        }

        [Fact]
        public void LoadFrom_PlaceholderInTripleOPrefix_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "literal",
                    TripleOPrefix = "x:{facet-id}s/"
                });

            Assert.Equal(1, set.Count);
            NodeMappingVariable v = set.GetVariable("facet-id");
            Assert.NotNull(v);
            Assert.Equal("facet-id", v.Name);
            Assert.False(v.HasArguments);
            Assert.Null(v.Value);
        }

        [Fact]
        public void LoadFrom_PlaceholderInLabelTemplate_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "persons/{facet-id}s/{title-uid}",
                    Prefix = "x:"
                });

            Assert.Equal(2, set.Count);

            NodeMappingVariable v = set.GetVariable("facet-id");
            Assert.NotNull(v);
            Assert.Equal("facet-id", v.Name);
            Assert.False(v.HasArguments);
            Assert.Null(v.Value);

            v = set.GetVariable("title-uid");
            Assert.NotNull(v);
            Assert.Equal("title-uid", v.Name);
            Assert.False(v.HasArguments);
            Assert.Null(v.Value);
        }

        [Fact]
        public void LoadFrom_MacroInTripleS_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "literal",
                    TripleS = "$item"
                });

            Assert.Equal(1, set.Count);
            NodeMappingVariable v = set.GetVariable("item");
            Assert.NotNull(v);
            Assert.Equal("item", v.Name);
            Assert.False(v.HasArguments);
            Assert.Null(v.Value);
        }

        [Fact]
        public void LoadFrom_MacroInTripleP_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "literal",
                    TripleP = "$item"
                });

            Assert.Equal(1, set.Count);
            NodeMappingVariable v = set.GetVariable("item");
            Assert.NotNull(v);
            Assert.Equal("item", v.Name);
            Assert.False(v.HasArguments);
            Assert.Null(v.Value);
        }

        [Fact]
        public void LoadFrom_MacroInTripleO_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "literal",
                    TripleO = "$item"
                });

            Assert.Equal(1, set.Count);
            NodeMappingVariable v = set.GetVariable("item");
            Assert.NotNull(v);
            Assert.Equal("item", v.Name);
            Assert.False(v.HasArguments);
            Assert.Null(v.Value);
        }

        [Fact]
        public void LoadFrom_PlaceholderWithArgument_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "{pin-eid:2}",
                });

            Assert.Equal(1, set.Count);
            NodeMappingVariable v = set.GetVariable("pin-eid:2");
            Assert.NotNull(v);
            Assert.Equal("pin-eid", v.Name);
            Assert.Equal(2, v.GetArgument(0, 0));
            Assert.Null(v.Value);
        }

        [Fact]
        public void LoadFrom_PlaceholdersInPrefix_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "literal",
                    Prefix = "x:{facet-id}s/{group-id}/"
                });

            Assert.Equal(2, set.Count);

            NodeMappingVariable v = set.GetVariable("facet-id");
            Assert.NotNull(v);
            Assert.Equal("facet-id", v.Name);
            Assert.False(v.HasArguments);
            Assert.Null(v.Value);

            v = set.GetVariable("group-id");
            Assert.NotNull(v);
            Assert.Equal("group-id", v.Name);
            Assert.False(v.HasArguments);
            Assert.Null(v.Value);
        }
        #endregion

        #region SetValues
        [Fact]
        public void SetValues_NoVariable_Nope()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "literal"
                });

            set.SetValues(new NodeMapperState
            {
                Item = new Item
                {
                    Title = "Sample"
                }
            });

            Assert.Equal(0, set.Count);
        }

        [Fact]
        public void SetValues_SimpleTitle_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "Hello {title}"
                });

            set.SetValues(new NodeMapperState
            {
                Item = new Item
                {
                    Title = "A Sample Item"
                }
            });

            Assert.Equal("A Sample Item", set.GetVariable("title")?.Value);
        }

        [Fact]
        public void SetValues_TitleWithUid_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "Hello {title}"
                });

            set.SetValues(new NodeMapperState
            {
                Item = new Item
                {
                    Title = "A Sample Item [#x:the_uid]"
                }
            });

            Assert.Equal("A Sample Item", set.GetVariable("title")?.Value);
        }

        [Fact]
        public void SetValues_TitleWithUidUsingBoth_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "Hello {title} and {title-uid}"
                });

            set.SetValues(new NodeMapperState
            {
                Item = new Item
                {
                    Title = "A Sample Item [#x:the_uid]"
                }
            });

            Assert.Equal("A Sample Item", set.GetVariable("title").Value);
            Assert.Equal("x:the_uid", set.GetVariable("title-uid").Value);
        }

        [Fact]
        public void SetValues_TitleWithPrefix_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "Hello {title}"
                });

            set.SetValues(new NodeMapperState
            {
                Item = new Item
                {
                    Title = "A Sample Item [@x:items/]"
                }
            });

            Assert.Equal("A Sample Item", set.GetVariable("title")?.Value);
        }

        [Fact]
        public void SetValues_TitleWithPrefixUsingBoth_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "Hello {title} and {title-prefix}"
                });

            set.SetValues(new NodeMapperState
            {
                Item = new Item
                {
                    Title = "A Sample Item [@x:items/]"
                }
            });

            Assert.Equal("A Sample Item", set.GetVariable("title").Value);
            Assert.Equal("x:items/", set.GetVariable("title-prefix").Value);
        }

        [Fact]
        public void SetValues_FacetId_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "Hello {facet-id}"
                });

            set.SetValues(new NodeMapperState
            {
                Item = new Item
                {
                    Title = "John Doe",
                    FacetId = "person"
                }
            });

            Assert.Equal("person", set.GetVariable("facet-id")?.Value);
        }

        [Fact]
        public void SetValues_SimpleGroupId_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "Hello {group-id}"
                });

            set.SetValues(new NodeMapperState
            {
                Item = new Item
                {
                    Title = "John Doe",
                    GroupId = "patients"
                }
            });

            Assert.Equal("patients", set.GetVariable("group-id")?.Value);
        }

        [Fact]
        public void SetValues_CompositeGroupId_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "Hello {group-id}"
                });

            set.SetValues(new NodeMapperState
            {
                Item = new Item
                {
                    Title = "John Doe",
                    GroupId = "patients/alpha"
                }
            });

            Assert.Equal("patients/alpha", set.GetVariable("group-id").Value);
        }

        [Fact]
        public void SetValues_CompositeGroupId1_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "Hello {group-id:1}"
                });

            set.SetValues(new NodeMapperState
            {
                Item = new Item
                {
                    Title = "John Doe",
                    GroupId = "patients/alpha"
                }
            });

            Assert.Equal("patients", set.GetVariable("group-id:1")?.Value);
        }

        [Fact]
        public void SetValues_CompositeGroupId2_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "Hello {group-id:2}"
                });

            set.SetValues(new NodeMapperState
            {
                Item = new Item
                {
                    Title = "John Doe",
                    GroupId = "patients/alpha"
                }
            });

            Assert.Equal("alpha", set.GetVariable("group-id:2").Value);
        }

        [Fact]
        public void SetValues_Description_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "Hello {dsc}"
                });

            set.SetValues(new NodeMapperState
            {
                Item = new Item
                {
                    Title = "A Sample Item",
                    Description = "This is a description."
                }
            });

            Assert.Equal("This is a description.", set.GetVariable("dsc")?.Value);
        }

        [Fact]
        public void SetValues_PinName_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "Hello {pin-name}"
                });

            set.SetValues(new NodeMapperState
            {
                Item = new Item
                {
                    Title = "A Sample Item",
                },
                PinName = "name"
            });

            Assert.Equal("name", set.GetVariable("pin-name").Value);
        }

        [Fact]
        public void SetValues_PinNameWithSuffix_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "Hello {pin-name}"
                });

            set.SetValues(new NodeMapperState
            {
                Item = new Item
                {
                    Title = "A Sample Item",
                },
                PinName = "name@some"
            });

            Assert.Equal("name", set.GetVariable("pin-name")?.Value);
        }

        [Fact]
        public void SetValues_PinValue_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "Hello {pin-value}"
                });

            set.SetValues(new NodeMapperState
            {
                Item = new Item
                {
                    Title = "A Sample Item",
                },
                PinName = "name",
                PinValue = "John Doe"
            });

            Assert.Equal("John Doe", set.GetVariable("pin-value")?.Value);
        }

        [Fact]
        public void SetValues_PinEid_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "Hello {pin-eid}"
                });

            set.SetValues(new NodeMapperState
            {
                Item = new Item
                {
                    Title = "A Sample Item",
                },
                PinName = "eid@alpha@beta"
            });

            Assert.Equal("alpha@beta", set.GetVariable("pin-eid")?.Value);
        }

        [Fact]
        public void SetValues_PinEid1_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "Hello {pin-eid:1}"
                });

            set.SetValues(new NodeMapperState
            {
                Item = new Item
                {
                    Title = "A Sample Item",
                },
                PinName = "eid@alpha@beta"
            });

            Assert.Equal("alpha", set.GetVariable("pin-eid:1")?.Value);
        }

        [Fact]
        public void SetValues_PinEid2_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "Hello {pin-eid:2}"
                });

            set.SetValues(new NodeMapperState
            {
                Item = new Item
                {
                    Title = "A Sample Item",
                },
                PinName = "eid@alpha@beta"
            });

            Assert.Equal("beta", set.GetVariable("pin-eid:2")?.Value);
        }

        private static IItem GetItem()
        {
            return new Item
            {
                Title = "Sample Item",
                Description = "A sample item.",
                FacetId = "default",
                GroupId = "samples",
                SortKey = "a-sample-item",
                CreatorId = "zeus",
                UserId = "zeus"
            };
        }

        [Fact]
        public void SetValues_ParentNoParent_Null()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "Sample",
                    TripleS = "$parent"
                });
            NodeMapperState state = new NodeMapperState
            {
                Item = GetItem()
            };
            set.SetValues(state);

            Assert.Null(set.GetVariable("parent")?.Value);
        }

        [Fact]
        public void SetValues_Parent_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    Id = 2,
                    ParentId = 1,
                    LabelTemplate = "Sample",
                    TripleS = "$parent"
                });
            NodeMapperState state = new NodeMapperState
            {
                Item = GetItem()
            };
            state.MappingPath.Add(1);
            state.MappedUris[1] = "x:the_parent_node";

            set.SetValues(state);

            Assert.Equal(state.MappedUris[1], set.GetVariable("parent")?.Value);
        }

        [Fact]
        public void SetValues_Ancestor1_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    Id = 2,
                    ParentId = 1,
                    LabelTemplate = "Sample",
                    TripleS = "$ancestor:1"
                });
            NodeMapperState state = new NodeMapperState
            {
                Item = GetItem()
            };
            state.MappingPath.Add(1);
            state.MappedUris[1] = "x:the_parent_node";

            set.SetValues(state);

            Assert.Equal(state.MappedUris[1],
                set.GetVariable("ancestor:1")?.Value);
        }

        [Fact]
        public void SetValues_Ancestor2_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    Id = 3,
                    ParentId = 2,
                    LabelTemplate = "Sample",
                    TripleS = "$ancestor:2"
                });
            NodeMapperState state = new NodeMapperState
            {
                Item = GetItem()
            };
            state.MappingPath.Add(1);
            state.MappingPath.Add(2);
            state.MappedUris[1] = "x:granpa";
            state.MappedUris[2] = "x:pa";

            set.SetValues(state);

            Assert.Equal(state.MappedUris[1],
                set.GetVariable("ancestor:2")?.Value);
        }

        [Fact]
        public void SetValues_Item_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    Id = 1,
                    LabelTemplate = "Sample",
                    TripleS = "$item"
                });
            NodeMapperState state = new NodeMapperState
            {
                Item = GetItem()
            };
            state.AddNode(
                new NodeResult
                {
                    Id = 1,
                    Uri = "x:sample",
                    Label = "sample",
                    SourceType = NodeSourceType.Item
                }, 1);

            set.SetValues(state);

            Assert.Equal(state.MappedUris[1], set.GetVariable("item")?.Value);
        }

        [Fact]
        public void SetValues_Facet_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    Id = 1,
                    LabelTemplate = "Sample",
                    TripleS = "$facet"
                });
            NodeMapperState state = new NodeMapperState
            {
                Item = GetItem()
            };
            state.AddNode(
                new NodeResult
                {
                    Id = 1,
                    Uri = "x:sample",
                    Label = "sample",
                    SourceType = NodeSourceType.ItemFacet
                }, 1);

            set.SetValues(state);

            Assert.Equal(state.MappedUris[1], set.GetVariable("facet")?.Value);
        }

        [Fact]
        public void SetValues_SimpleGroup_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    Id = 1,
                    LabelTemplate = "Sample",
                    TripleS = "$group"
                });
            NodeMapperState state = new NodeMapperState
            {
                Item = GetItem(),
                GroupOrdinal = 0
            };
            state.AddNode(
                new NodeResult
                {
                    Id = 1,
                    Uri = "x:animals",
                    Label = "animals",
                    SourceType = NodeSourceType.ItemGroup
                }, 1);

            set.SetValues(state);

            Assert.Equal(state.GroupUids[0], set.GetVariable("group")?.Value);
        }

        [Fact]
        public void SetValues_CompositeGroup1_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    Id = 1,
                    LabelTemplate = "Sample",
                    TripleS = "$group:1"
                });
            // 1st component
            NodeMapperState state = new NodeMapperState
            {
                Item = GetItem(),
                GroupOrdinal = 1
            };
            state.AddNode(
                new NodeResult
                {
                    Id = 1,
                    Uri = "x:animals",
                    Label = "animals",
                    SourceType = NodeSourceType.ItemGroup
                }, 1);
            // 2nd component
            state.GroupOrdinal = 2;
            state.AddNode(
                new NodeResult
                {
                    Id = 2,
                    Uri = "x:humans",
                    Label = "humans",
                    SourceType = NodeSourceType.ItemGroup
                }, 2);

            set.SetValues(state);

            Assert.Equal(state.GroupUids[1], set.GetVariable("group:1")?.Value);
        }

        [Fact]
        public void SetValues_CompositeGroup2_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    Id = 1,
                    LabelTemplate = "Sample",
                    TripleS = "$group:2"
                });
            // 1st component
            NodeMapperState state = new NodeMapperState
            {
                Item = GetItem(),
                GroupOrdinal = 1
            };
            state.AddNode(
                new NodeResult
                {
                    Id = 1,
                    Uri = "x:animals",
                    Label = "animals",
                    SourceType = NodeSourceType.ItemGroup
                }, 1);
            // 2nd component
            state.GroupOrdinal = 2;
            state.AddNode(
                new NodeResult
                {
                    Id = 2,
                    Uri = "x:humans",
                    Label = "humans",
                    SourceType = NodeSourceType.ItemGroup
                }, 2);

            set.SetValues(state);

            Assert.Equal(state.GroupUids[2], set.GetVariable("group:2")?.Value);
        }

        [Fact]
        public void SetValues_PinUid_Ok()
        {
            NodeMappingVariableSet set = NodeMappingVariableSet.LoadFrom(
                new NodeMapping
                {
                    LabelTemplate = "Hello",
                    TripleS = "$pin-uid"
                });

            set.SetValues(new NodeMapperState
            {
                Item = new Item
                {
                    Title = "A Sample Item",
                },
                PinName = "eid",
                PinValue = "x:guys/john_doe"
            });

            Assert.Equal("x:guys/john_doe", set.GetVariable("pin-uid")?.Value);
        }
        #endregion
    }
}
