using Cadmus.Core;
using Cadmus.General.Parts;
using Fusi.Tools.Data;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace Cadmus.Index.Sql.Test;

public abstract class SqlItemIndexReaderTestBase : SqlItemIndexWriterTestBase
{
    protected SqlItemIndexReaderTestBase(string connectionString, bool legacy)
        : base(connectionString, legacy)
    {
    }

    protected abstract IItemIndexReader GetReader();

    protected void DoSearchItems_ByTitleNoOp_Ok()
    {
        IItemIndexWriter writer = GetWriter();
        writer.Clear();

        List<IItem> items = new();
        for (int n = 1; n <= 3; n++)
        {
            IItem item = GetItem(n);
            IPart part = new NotePart
            {
                ItemId = item.Id,
                CreatorId = "creator",
                RoleId = null,
                Tag = (n & 1) == 1 ? "odd" : "even",
                Text = $"A note for item {n}.",
                UserId = "user"
            };
            item.Parts.Add(part);

            writer.WriteItem(item);
            items.Add(item);
        }
        IItemIndexReader reader = GetReader();

        DataPage<ItemInfo> page = reader.SearchItems("Item #1", new PagingOptions());

        Assert.Equal(1, page.Total);
        Assert.Single(page.Items);
        Assert.Equal(items[0].Id, page.Items[0].Id);
        Assert.Equal(items[0].Title, page.Items[0].Title);
        Assert.Equal(items[0].Description, page.Items[0].Description);
        Assert.Equal(items[0].FacetId, page.Items[0].FacetId);
        Assert.Equal(items[0].GroupId, page.Items[0].GroupId);
        Assert.Equal(items[0].SortKey, page.Items[0].SortKey);
        Assert.Equal(items[0].UserId, page.Items[0].UserId);
        Assert.Equal(items[0].CreatorId, page.Items[0].CreatorId);
        Assert.Equal(items[0].Flags, page.Items[0].Flags);
    }

    protected void DoSearchItems_ByTitleContains_Ok()
    {
        IItemIndexWriter writer = GetWriter();
        writer.Clear();

        List<IItem> items = new();
        for (int n = 1; n <= 3; n++)
        {
            IItem item = GetItem(n);
            IPart part = new NotePart
            {
                ItemId = item.Id,
                CreatorId = "creator",
                RoleId = null,
                Tag = (n & 1) == 1 ? "odd" : "even",
                Text = $"A note for item {n}.",
                UserId = "user"
            };
            item.Parts.Add(part);

            writer.WriteItem(item);
            items.Add(item);
        }
        IItemIndexReader reader = GetReader();

        DataPage<ItemInfo> page = reader.SearchItems("[title*=1]",
            new PagingOptions());

        Assert.Equal(1, page.Total);
        Assert.Single(page.Items);
        Assert.Equal(items[0].Id, page.Items[0].Id);
        Assert.Equal(items[0].Title, page.Items[0].Title);
        Assert.Equal(items[0].Description, page.Items[0].Description);
        Assert.Equal(items[0].FacetId, page.Items[0].FacetId);
        Assert.Equal(items[0].GroupId, page.Items[0].GroupId);
        Assert.Equal(items[0].SortKey, page.Items[0].SortKey);
        Assert.Equal(items[0].UserId, page.Items[0].UserId);
        Assert.Equal(items[0].CreatorId, page.Items[0].CreatorId);
        Assert.Equal(items[0].Flags, page.Items[0].Flags);
    }

    protected void DoSearchItems_ByTagStarts_Ok()
    {
        IItemIndexWriter writer = GetWriter();
        writer.Clear();

        List<IItem> items = new();
        for (int n = 1; n <= 3; n++)
        {
            IItem item = GetItem(n);
            IPart part = new NotePart
            {
                ItemId = item.Id,
                CreatorId = "creator",
                RoleId = null,
                Tag = (n & 1) == 1 ? "odd" : "even",
                Text = $"A note for item {n}.",
                UserId = "user"
            };
            item.Parts.Add(part);

            writer.WriteItem(item);
            items.Add(item);
        }
        IItemIndexReader reader = GetReader();

        DataPage<ItemInfo> page = reader.SearchItems("[tag^=e]",
            new PagingOptions());

        Assert.Equal(1, page.Total);
        Assert.Single(page.Items);
        Assert.Equal(items[1].Id, page.Items[0].Id);
        Assert.Equal(items[1].Title, page.Items[0].Title);
        Assert.Equal(items[1].Description, page.Items[0].Description);
        Assert.Equal(items[1].FacetId, page.Items[0].FacetId);
        Assert.Equal(items[1].GroupId, page.Items[0].GroupId);
        Assert.Equal(items[1].SortKey, page.Items[0].SortKey);
        Assert.Equal(items[1].UserId, page.Items[0].UserId);
        Assert.Equal(items[1].CreatorId, page.Items[0].CreatorId);
        Assert.Equal(items[1].Flags, page.Items[0].Flags);
    }

    protected void DoSearchItems_ByTitleRegex_Ok()
    {
        IItemIndexWriter writer = GetWriter();
        writer.Clear();

        List<IItem> items = new();
        for (int n = 1; n <= 3; n++)
        {
            IItem item = GetItem(n);
            IPart part = new NotePart
            {
                ItemId = item.Id,
                CreatorId = "creator",
                RoleId = null,
                Tag = (n & 1) == 1 ? "odd" : "even",
                Text = $"A note for item {n}.",
                UserId = "user"
            };
            item.Parts.Add(part);

            writer.WriteItem(item);
            items.Add(item);
        }
        IItemIndexReader reader = GetReader();

        DataPage<ItemInfo> page = reader.SearchItems("[title~=\\[01\\]]",
            new PagingOptions());

        Assert.Equal(1, page.Total);
        Assert.Single(page.Items);
        Assert.Equal(items[0].Id, page.Items[0].Id);
        Assert.Equal(items[0].Title, page.Items[0].Title);
        Assert.Equal(items[0].Description, page.Items[0].Description);
        Assert.Equal(items[0].FacetId, page.Items[0].FacetId);
        Assert.Equal(items[0].GroupId, page.Items[0].GroupId);
        Assert.Equal(items[0].SortKey, page.Items[0].SortKey);
        Assert.Equal(items[0].UserId, page.Items[0].UserId);
        Assert.Equal(items[0].CreatorId, page.Items[0].CreatorId);
        Assert.Equal(items[0].Flags, page.Items[0].Flags);
    }

    protected void DoSearchItems_ByTitleFuzzy_Ok(float treshold, int expected)
    {
        IItemIndexWriter writer = GetWriter();
        writer.Clear();

        List<IItem> items = new();
        for (int n = 1; n <= 3; n++)
        {
            IItem item = GetItem(n);
            IPart part = new NotePart
            {
                ItemId = item.Id,
                CreatorId = "creator",
                RoleId = null,
                Tag = (n & 1) == 1 ? "odd" : "even",
                Text = $"A note for item {n}.",
                UserId = "user"
            };
            item.Parts.Add(part);

            writer.WriteItem(item);
            items.Add(item);
        }
        IItemIndexReader reader = GetReader();

        DataPage<ItemInfo> page = reader.SearchItems(
            $"[title%=itm #1:{treshold.ToString(CultureInfo.InvariantCulture)}]",
            new PagingOptions());

        Assert.Equal(expected, page.Total);
        Assert.Equal(expected > page.PageSize? page.PageSize : expected,
            page.Items.Count);
    }

    protected void DoSearchItems_ByDscContains_Ok()
    {
        IItemIndexWriter writer = GetWriter();
        writer.Clear();

        List<IItem> items = new();
        for (int n = 1; n <= 3; n++)
        {
            IItem item = GetItem(n);
            IPart part = new NotePart
            {
                ItemId = item.Id,
                CreatorId = "creator",
                RoleId = null,
                Tag = (n & 1) == 1 ? "odd" : "even",
                Text = $"A note for item {n}.",
                UserId = "user"
            };
            item.Parts.Add(part);

            writer.WriteItem(item);
            items.Add(item);
        }
        IItemIndexReader reader = GetReader();

        DataPage<ItemInfo> page = reader.SearchItems("[dsc*=1]",
            new PagingOptions());

        Assert.Equal(1, page.Total);
        Assert.Single(page.Items);
        Assert.Equal(items[0].Id, page.Items[0].Id);
        Assert.Equal(items[0].Title, page.Items[0].Title);
        Assert.Equal(items[0].Description, page.Items[0].Description);
        Assert.Equal(items[0].FacetId, page.Items[0].FacetId);
        Assert.Equal(items[0].GroupId, page.Items[0].GroupId);
        Assert.Equal(items[0].SortKey, page.Items[0].SortKey);
        Assert.Equal(items[0].UserId, page.Items[0].UserId);
        Assert.Equal(items[0].CreatorId, page.Items[0].CreatorId);
        Assert.Equal(items[0].Flags, page.Items[0].Flags);
    }

    protected void DoSearchPins_Equals_Ok()
    {
        IItemIndexWriter writer = GetWriter();
        writer.Clear();

        List<IItem> items = new();
        for (int n = 1; n <= 3; n++)
        {
            IItem item = GetItem(n);
            IPart part = new NotePart
            {
                ItemId = item.Id,
                CreatorId = "creator",
                RoleId = null,
                Tag = (n & 1) == 1 ? "odd" : "even",
                Text = $"A note for item {n}.",
                UserId = "user"
            };
            item.Parts.Add(part);

            writer.WriteItem(item);
            items.Add(item);
        }
        IItemIndexReader reader = GetReader();

        DataPage<DataPinInfo> page = reader.SearchPins("[tag=odd]",
            new PagingOptions());

        Assert.Equal(2, page.Total);
        Assert.Equal(2, page.Items.Count);
    }
    // TODO other tests
}
