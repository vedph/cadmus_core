using Cadmus.Core;
using Fusi.Tools.Configuration;
using System.Collections.Generic;
using System.Reflection;

namespace Cadmus.Seed.Test;

/// <summary>
/// Tests for <see cref="NotePartSeeder"/> demonstrating legacy (non-role)
/// seeding still works correctly.
/// </summary>
public sealed class NotePartSeederTest
{
    private static readonly PartSeederFactory _factory = TestHelper.GetFactory();

    #region Infrastructure
    [Fact]
    public void TypeHasTagAttribute()
    {
        TagAttribute? attr = typeof(NotePartSeeder).GetTypeInfo()
            .GetCustomAttribute<TagAttribute>();
        Assert.NotNull(attr);
        Assert.Equal("seed.it.vedph.note", attr!.Tag);
    }
    #endregion

    #region GetPart
    [Fact]
    public void GetPart_NoOptions_ReturnsPartWithTextAndNoTag()
    {
        // legacy seeding: seeder with no options configured
        NotePartSeeder seeder = new();
        seeder.SetSeedOptions(_factory.GetSeedOptions());

        IItem item = TestHelper.GetItem();
        IPart? part = seeder.GetPart(item, null, _factory);

        Assert.NotNull(part);
        TestHelper.AssertPartMetadata(part!);
        Assert.IsType<NotePart>(part);

        NotePart notePart = (NotePart)part!;
        // Text should be generated even without options
        Assert.False(string.IsNullOrEmpty(notePart.Text));
        // Tag should be null since no Tags options were provided
        Assert.Null(notePart.Tag);
    }

    [Fact]
    public void GetPart_FromFactory_ReturnsPartWithText()
    {
        // get seeder from factory (legacy non-role lookup)
        Dictionary<string, IPartSeeder> seeders = _factory.GetPartSeeders();
        Assert.True(seeders.ContainsKey("it.vedph.note"));

        IPartSeeder seeder = seeders["it.vedph.note"];

        IItem item = TestHelper.GetItem();
        IPart? part = seeder.GetPart(item, null, _factory);

        Assert.NotNull(part);
        TestHelper.AssertPartMetadata(part!);
        Assert.IsType<NotePart>(part);

        NotePart notePart = (NotePart)part!;
        Assert.False(string.IsNullOrEmpty(notePart.Text));
    }

    [Fact]
    public void GetPart_WithRole_SetsRoleId()
    {
        // legacy seeding should still support role assignment
        Dictionary<string, IPartSeeder> seeders = _factory.GetPartSeeders();
        IPartSeeder seeder = seeders["it.vedph.note"];

        IItem item = TestHelper.GetItem();
        IPart? part = seeder.GetPart(item, "scholarly", _factory);

        Assert.NotNull(part);
        Assert.Equal("scholarly", part!.RoleId);
        Assert.IsType<NotePart>(part);
    }
    #endregion

    #region Factory Integration
    [Fact]
    public void GetPartSeeders_ContainsNoteSeeder()
    {
        Dictionary<string, IPartSeeder> seeders = _factory.GetPartSeeders();

        // should have type-only seeder (no role)
        Assert.True(seeders.ContainsKey("it.vedph.note"));
    }

    [Fact]
    public void GetPartSeeders_NoteSeederIsCorrectType()
    {
        Dictionary<string, IPartSeeder> seeders = _factory.GetPartSeeders();
        IPartSeeder seeder = seeders["it.vedph.note"];

        Assert.IsType<NotePartSeeder>(seeder);
    }
    #endregion
}
