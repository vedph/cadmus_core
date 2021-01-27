using Bogus;
using Cadmus.Core;
using Cadmus.Parts.General;
using Fusi.Tools.Config;
using System;

namespace Cadmus.Seed.Parts.General
{
    /// <summary>
    /// Part seeder for <see cref="ExtBibliographyPart"/>.
    /// Tag: <c>seed.it.vedph.ext-bibliography</c>.
    /// </summary>
    /// <seealso cref="PartSeederBase" />
    [Tag("seed.it.vedph.ext-bibliography")]
    public sealed class ExtBibliographyPartSeeder : PartSeederBase,
        IConfigurable<ExtBibliographyPartSeederOptions>
    {
        private ExtBibEntry[] _entries;

        /// <summary>
        /// Configures the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        public void Configure(ExtBibliographyPartSeederOptions options)
        {
            _entries = options?.Entries;
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
        public override IPart GetPart(IItem item, string roleId,
            PartSeederFactory factory)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            ExtBibliographyPart part = new ExtBibliographyPart();
            SetPartMetadata(part, roleId, item);

            if (_entries?.Length > 0)
            {
                part.Entries.AddRange(_entries);
            }
            else
            {
                for (int n = 1; n <= Randomizer.Seed.Next(1, 3 + 1); n++)
                {
                    part.Entries.Add(new Faker<ExtBibEntry>()
                        .RuleFor(e => e.Id, Guid.NewGuid().ToString())
                        .RuleFor(e => e.Label, f => f.Lorem.Sentence(5))
                        .RuleFor(e => e.Tag, f => f.Lorem.Word())
                        .RuleFor(e => e.Note, f => f.Random.Bool(0.25f) ?
                            f.Lorem.Sentence() : null)
                        .Generate());
                }
            }

            return part;
        }
    }

    /// <summary>
    /// Options for <see cref="ExtBibliographyPartSeeder"/>.
    /// </summary>
    public class ExtBibliographyPartSeederOptions
    {
        /// <summary>
        /// Gets or sets the entries.
        /// </summary>
        public ExtBibEntry[] Entries { get; set; }
    }
}
