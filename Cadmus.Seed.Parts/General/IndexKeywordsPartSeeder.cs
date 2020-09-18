using Bogus;
using Cadmus.Core;
using Cadmus.Parts.General;
using Fusi.Tools.Config;
using System;

namespace Cadmus.Seed.Parts.General
{
    /// <summary>
    /// Seeder for <see cref="IndexKeywordsPart"/>.
    /// Tag: <c>seed.it.vedph.index-keywords</c>.
    /// </summary>
    /// <seealso cref="PartSeederBase" />
    /// <seealso cref="IConfigurable{IndexKeywordsPartSeederOptions}" />
    [Tag("seed.it.vedph.index-keywords")]
    public sealed class IndexKeywordsPartSeeder : PartSeederBase,
        IConfigurable<IndexKeywordsPartSeederOptions>
    {
        private IndexKeywordsPartSeederOptions _options;

        /// <summary>
        /// Configures the object with the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        public void Configure(IndexKeywordsPartSeederOptions options)
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

            if (_options?.Languages == null || _options.Languages.Length == 0)
                return null;

            IndexKeywordsPart part = new IndexKeywordsPart();
            SetPartMetadata(part, roleId, item);

            int count = Randomizer.Seed.Next(1, 4);
            bool hasIndexIds = _options.IndexIds?.Length > 0;
            while (count > 0)
            {
                IndexKeyword keyword = new Faker<IndexKeyword>()
                    .RuleFor(k => k.IndexId, f => hasIndexIds?
                        f.PickRandom(_options.IndexIds) : null)
                    .RuleFor(k => k.Language, f => f.PickRandom(_options.Languages))
                    .RuleFor(k => k.Value, f => f.Lorem.Word())
                    .Generate();

                part.AddKeyword(keyword);
                count--;
            }

            return part;
        }
    }

    /// <summary>
    /// Options for <see cref="KeywordsPartSeeder"/>.
    /// </summary>
    public sealed class IndexKeywordsPartSeederOptions
    {
        /// <summary>
        /// Gets or sets the index IDs to pick from. You can use an empty
        /// string for the default index.
        /// </summary>
        public string[] IndexIds { get; set; }

        /// <summary>
        /// Gets or sets the languages codes to pick from.
        /// </summary>
        public string[] Languages { get; set; }
    }
}
