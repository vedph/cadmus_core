using Cadmus.Core;
using Fusi.Tools.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cadmus.Seed.Test;

/// <summary>
/// Tests for <see cref="CategoriesPartSeeder"/>.
/// </summary>
public sealed class CategoriesPartSeederTest
{
    private static readonly PartSeederFactory _factory = TestHelper.GetFactory();

    #region Infrastructure
    [Fact]
    public void TypeHasTagAttribute()
    {
        TagAttribute? attr = typeof(CategoriesPartSeeder).GetTypeInfo()
            .GetCustomAttribute<TagAttribute>();
        Assert.NotNull(attr);
        Assert.Equal("seed.it.vedph.categories", attr!.Tag);
    }
    #endregion

    #region GetPart
    [Fact]
    public void GetPart_NoOptions_ReturnsNull()
    {
        CategoriesPartSeeder seeder = new();
        seeder.SetSeedOptions(_factory.GetSeedOptions());
        // no configuration means no options

        IItem item = TestHelper.GetItem();
        IPart? part = seeder.GetPart(item, null, _factory);

        Assert.Null(part);
    }

    [Fact]
    public void GetPart_WithOptions_ReturnsPart()
    {
        // get seeder from factory to ensure it's configured
        Dictionary<string, IPartSeeder> seeders = _factory.GetPartSeeders();
        IPartSeeder seeder = seeders["it.vedph.categories"];

        IItem item = TestHelper.GetItem();
        IPart? part = seeder.GetPart(item, null, _factory);

        Assert.NotNull(part);
        TestHelper.AssertPartMetadata(part!);
        Assert.IsType<CategoriesPart>(part);

        CategoriesPart categoriesPart = (CategoriesPart)part!;
        Assert.NotEmpty(categoriesPart.Categories);
        Assert.True(categoriesPart.Categories.Count <= 3);
    }
    #endregion

    #region Role-Aware Seeding
    [Fact]
    public void GetPartSeeders_ContainsRoleSpecificSeeders()
    {
        Dictionary<string, IPartSeeder> seeders = _factory.GetPartSeeders();

        // should have type-only and role-specific seeders
        Assert.True(seeders.ContainsKey("it.vedph.categories"));
        Assert.True(seeders.ContainsKey("it.vedph.categories:function"));
        Assert.True(seeders.ContainsKey("it.vedph.categories:material"));
    }

    [Fact]
    public void GetPart_FunctionRole_ReturnsPartWithFunctionCategories()
    {
        Dictionary<string, IPartSeeder> seeders = _factory.GetPartSeeders();
        IPartSeeder seeder = seeders["it.vedph.categories:function"];

        IItem item = TestHelper.GetItem();
        IPart? part = seeder.GetPart(item, "function", _factory);

        Assert.NotNull(part);
        TestHelper.AssertPartMetadata(part!);
        Assert.Equal("function", part!.RoleId);

        CategoriesPart categoriesPart = (CategoriesPart)part;
        Assert.NotEmpty(categoriesPart.Categories);
        Assert.True(categoriesPart.Categories.Count <= 2);

        // categories should be from function set
        string[] functionCategories = ["funerary", "votive", "honorary", "legal"];
        Assert.True(categoriesPart.Categories.All(
            c => functionCategories.Contains(c)));
    }

    [Fact]
    public void GetPart_MaterialRole_ReturnsPartWithMaterialCategories()
    {
        Dictionary<string, IPartSeeder> seeders = _factory.GetPartSeeders();
        IPartSeeder seeder = seeders["it.vedph.categories:material"];

        IItem item = TestHelper.GetItem();
        IPart? part = seeder.GetPart(item, "material", _factory);

        Assert.NotNull(part);
        TestHelper.AssertPartMetadata(part!);
        Assert.Equal("material", part!.RoleId);

        CategoriesPart categoriesPart = (CategoriesPart)part;
        Assert.NotEmpty(categoriesPart.Categories);
        Assert.True(categoriesPart.Categories.Count <= 2);

        // categories should be from material set
        string[] materialCategories = ["limestone", "marble", "granite", "bronze"];
        Assert.True(categoriesPart.Categories.All(
            c => materialCategories.Contains(c)));
    }

    [Fact]
    public void GetPart_NoRole_ReturnsPartWithDefaultCategories()
    {
        Dictionary<string, IPartSeeder> seeders = _factory.GetPartSeeders();
        IPartSeeder seeder = seeders["it.vedph.categories"];

        IItem item = TestHelper.GetItem();
        IPart? part = seeder.GetPart(item, null, _factory);

        Assert.NotNull(part);
        CategoriesPart categoriesPart = (CategoriesPart)part!;

        // categories should be from default set
        string[] defaultCategories =
        [
            "language.phonology", "language.morphology", "language.syntax",
            "literature", "geography"
        ];
        Assert.True(categoriesPart.Categories.All(
            c => defaultCategories.Contains(c)));
    }
    #endregion
}
