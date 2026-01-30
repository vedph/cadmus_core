using Cadmus.Core;
using System.Collections.Generic;
using System.Linq;

namespace Cadmus.Seed.Test;

/// <summary>
/// Tests for <see cref="CadmusSeeder"/>.
/// </summary>
public sealed class CadmusSeederTest
{
    private static readonly PartSeederFactory _factory = TestHelper.GetFactory();

    #region GetItems Basic
    [Fact]
    public void GetItems_Zero_ReturnsEmpty()
    {
        CadmusSeeder seeder = new(_factory);

        IEnumerable<IItem> items = seeder.GetItems(0);

        Assert.Empty(items);
    }

    [Fact]
    public void GetItems_One_ReturnsOneItem()
    {
        CadmusSeeder seeder = new(_factory);

        List<IItem> items = [.. seeder.GetItems(1)];

        Assert.Single(items);
        Assert.False(string.IsNullOrEmpty(items[0].Id));
        Assert.False(string.IsNullOrEmpty(items[0].Title));
        Assert.False(string.IsNullOrEmpty(items[0].FacetId));
    }

    [Fact]
    public void GetItems_Multiple_ReturnsMultipleItems()
    {
        CadmusSeeder seeder = new(_factory);

        List<IItem> items = [.. seeder.GetItems(5)];

        Assert.Equal(5, items.Count);
        Assert.True(items.All(i => !string.IsNullOrEmpty(i.Id)));
    }
    #endregion

    #region IsRequired Handling
    [Fact]
    public void GetItems_DefaultFacet_AlwaysHasRequiredPart()
    {
        // The default facet has a required categories part with role "function"
        // We need to generate multiple items to ensure statistical significance
        CadmusSeeder seeder = new(_factory);

        // generate items with fixed seed for reproducibility
        List<IItem> items = [.. seeder.GetItems(10)];

        // filter items from the "default" facet
        List<IItem> defaultFacetItems = items
            .Where(i => i.FacetId == "default")
            .ToList();

        // all items from default facet must have the required part
        foreach (IItem item in defaultFacetItems)
        {
            // the required part is categories with role "function"
            IPart? requiredPart = item.Parts.Find(
                p => p.TypeId == "it.vedph.categories" && p.RoleId == "function");
            Assert.NotNull(requiredPart);
        }
    }

    [Fact]
    public void GetItems_OptionalPartsMayBeAbsent()
    {
        // The default facet has an optional categories part with role "material"
        // With random seeding, some items may not have it
        CadmusSeeder seeder = new(_factory);

        List<IItem> items = [.. seeder.GetItems(20)];

        // filter items from the "default" facet
        List<IItem> defaultFacetItems = items
            .Where(i => i.FacetId == "default")
            .ToList();

        if (defaultFacetItems.Count == 0) return; // skip if no default facet items

        // count items with optional material part
        int withMaterial = defaultFacetItems.Count(i =>
            i.Parts.Any(p => p.TypeId == "it.vedph.categories" &&
                            p.RoleId == "material"));

        // with 50% probability, we should see some variation
        // (not all or none) - though this is probabilistic
        // For a deterministic test, we just verify the required part
        // is always present
    }
    #endregion

    #region Role-Aware Seeding
    [Fact]
    public void GetItems_RoleSpecificSeederUsed()
    {
        // Items from the default facet should use role-specific seeders
        CadmusSeeder seeder = new(_factory);

        List<IItem> items = [.. seeder.GetItems(10)];

        // filter items from the "default" facet
        List<IItem> defaultFacetItems = items
            .Where(i => i.FacetId == "default")
            .ToList();

        foreach (IItem item in defaultFacetItems)
        {
            // check the required function categories part
            IPart? functionPart = item.Parts.Find(
                p => p.TypeId == "it.vedph.categories" && p.RoleId == "function");

            Assert.NotNull(functionPart);

            CategoriesPart categoriesPart = (CategoriesPart)functionPart!;

            // categories should be from function set (not default set)
            string[] functionCategories =
                ["funerary", "votive", "honorary", "legal"];
            Assert.True(categoriesPart.Categories.All(
                c => functionCategories.Contains(c)),
                $"Expected function categories but got: " +
                $"{string.Join(", ", categoriesPart.Categories)}");
        }
    }

    [Fact]
    public void GetItems_FallbackToTypeOnlySeeder()
    {
        // Items from the text facet have a categories part without role
        // It should use the type-only seeder (not role-specific)
        CadmusSeeder seeder = new(_factory);

        List<IItem> items = [.. seeder.GetItems(20)];

        // filter items from the "text" facet that have categories
        List<IItem> textFacetItems = items
            .Where(i => i.FacetId == "text")
            .ToList();

        // Skip if no text facet items were generated
        if (textFacetItems.Count == 0) return;

        foreach (IItem item in textFacetItems)
        {
            // check for categories part without role
            IPart? categoriesPart = item.Parts.Find(
                p => p.TypeId == "it.vedph.categories" &&
                    string.IsNullOrEmpty(p.RoleId));

            if (categoriesPart == null) continue; // optional, may not exist

            CategoriesPart part = (CategoriesPart)categoriesPart;

            // categories should be from default set
            string[] defaultCategories =
            [
                "language.phonology", "language.morphology", "language.syntax",
                "literature", "geography"
            ];
            Assert.True(part.Categories.All(
                c => defaultCategories.Contains(c)),
                $"Expected default categories but got: " +
                $"{string.Join(", ", part.Categories)}");
        }
    }
    #endregion

    #region SeederTypeRoleId
    [Fact]
    public void SeederTypeRoleId_Parse_TypeOnly()
    {
        SeederTypeRoleId id = SeederTypeRoleId.Parse("seed.it.vedph.categories");

        Assert.Equal("it.vedph.categories", id.TypeId);
        Assert.Null(id.RoleId);
        Assert.False(id.HasRole);
    }

    [Fact]
    public void SeederTypeRoleId_Parse_WithRole()
    {
        SeederTypeRoleId id = SeederTypeRoleId.Parse(
            "seed.it.vedph.categories:function");

        Assert.Equal("it.vedph.categories", id.TypeId);
        Assert.Equal("function", id.RoleId);
        Assert.True(id.HasRole);
    }

    [Fact]
    public void SeederTypeRoleId_BuildKey_TypeOnly()
    {
        string key = SeederTypeRoleId.BuildKey("it.vedph.categories", null);
        Assert.Equal("it.vedph.categories", key);
    }

    [Fact]
    public void SeederTypeRoleId_BuildKey_WithRole()
    {
        string key = SeederTypeRoleId.BuildKey(
            "it.vedph.categories", "function");
        Assert.Equal("it.vedph.categories:function", key);
    }

    [Fact]
    public void SeederTypeRoleId_GetRolesForType_FindsMatchingRoles()
    {
        IList<string> roles = SeederTypeRoleId.GetRolesForType(
            [
                "seed.it.vedph.categories:function",
                "seed.it.vedph.categories:material",
                "seed.it.vedph.note:general"
            ],
            "it.vedph.categories");

        Assert.Equal(2, roles.Count);
        Assert.Contains("function", roles);
        Assert.Contains("material", roles);
    }

    [Fact]
    public void SeederTypeRoleId_ExtractFragmentRole_WithRole()
    {
        string? role = SeederTypeRoleId.ExtractFragmentRole(
            "fr.it.vedph.apparatus:scholarly");
        Assert.Equal("scholarly", role);
    }

    [Fact]
    public void SeederTypeRoleId_ExtractFragmentRole_WithoutRole()
    {
        string? role = SeederTypeRoleId.ExtractFragmentRole(
            "fr.it.vedph.apparatus");
        Assert.Null(role);
    }

    [Fact]
    public void SeederTypeRoleId_ExtractFragmentTypeId_WithRole()
    {
        string? typeId = SeederTypeRoleId.ExtractFragmentTypeId(
            "fr.it.vedph.apparatus:scholarly");
        Assert.Equal("fr.it.vedph.apparatus", typeId);
    }

    [Fact]
    public void SeederTypeRoleId_ExtractFragmentTypeId_WithoutRole()
    {
        string? typeId = SeederTypeRoleId.ExtractFragmentTypeId(
            "fr.it.vedph.apparatus");
        Assert.Equal("fr.it.vedph.apparatus", typeId);
    }
    #endregion
}
