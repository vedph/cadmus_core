using Bogus;
using Cadmus.Core;
using Fusi.Tools.Configuration;
using System;
using System.Collections.Generic;

namespace Cadmus.Seed.Test;

/// <summary>
/// Note part seeder.
/// Tag: <c>seed.it.vedph.note</c>.
/// </summary>
/// <seealso cref="PartSeederBase" />
[Tag("seed.it.vedph.note")]
public sealed class NotePartSeeder : PartSeederBase,
    IConfigurable<NotePartSeederOptions>
{
    private NotePartSeederOptions? _options;

    /// <summary>
    /// Configures the object with the specified options.
    /// </summary>
    /// <param name="options">The options.</param>
    public void Configure(NotePartSeederOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// Creates and seeds a new part.
    /// </summary>
    /// <param name="item">The item this part should belong to.</param>
    /// <param name="roleId">The optional part role ID.</param>
    /// <param name="factory">The part seeder factory. This is used
    /// for layer parts, which need to seed a set of fragments.</param>
    /// <returns>A new part.</returns>
    /// <exception cref="ArgumentNullException">item or factory</exception>
    public override IPart? GetPart(IItem item, string? roleId,
        PartSeederFactory? factory)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(factory);

        NotePart part = new Faker<NotePart>()
            .RuleFor(p => p.Tag,
                f => _options?.Tags?.Count > 0
                ? f.PickRandom(_options.Tags) : null)
            .RuleFor(p => p.Text, f => f.Lorem.Sentences())
            .Generate();

        SetPartMetadata(part, roleId, item);

        return part;
    }
}

/// <summary>
/// Options for <see cref="NotePartSeeder"/>.
/// </summary>
public sealed class NotePartSeederOptions
{
    /// <summary>
    /// Gets or sets the tags to pick from. If not set, tags will always
    /// be null.
    /// </summary>
    public IList<string>? Tags { get; set; }
}
