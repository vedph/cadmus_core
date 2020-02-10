﻿using System;
using System.Collections.Generic;
using System.Linq;
using Cadmus.Core;
using Cadmus.Core.Config;
using Cadmus.Core.Storage;
using Cadmus.Parts.General;
using Cadmus.Parts.Layers;
using Xunit;

namespace Cadmus.TestBase
{
    /// <summary>
    /// Base class for Cadmus repository tests.
    /// </summary>
    public abstract class CadmusRepositoryTestBase
    {
        protected abstract ICadmusRepository GetRepository();

        protected abstract void PrepareDatabase();

        #region Flags
        protected void DoGetFlagDefinitions_Ok()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            List<FlagDefinition> defs = repository.GetFlagDefinitions().ToList();

            Assert.Equal(2, defs.Count);

            FlagDefinition def = defs[0];
            Assert.Equal(1, def.Id);
            Assert.Equal("Alpha", def.Label);
            Assert.Equal("The alpha flag", def.Description);
            Assert.Equal("FF0000", def.ColorKey);

            def = defs[1];
            Assert.Equal(2, def.Id);
            Assert.Equal("Beta", def.Label);
            Assert.Equal("The beta flag", def.Description);
            Assert.Equal("00FF00", def.ColorKey);
        }

        protected void DoGetFlagDefinition_NotExisting_Null()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            FlagDefinition def = repository.GetFlagDefinition(8);

            Assert.Null(def);
        }

        protected void DoGetFlagDefinition_Existing_Ok()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            FlagDefinition def = repository.GetFlagDefinition(1);

            Assert.NotNull(def);
            Assert.Equal(1, def.Id);
            Assert.Equal("Alpha", def.Label);
            Assert.Equal("The alpha flag", def.Description);
            Assert.Equal("FF0000", def.ColorKey);
        }

        protected void DoAddFlagDefinition_Existing_Updated()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            FlagDefinition def = new FlagDefinition
            {
                Id = 1,
                Label = "Alpha",
                Description = "The alpha",
                ColorKey = "FF00FF"
            };

            repository.AddFlagDefinition(def);

            FlagDefinition def2 = repository.GetFlagDefinition(1);
            Assert.Equal(1, def2.Id);
            Assert.Equal(def.Label, def2.Label);
            Assert.Equal(def.Description, def2.Description);
            Assert.Equal(def.ColorKey, def2.ColorKey);
        }

        protected void DoAddFlagDefinition_NotExisting_Added()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            FlagDefinition def = new FlagDefinition
            {
                Id = 4,
                Label = "Gamma",
                Description = "The gamma flag",
                ColorKey = "FF00FF"
            };

            repository.AddFlagDefinition(def);

            FlagDefinition def2 = repository.GetFlagDefinition(4);
            Assert.Equal(def.Id, def2.Id);
            Assert.Equal(def.Label, def2.Label);
            Assert.Equal(def.Description, def2.Description);
            Assert.Equal(def.ColorKey, def2.ColorKey);

            Assert.Equal(3, repository.GetFlagDefinitions().Count);
        }

        protected void DoDeleteFlagDefinition_NotExisting_Nope()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            repository.DeleteFlagDefinition(4);

            Assert.Equal(2, repository.GetFlagDefinitions().Count);
        }

        protected void DoDeleteFlagDefinition_Existing_Deleted()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            repository.DeleteFlagDefinition(4);

            Assert.Equal(2, repository.GetFlagDefinitions().Count);
        }
        #endregion

        #region Facets
        protected void DoGetFacets_Ok()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            IList<FacetDefinition> facets = repository.GetFacetDefinitions();

            Assert.Equal(2, facets.Count);

            FacetDefinition facet = facets[0];
            Assert.Equal("alpha", facet.Id);
            Assert.Equal("Alpha", facet.Label);
            Assert.Equal("Alpha facet", facet.Description);
            Assert.Equal("FF0000", facet.ColorKey);
            Assert.Equal(2, facet.PartDefinitions.Count);

            facet = facets[1];
            Assert.Equal("beta", facet.Id);
            Assert.Equal("Beta", facet.Label);
            Assert.Equal("Beta facet", facet.Description);
            Assert.Equal("00FF00", facet.ColorKey);
            Assert.Equal(2, facet.PartDefinitions.Count);
        }

        protected void DoGetFacet_NotExisting_Null()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            FacetDefinition facet = repository.GetFacetDefinition("NotExisting");

            Assert.Null(facet);
        }

        protected void DoGetFacet_Existing_Ok()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            FacetDefinition facet = repository.GetFacetDefinition("alpha");

            Assert.NotNull(facet);
            Assert.Equal("alpha", facet.Id);
            Assert.Equal("Alpha", facet.Label);
            Assert.Equal("Alpha facet", facet.Description);
            Assert.Equal("FF0000", facet.ColorKey);
            Assert.Equal(2, facet.PartDefinitions.Count);
        }

        protected void DoAddFacet_Existing_Updated()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            FacetDefinition facet = new FacetDefinition
            {
                Id = "alpha",
                Label = "Alpha",
                Description = "Alpha facet description",
                ColorKey = "101010",
                PartDefinitions =
                {
                    new PartDefinition
                    {
                        TypeId = "categories",
                        RoleId = "categories",
                        Name = "Categories",
                        Description = "Item's categories updated",
                        IsRequired = true,
                        ColorKey = "FF00FF",
                        GroupKey = "General",
                        SortKey = "categories",
                        EditorKey = "module1"
                    },
                    new PartDefinition
                    {
                        TypeId = "keywords",
                        RoleId = "keywords",
                        Name = "Keywords",
                        Description = "Generic keywords",
                        IsRequired = false,
                        ColorKey = "00FF00",
                        GroupKey = "General",
                        SortKey = "keywords",
                        EditorKey = "module2"
                    }
                }
            };

            repository.AddFacetDefinition(facet);

            FacetDefinition facet2 = repository.GetFacetDefinition(facet.Id);
            Assert.NotNull(facet2);
            Assert.Equal(facet.Id, facet2.Id);
            Assert.Equal(facet.Label, facet2.Label);
            Assert.Equal(facet.Description, facet2.Description);
            Assert.Equal(facet.ColorKey, facet2.ColorKey);
            Assert.Equal(facet.PartDefinitions.Count, facet2.PartDefinitions.Count);

            Assert.Contains(facet2.PartDefinitions, p => p.TypeId == "categories");
            Assert.Contains(facet2.PartDefinitions, p => p.TypeId == "keywords");
        }

        protected void DoAddFacet_NotExisting_Added()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            FacetDefinition facet = new FacetDefinition
            {
                Id = "gamma",
                Label = "Gamma",
                Description = "A new facet",
                ColorKey = "101010",
                PartDefinitions =
                {
                    new PartDefinition
                    {
                        TypeId = "note",
                        RoleId = "note",
                        Name = "Note",
                        Description = "Generic note",
                        IsRequired = false,
                        ColorKey = "00FF80",
                        GroupKey = "General",
                        SortKey = "note",
                        EditorKey = "module1"
                    }
                }
            };

            repository.AddFacetDefinition(facet);

            FacetDefinition facet2 = repository.GetFacetDefinition(facet.Id);
            Assert.NotNull(facet2);
            Assert.Equal(facet.Id, facet2.Id);
            Assert.Equal(facet.Label, facet2.Label);
            Assert.Equal(facet.Description, facet2.Description);
            Assert.Equal(facet.ColorKey, facet2.ColorKey);
            Assert.Equal(facet.PartDefinitions.Count, facet2.PartDefinitions.Count);

            Assert.Equal(3, repository.GetFacetDefinitions().Count);
        }

        protected void DoDeleteFacet_NotExisting_Nope()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            repository.DeleteFacetDefinition("NotExisting");

            Assert.Equal(2, repository.GetFacetDefinitions().Count);
        }

        protected void DoDeleteFacet_Existing_Deleted()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            repository.DeleteFacetDefinition("beta");

            Assert.Equal(1, repository.GetFacetDefinitions().Count);
        }
        #endregion

        #region Thesauri
        private void SeedThesauri(ICadmusRepository repository, int count)
        {
            for (int i = 1; i <= count; i++)
            {
                Thesaurus thesaurus = new Thesaurus($"t{i}@en");
                for (int j = 1; j <= 5; j++)
                {
                    thesaurus.AddEntry(new ThesaurusEntry
                        ($"entry{j}", $"value of entry{j}"));
                }
                repository.AddThesaurus(thesaurus);
            }
        }

        protected void DoGetThesaurusIds_Ok()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();
            SeedThesauri(repository, 3);

            IList<string> ids = repository.GetThesaurusIds();

            Assert.Equal(3, ids.Count);
            for (int i = 1; i <= 3; i++)
            {
                Assert.Equal($"t{i}@en", ids[i - 1]);
            }
        }

        protected void DoGetThesaurus_NotExisting_Null()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();
            SeedThesauri(repository, 1);

            Thesaurus thesaurus = repository.GetThesaurus("notexisting@en");

            Assert.Null(thesaurus);
        }

        protected void DoGetThesaurus_Existing_Ok()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();
            SeedThesauri(repository, 2);

            Thesaurus thesaurus = repository.GetThesaurus("t2@en");

            Assert.NotNull(thesaurus);
            Assert.Equal("en", thesaurus.GetLanguage());
            var entries = thesaurus.GetEntries();
            Assert.Equal(5, entries.Count);
            for (int i = 1; i <= 5; i++)
            {
                Assert.Equal($"entry{i}", entries[i - 1].Id);
                Assert.Equal($"value of entry{i}", entries[i - 1].Value);
            }
        }

        protected void DoAddThesaurus_NotExisting_Added()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            SeedThesauri(repository, 1);

            Thesaurus thesaurus = repository.GetThesaurus("t1@en");

            Assert.NotNull(thesaurus);
            Assert.Equal("en", thesaurus.GetLanguage());
            IList<ThesaurusEntry> entries = thesaurus.GetEntries();
            Assert.Equal(5, entries.Count);
            for (int i = 1; i <= 5; i++)
            {
                Assert.Equal($"entry{i}", entries[i - 1].Id);
                Assert.Equal($"value of entry{i}", entries[i - 1].Value);
            }
        }

        protected void DoAddThesaurus_Existing_Updated()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();
            SeedThesauri(repository, 1);
            Thesaurus thesaurus = repository.GetThesaurus("t1@en");
            thesaurus.AddEntry(new ThesaurusEntry("added", "here I am"));

            repository.AddThesaurus(thesaurus);

            Assert.NotNull(thesaurus);
            Assert.Equal("en", thesaurus.GetLanguage());
            IList<ThesaurusEntry> entries = thesaurus.GetEntries();
            Assert.Equal(6, entries.Count);
            for (int i = 1; i <= 5; i++)
            {
                Assert.Equal($"entry{i}", entries[i - 1].Id);
                Assert.Equal($"value of entry{i}", entries[i - 1].Value);
            }
            ThesaurusEntry entry = entries[5];
            Assert.Equal("added", entry.Id);
            Assert.Equal("here I am", entry.Value);
        }

        protected void DoDeleteThesaurus_NotExisting_Nope()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();
            SeedThesauri(repository, 1);

            repository.DeleteThesaurus("notexisting");

            Assert.Single(repository.GetThesaurusIds());
        }

        protected void DoDeleteThesaurus_Existing_Deleted()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();
            SeedThesauri(repository, 1);

            repository.DeleteThesaurus("t1@en");

            Assert.Empty(repository.GetThesaurusIds());
        }
        #endregion

        #region Items
        protected void DoGetItemsPage_1_Ok()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            var result = repository.GetItems(new ItemFilter
            {
                PageNumber = 1,
                PageSize = 10
            });

            Assert.Equal(10, result.Items.Count);
            Assert.Equal(20, result.Total);
            for (int i = 0; i < 10; i++)
                Assert.Equal($"item-{i + 1:000}", result.Items[i].Id);
        }

        protected void DoGetItemsPage_2_Ok()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            var result = repository.GetItems(new ItemFilter
            {
                PageNumber = 2,
                PageSize = 10
            });

            Assert.Equal(10, result.Items.Count);
            Assert.Equal(20, result.Total);
            for (int i = 0; i < 10; i++)
                Assert.Equal($"item-{i + 11:000}", result.Items[i].Id);
        }

        protected void DoGetItemsPage_10_Empty()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            var result = repository.GetItems(new ItemFilter
            {
                PageNumber = 10,
                PageSize = 10
            });

            Assert.Equal(0, result.Items.Count);
            Assert.Equal(20, result.Total);
        }

        protected void DoGetItem_NotExisting_Null()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            IItem item = repository.GetItem("NotExisting");

            Assert.Null(item);
        }

        protected void DoGetItem_Existing_Ok()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            IItem item = repository.GetItem("item-001");

            DateTime expectedTime = new DateTime(2015, 12, 1, 0, 0, 0, DateTimeKind.Utc);
            Assert.NotNull(item);
            Assert.Equal("Item 1", item.Title);
            Assert.Equal("Description of item 1", item.Description);
            Assert.Equal("alpha", item.FacetId);
            Assert.Equal("item001", item.SortKey);
            Assert.Equal(1, item.Flags);
            Assert.Equal("Odd", item.CreatorId);
            Assert.Equal("Odd", item.UserId);
            Assert.Equal(expectedTime, item.TimeCreated);
            Assert.Equal(expectedTime, item.TimeModified);

            // parts
            Assert.Equal(3, item.Parts.Count);
        }

        protected void DoGetItem_ExistingExcludeParts_Ok()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            IItem item = repository.GetItem("item-001", false);

            DateTime expectedTime = new DateTime(2015, 12, 1, 0, 0, 0, DateTimeKind.Utc);
            Assert.NotNull(item);
            Assert.Equal("Item 1", item.Title);
            Assert.Equal("Description of item 1", item.Description);
            Assert.Equal("alpha", item.FacetId);
            Assert.Equal("item001", item.SortKey);
            Assert.Equal(1, item.Flags);
            Assert.Equal("Odd", item.CreatorId);
            Assert.Equal(expectedTime, item.TimeCreated);
            Assert.Equal("Odd", item.UserId);
            Assert.Equal(expectedTime, item.TimeModified);
            Assert.Empty(item.Parts);
        }

        protected void DoAddItem_Existing_Updated()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();
            IItem item = new Item
            {
                Id = "item-100",
                Title = "Item 100",
                Description = "This item was added.",
                FacetId = "alpha",
                SortKey = "item100",
                Flags = 0,
                CreatorId = "Even",
                UserId = "Even"
            };

            repository.AddItem(item);

            IItem item2 = repository.GetItem("item-100", false);
            Assert.Equal(item2.Id, item.Id);
            Assert.Equal(item2.Title, item.Title);
            Assert.Equal(item2.Description, item.Description);
            Assert.Equal(item2.FacetId, item.FacetId);
            Assert.Equal(item2.SortKey, item.SortKey);
            Assert.Equal(item2.Flags, item.Flags);
            Assert.Equal(item2.CreatorId, item.CreatorId);
            Assert.Equal(item2.UserId, item.UserId);
            Assert.True(Math.Abs((item2.TimeModified - item.TimeModified).TotalSeconds) < 1);
        }

        protected void DoAddItem_NotExisting_Added()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();
            IItem item = new Item
            {
                Id = "item-100",
                Title = "Item 100",
                Description = "This item was added.",
                FacetId = "alpha",
                SortKey = "item100",
                Flags = 0,
                CreatorId = "Even",
                UserId = "Even"
            };

            repository.AddItem(item);

            Assert.Equal(21, repository.GetItems(
                new ItemFilter { PageSize = 30 }).Total);
        }

        protected void DoSetItemFlags_NotExisting_Nope()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            repository.SetItemFlags(new[] { "NotExisting" }, 128);

            Assert.Equal(0, repository.GetItems(
                new ItemFilter {Flags = 128}).Total);
        }

        protected void DoSetItemFlags_Existing_Updated()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            repository.SetItemFlags(new[] { "item-001" }, 128);

            Assert.Equal(1, repository.GetItems(
                new ItemFilter { Flags = 128 }).Total);
        }

        protected void DoDeleteItem_NotExisting_Nope()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            repository.DeleteItem("NotExisting", "Odd");

            Assert.Equal(20, repository.GetItems(
                new ItemFilter { PageSize = 20 }).Total);
        }

        protected void DoDeleteItem_ExistingNoParts_Deleted()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            repository.DeleteItem("item-002", "Killer");

            IItem item = repository.GetItem("item-002", false);
            Assert.Null(item);

            // history
            var result = repository.GetHistoryItems(new HistoryItemFilter
            {
                ReferenceId = "item-002"
            });
            Assert.Equal(1, result.Total);

            HistoryItemInfo historyItem = result.Items[0];
            Assert.NotEqual("item-002", historyItem.Id);
            Assert.Equal("item-002", historyItem.ReferenceId);
            Assert.Equal(EditStatus.Deleted, historyItem.Status);
            Assert.Equal("Item 2", historyItem.Title);
            Assert.Equal("beta", historyItem.FacetId);
            Assert.Equal("Description of item 2", historyItem.Description);
            Assert.Equal("item002", historyItem.SortKey);
            Assert.Equal(2, historyItem.Flags);
            Assert.Equal("Killer", historyItem.UserId);
        }

        protected void DoDeleteItem_ExistingNoPartsNoHistory_Deleted()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            repository.DeleteItem("item-002", "Killer", false);

            IItem item = repository.GetItem("item-002", false);
            Assert.Null(item);

            // history
            var result = repository.GetHistoryItems(new HistoryItemFilter
            {
                ReferenceId = "item-002"
            });
            Assert.Equal(0, result.Total);
        }

        protected void DoDeleteItem_ExistingWithParts_Deleted()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            repository.DeleteItem("item-001", "Killer");

            IItem item = repository.GetItem("item-001", false);
            Assert.Null(item);

            var result = repository.GetHistoryParts(new HistoryPartFilter
            {
                ItemIds = new[] { "item-001" }
            });
            // categories and note history parts
            Assert.Equal(3, result.Total);
        }

        protected void DoGetItemLayerInfo_NoAbsentNoItem_Empty()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            var ids = repository.GetItemLayerInfo("item-not-existing", false);

            Assert.Empty(ids);
        }

        protected void DoGetItemLayerInfo_AbsentNoItem_Empty()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            var ids = repository.GetItemLayerInfo("item-not-existing", true);

            Assert.Empty(ids);
        }

        private string AddItemWithLayers(ICadmusRepository repository,
            bool addPart = false)
        {
            // a facet with 1 base text part and 2 layer parts
            FacetDefinition facet = new FacetDefinition
            {
                Id = "default",
                Label = "default",
                ColorKey = "FF0000",
                Description = "Test",
            };
            facet.PartDefinitions.Add(
                new PartDefinition
                {
                    TypeId = "net.fusisoft.token-text",
                    RoleId = "base-text",
                    Name = "text",
                    Description = "Text",
                    IsRequired = true,
                    ColorKey = "00FF00",
                    GroupKey = "general",
                    SortKey = "text",
                    EditorKey = "general"
                });
            facet.PartDefinitions.Add(
                new PartDefinition
                {
                    TypeId = "net.fusisoft.token-text-layer",
                    RoleId = "fr.net.fusisoft.comment",
                    Name = "comments layer",
                    Description = "Generic comments",
                    IsRequired = false,
                    ColorKey = "FF0000",
                    GroupKey = "layers",
                    SortKey = "comments",
                    EditorKey = "general"
                });
            facet.PartDefinitions.Add(
                new PartDefinition
                {
                    TypeId = "net.fusisoft.token-text-layer",
                    RoleId = "fr.net.fusisoft.apparatus",
                    Name = "apparatus layer",
                    Description = "Critical apparatus",
                    IsRequired = false,
                    ColorKey = "0000FF",
                    GroupKey = "layers",
                    SortKey = "apparatus",
                    EditorKey = "philology"
                });

            // assign item to this facet
            repository.AddFacetDefinition(facet);
            IItem item = new Item
            {
                FacetId = facet.Id,
                Description = "A new item",
                SortKey = "newitem",
                Title = "New item",
                UserId = "zeus",
                CreatorId = "zeus"
            };
            repository.AddItem(item, false);

            // add comment part if requested
            if (addPart)
            {
                TokenTextLayerPart<CommentLayerFragment> part =
                    new TokenTextLayerPart<CommentLayerFragment>
                    {
                        ItemId = item.Id,
                        CreatorId = "zeus",
                        UserId = "zeus"
                    };
                part.AddFragment(new CommentLayerFragment
                {
                    Location = "1.2",
                    Text = "A comment"
                });
                repository.AddPart(part, false);
            }

            return item.Id;
        }

        protected void DoGetItemLayerInfo_ItemNoAbsentNoPart_0()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();
            string id = AddItemWithLayers(repository);

            IList<LayerPartInfo> parts =
                repository.GetItemLayerInfo(id, false);

            Assert.Empty(parts);
        }

        protected void DoGetItemLayerInfo_ItemAbsentNoPart_2()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();
            string id = AddItemWithLayers(repository);

            IList<LayerPartInfo> parts =
                repository.GetItemLayerInfo(id, true);

            Assert.Equal(2, parts.Count);
            Assert.Equal(2, parts.Count(p => p.IsAbsent));

            // comments
            LayerPartInfo part = parts.FirstOrDefault(
                p => p.RoleId == "fr.net.fusisoft.comment");
            Assert.NotNull(part);
            Assert.Equal("net.fusisoft.token-text-layer", part.TypeId);
            Assert.Equal(0, part.FragmentCount);

            // apparatus
            part = parts.FirstOrDefault(
                p => p.RoleId == "fr.net.fusisoft.apparatus");
            Assert.NotNull(part);
            Assert.Equal("net.fusisoft.token-text-layer", part.TypeId);
            Assert.Equal(0, part.FragmentCount);
        }

        protected void DoGetItemLayerInfo_ItemAbsent1Part_2()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();
            string id = AddItemWithLayers(repository, true);

            IList<LayerPartInfo> parts =
                repository.GetItemLayerInfo(id, true);

            Assert.Equal(2, parts.Count);

            // comments
            LayerPartInfo part = parts.FirstOrDefault(
                p => p.RoleId == "fr.net.fusisoft.comment");
            Assert.NotNull(part);
            Assert.Equal("net.fusisoft.token-text-layer", part.TypeId);
            Assert.False(part.IsAbsent);
            Assert.Equal(1, part.FragmentCount);

            // apparatus
            part = parts.FirstOrDefault(
                p => p.RoleId == "fr.net.fusisoft.apparatus");
            Assert.NotNull(part);
            Assert.Equal("net.fusisoft.token-text-layer", part.TypeId);
            Assert.True(part.IsAbsent);
            Assert.Equal(0, part.FragmentCount);
        }
        #endregion

        #region Parts
        protected void DoGetPartsPage_1Any_2()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            var page = repository.GetParts(new PartFilter());

            Assert.Equal(3, page.Items.Count);
            Assert.Equal(3, page.Total);

            // categories
            PartInfo info = page.Items[0];
            Assert.Equal("item-001", info.ItemId);
            Assert.Equal("net.fusisoft.categories", info.TypeId);
            Assert.Equal("categories", info.RoleId);

            // note
            info = page.Items[1];
            Assert.Equal("item-001", info.ItemId);
            Assert.Equal("net.fusisoft.note", info.TypeId);
            Assert.Equal("note", info.RoleId);

            // layer
            info = page.Items[2];
            Assert.Equal("item-001", info.ItemId);
            Assert.Equal("net.fusisoft.token-text-layer", info.TypeId);
            Assert.Equal("fr.net.fusisoft.comment", info.RoleId);
        }

        protected void DoGetPartsPage_1TypeId_1()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            var page = repository.GetParts(new PartFilter {TypeId = "net.fusisoft.note" });

            Assert.Equal(1, page.Items.Count);
            Assert.Equal(1, page.Total);

            // note
            PartInfo info = page.Items[0];
            Assert.Equal("item-001", info.ItemId);
            Assert.Equal("net.fusisoft.note", info.TypeId);
            Assert.Equal("note", info.RoleId);
        }

        protected void DoGetPartsPage_1RoleId_1()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            var page = repository.GetParts(new PartFilter { RoleId = "note" });

            Assert.Equal(1, page.Items.Count);
            Assert.Equal(1, page.Total);

            // note
            PartInfo info = page.Items[0];
            Assert.Equal("item-001", info.ItemId);
            Assert.Equal("net.fusisoft.note", info.TypeId);
            Assert.Equal("note", info.RoleId);
        }

        private static void SeedHierarchyParts(ICadmusRepository repository)
        {
            HierarchyPart part1 = new HierarchyPart
            {
                ItemId = "item-001",
                ChildrenIds = new HashSet<string> { "item-002", "item-003" },
                Y = 1,
                X = 1,
                CreatorId = "Alpha",
                UserId = "Alpha"
            };
            repository.AddPart(part1);

            HierarchyPart part2 = new HierarchyPart
            {
                ItemId = "item-002",
                ParentId = "item-001",
                Y = 2,
                X = 1,
                CreatorId = "Alpha",
                UserId = "Alpha"
            };
            repository.AddPart(part2);

            HierarchyPart part3 = new HierarchyPart
            {
                ItemId = "item-003",
                ParentId = "item-001",
                Y = 2,
                X = 2,
                CreatorId = "Alpha",
                UserId = "Alpha"
            };
            repository.AddPart(part3);
        }

        protected void DoGetPartsPage_TypeIdSortExpressions_Ok()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();
            SeedHierarchyParts(repository);

            // act
            var page = repository.GetParts(new PartFilter
            {
                TypeId = "net.fusisoft.hierarchy",
                SortExpressions = new[]
                {
                    Tuple.Create("RoleId", true),
                    Tuple.Create("TypeId", false)
                }
            });

            // assert
            Assert.Equal(3, page.Items.Count);
            Assert.Equal("item-001", page.Items[0].ItemId);
            Assert.Equal("item-002", page.Items[1].ItemId);
            Assert.Equal("item-003", page.Items[2].ItemId);
        }

        protected void DoGetPartsPage_ItemIdTypeId_Ok()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();
            SeedHierarchyParts(repository);

            // act
            var page = repository.GetParts(new PartFilter
            {
                ItemIds = new[] {"item-001"},
                TypeId = "net.fusisoft.hierarchy"
            });

            // assert
            Assert.Equal(1, page.Items.Count);
            Assert.Equal("item-001", page.Items[0].ItemId);
        }

        protected void DoGetItemParts_Item1_2()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            IList<IPart> parts = repository.GetItemParts(new[] {"item-001"});

            Assert.Equal(3, parts.Count);

            CategoriesPart categoriesPart = parts.OfType<CategoriesPart>()
                .FirstOrDefault();
            Assert.NotNull(categoriesPart);
            Assert.Equal(2, categoriesPart.Categories.Count);

            NotePart notePart = parts.OfType<NotePart>().FirstOrDefault();
            Assert.NotNull(notePart);
            Assert.Equal("Some notes.", notePart.Text);

            TokenTextLayerPart<CommentLayerFragment> commentLayerPart =
                parts.OfType<TokenTextLayerPart<CommentLayerFragment>>()
                .FirstOrDefault();
            Assert.NotNull(commentLayerPart);
            Assert.Equal(2, commentLayerPart.Fragments.Count);
        }

        protected void DoGetItemParts_Items1And2_2()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            IList<IPart> parts = repository.GetItemParts(new[]
            {
                "item-001",
                "item-002"
            });

            Assert.Equal(3, parts.Count);

            CategoriesPart categoriesPart = parts.OfType<CategoriesPart>()
                .FirstOrDefault();
            Assert.NotNull(categoriesPart);
            Assert.Equal(2, categoriesPart.Categories.Count);

            NotePart notePart = parts.OfType<NotePart>().FirstOrDefault();
            Assert.NotNull(notePart);
            Assert.Equal("Some notes.", notePart.Text);

            TokenTextLayerPart<CommentLayerFragment> commentLayerPart =
                parts.OfType<TokenTextLayerPart<CommentLayerFragment>>()
                .FirstOrDefault();
            Assert.NotNull(commentLayerPart);
            Assert.Equal(2, commentLayerPart.Fragments.Count);
        }

        protected void DoGetPart_NotExisting_Null()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            var part = repository.GetPart<CategoriesPart>("notexisting");

            Assert.Null(part);
        }

        protected void DoGetPart_Existing_Ok()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            CategoriesPart part = repository.GetPart<CategoriesPart>("part-001");

            Assert.NotNull(part);
            Assert.Equal(2, part.Categories.Count);
            Assert.Contains("alpha", part.Categories);
            Assert.Contains("beta", part.Categories);
        }

        protected void DoGetPartCreatorId_NotExisting_Null()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            string id = repository.GetPartCreatorId("notexisting");

            Assert.Null(id);
        }

        protected void DoGetPartCreatorId_Existing_Ok()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            string id = repository.GetPartCreatorId("part-001");

            Assert.Equal("Odd", id);
        }

        protected void DoGetPartContent_NotExisting_Null()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            string json = repository.GetPartContent("notexisting");

            Assert.Null(json);
        }

        protected void DoGetPartContent_Existing_Ok()
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            string json = repository.GetPartContent("part-001");

            Assert.NotNull(json);
            var part = TestHelper.DeserializePart<CategoriesPart>(json);
            Assert.Equal(2, part.Categories.Count);
            Assert.Contains("alpha", part.Categories);
            Assert.Contains("beta", part.Categories);
        }

        protected void DoAddPart_NotExisting_Added(bool history)
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();
            DateTime now = DateTime.UtcNow;
            NotePart part = new NotePart
            {
                Id = "new",
                ItemId = "item-001",
                CreatorId = "Creator",
                UserId = "Creator",
                Tag = "tag",
                Text = "Some text"
            };

            repository.AddPart(part, history);

            NotePart part2 = repository.GetPart<NotePart>("new");
            Assert.NotNull(part2);

            // history
            var historyParts = repository.GetHistoryParts(
                new HistoryPartFilter
                {
                    ReferenceId = "new",
                    PageNumber = 1,
                    PageSize = 10
                });
            if (history)
            {
                Assert.Equal(1, historyParts.Total);
                var hp = historyParts.Items[0];
                Assert.Equal(EditStatus.Created, hp.Status);
                Assert.Equal("Creator", hp.UserId);
                Assert.Equal("Creator", hp.CreatorId);
                Assert.True((int)(hp.TimeCreated - now).TotalSeconds >= 0);
                Assert.True((int)(hp.TimeModified - now).TotalSeconds >= 0);
            }
            else Assert.Equal(0, historyParts.Total);
        }

        protected void DoAddPart_Existing_Updated(bool history)
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();
            CategoriesPart part = repository.GetPart<CategoriesPart>("part-001");
            part.Categories.Add("new");
            part.UserId = "Updater";
            DateTime now = DateTime.UtcNow;

            repository.AddPart(part, history);

            CategoriesPart part2 = repository.GetPart<CategoriesPart>("part-001");
            Assert.NotNull(part2);
            Assert.Equal(3, part2.Categories.Count);
            Assert.Contains("new", part2.Categories);

            // history
            var historyParts = repository.GetHistoryParts(
                new HistoryPartFilter
                {
                    ReferenceId = "part-001",
                    PageNumber = 1,
                    PageSize = 10
                });
            if (history)
            {
                Assert.Equal(1, historyParts.Total);
                var hp = historyParts.Items[0];
                Assert.Equal(EditStatus.Updated, hp.Status);
                Assert.Equal("Updater", hp.UserId);
                Assert.NotEqual("Updater", hp.CreatorId);
                Assert.True(hp.TimeCreated <= now);
                Assert.True(hp.TimeModified >= now);
            }
            else Assert.Equal(0, historyParts.Total);
        }

        protected void DoAddPartFromContent_NotExisting_Added(bool history)
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();
            DateTime now = DateTime.UtcNow;
            NotePart part = new NotePart
            {
                Id = "new",
                ItemId = "item-001",
                CreatorId = "Creator",
                UserId = "Creator",
                Tag = "tag",
                Text = "Some text"
            };
            string json = TestHelper.SerializePart(part);

            repository.AddPartFromContent(json, history);

            NotePart part2 = repository.GetPart<NotePart>("new");
            Assert.NotNull(part2);

            // history
            var historyParts = repository.GetHistoryParts(
                new HistoryPartFilter
                {
                    ReferenceId = "new",
                    PageNumber = 1,
                    PageSize = 10
                });
            if (history)
            {
                Assert.Equal(1, historyParts.Total);
                var hp = historyParts.Items[0];
                Assert.Equal(EditStatus.Created, hp.Status);
                Assert.Equal("Creator", hp.UserId);
                Assert.Equal("Creator", hp.CreatorId);
                Assert.True((int)(hp.TimeCreated - now).TotalSeconds >= 0);
                Assert.True((int)(hp.TimeModified - now).TotalSeconds >= 0);
            }
            else Assert.Equal(0, historyParts.Total);
        }

        protected void DoAddPartFromContent_Existing_Updated(bool history)
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();
            CategoriesPart part = repository.GetPart<CategoriesPart>("part-001");
            part.Categories.Add("new");
            part.UserId = "Updater";
            string json = TestHelper.SerializePart(part);
            DateTime now = DateTime.UtcNow;

            repository.AddPartFromContent(json, history);

            CategoriesPart part2 = repository.GetPart<CategoriesPart>("part-001");
            Assert.NotNull(part2);
            Assert.Equal(3, part2.Categories.Count);
            Assert.Contains("new", part2.Categories);

            // history
            var historyParts = repository.GetHistoryParts(
                new HistoryPartFilter
                {
                    ReferenceId = "part-001",
                    PageNumber = 1,
                    PageSize = 10
                });
            if (history)
            {
                Assert.Equal(1, historyParts.Total);
                var hp = historyParts.Items[0];
                Assert.Equal(EditStatus.Updated, hp.Status);
                Assert.Equal("Updater", hp.UserId);
                Assert.NotEqual("Updater", hp.CreatorId);
                Assert.True(hp.TimeCreated <= now);
                Assert.True(hp.TimeModified >= now);
            }
            else Assert.Equal(0, historyParts.Total);
        }

        protected void DoDeletePart_NotExisting_Nope(bool history)
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();

            repository.DeletePart("notexisting", "Killer", history);

            // history
            if (history)
            {
                var historyParts = repository.GetHistoryParts(
                    new HistoryPartFilter
                    {
                        ReferenceId = "notexisting",
                        PageNumber = 1,
                        PageSize = 10
                    });
                Assert.Equal(0, historyParts.Total);
            }
        }

        protected void DoDeletePart_Existing_Deleted(bool history)
        {
            PrepareDatabase();
            ICadmusRepository repository = GetRepository();
            DateTime now = DateTime.UtcNow;

            repository.DeletePart("part-001", "Killer", history);

            CategoriesPart part = repository.GetPart<CategoriesPart>("part-001");
            Assert.Null(part);

            // history
            var historyParts = repository.GetHistoryParts(
                new HistoryPartFilter
                {
                    ReferenceId = "part-001",
                    PageNumber = 1,
                    PageSize = 10
                });
            if (history)
            {
                Assert.Equal(1, historyParts.Total);
                Assert.Equal(EditStatus.Deleted, historyParts.Items[0].Status);
                var hp = historyParts.Items[0];
                Assert.Equal("Killer", hp.UserId);
                Assert.NotEqual("Killer", hp.CreatorId);
                Assert.True(hp.TimeCreated <= now);
                Assert.True(hp.TimeModified >= now);
            }
            else Assert.Equal(0, historyParts.Total);
        }
        #endregion

        // TODO: test GetHistoryItems
        // TODO: test GetHistoryItem
        // TODO: test AddHistoryItem
        // TODO: test DeleteHistoryItem
        // TODO: test GetHistoryParts
        // TODO: test GetHistoryPart
        // TODO: test DeleteHistoryPart
    }
}
