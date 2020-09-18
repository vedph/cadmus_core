using Bogus;
using Cadmus.Core;
using Cadmus.Parts.General;
using Fusi.Tools.Config;
using System;

namespace Cadmus.Seed.Parts.General
{
    /// <summary>
    /// Note part seeder.
    /// Tag: <c>seed.it.vedph.note</c>.
    /// </summary>
    /// <seealso cref="PartSeederBase" />
    [Tag("seed.it.vedph.note")]
    public sealed class NotePartSeeder : PartSeederBase,
        IConfigurable<NotePartSeederOptions>
    {
        private NotePartSeederOptions _options;

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
        public override IPart GetPart(IItem item, string roleId,
            PartSeederFactory factory)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            NotePart part = new Faker<NotePart>()
                .RuleFor(p => p.Tag,
                    f => _options?.Tags.Length > 0
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
        public string[] Tags { get; set; }
    }
}
