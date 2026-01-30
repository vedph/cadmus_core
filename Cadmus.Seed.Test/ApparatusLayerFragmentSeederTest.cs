using Cadmus.Core;
using Cadmus.Core.Layers;
using Fusi.Tools.Configuration;
using System.Reflection;

namespace Cadmus.Seed.Test;

/// <summary>
/// Tests for <see cref="ApparatusLayerFragmentSeeder"/>.
/// </summary>
public sealed class ApparatusLayerFragmentSeederTest
{
    private static readonly PartSeederFactory _factory = TestHelper.GetFactory();
    private static readonly SeedOptions _options = _factory.GetSeedOptions();
    private static readonly IItem _item = TestHelper.GetItem();

    #region Infrastructure
    [Fact]
    public void TypeHasTagAttribute()
    {
        TagAttribute? attr = typeof(ApparatusLayerFragmentSeeder).GetTypeInfo()
            .GetCustomAttribute<TagAttribute>();
        Assert.NotNull(attr);
        Assert.Equal("seed.fr.it.vedph.apparatus", attr!.Tag);
    }

    [Fact]
    public void GetFragmentType_ReturnsApparatusLayerFragment()
    {
        ApparatusLayerFragmentSeeder seeder = new();
        Assert.Equal(typeof(ApparatusLayerFragment), seeder.GetFragmentType());
    }
    #endregion

    #region GetFragment
    [Fact]
    public void GetFragment_NoOptions_ReturnsFragment()
    {
        ApparatusLayerFragmentSeeder seeder = new();
        seeder.SetSeedOptions(_options);

        ITextLayerFragment? fragment = seeder.GetFragment(_item, "1.1", "alpha");

        Assert.NotNull(fragment);
        Assert.IsType<ApparatusLayerFragment>(fragment);
        Assert.Equal("1.1", fragment!.Location);

        ApparatusLayerFragment apparatus = (ApparatusLayerFragment)fragment;
        Assert.NotEmpty(apparatus.Entries);
    }

    [Fact]
    public void GetFragment_WithOptions_ReturnsFragmentWithConfiguredAuthors()
    {
        // get seeder from factory to ensure it's configured
        IFragmentSeeder? seeder = _factory.GetFragmentSeeder(
            "seed.fr.it.vedph.apparatus");
        Assert.NotNull(seeder);

        ITextLayerFragment? fragment = seeder!.GetFragment(_item, "1.2", "beta");

        Assert.NotNull(fragment);
        ApparatusLayerFragment apparatus = (ApparatusLayerFragment)fragment!;
        Assert.NotEmpty(apparatus.Entries);
    }
    #endregion

    #region Role-Aware Seeding
    [Fact]
    public void GetFragmentSeeder_WithRole_ReturnsSeeder()
    {
        // should find role-specific seeder
        IFragmentSeeder? seeder = _factory.GetFragmentSeeder(
            "seed.fr.it.vedph.apparatus:scholarly");

        Assert.NotNull(seeder);
    }

    [Fact]
    public void GetFragmentSeeder_WithRole_FallbackToTypeOnly()
    {
        // should fall back to type-only seeder if role-specific not found
        IFragmentSeeder? seeder = _factory.GetFragmentSeeder(
            "seed.fr.it.vedph.apparatus:nonexistent");

        Assert.NotNull(seeder);
    }

    [Fact]
    public void GetFragmentSeeder_WithRoleOverload_ReturnsSeeder()
    {
        // test the convenience overload
        IFragmentSeeder? seeder = _factory.GetFragmentSeeder(
            "seed.fr.it.vedph.apparatus", "scholarly");

        Assert.NotNull(seeder);
    }

    [Fact]
    public void GetFragment_ScholarlyRole_ReturnsFragment()
    {
        IFragmentSeeder? seeder = _factory.GetFragmentSeeder(
            "seed.fr.it.vedph.apparatus:scholarly");
        Assert.NotNull(seeder);

        ITextLayerFragment? fragment = seeder!.GetFragment(
            _item, "2.1", "gamma");

        Assert.NotNull(fragment);
        ApparatusLayerFragment apparatus = (ApparatusLayerFragment)fragment!;
        Assert.Equal("2.1", apparatus.Location);
        Assert.NotEmpty(apparatus.Entries);
    }
    #endregion
}
