using Bogus;
using Cadmus.Core;
using Cadmus.Parts.General;
using Fusi.Tools.Config;
using System;
using System.Collections.Generic;

namespace Cadmus.Seed.Parts.General
{
    /// <summary>
    /// Comment part seeder.
    /// Tag: <c>seed.it.vedph.comment</c>.
    /// </summary>
    /// <seealso cref="PartSeederBase" />
    [Tag("seed.it.vedph.comment")]
    public sealed class CommentPartSeeder : PartSeederBase,
        IConfigurable<CommentPartSeederOptions>
    {
        private CommentPartSeederOptions _options;

        /// <summary>
        /// Configures the object with the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        public void Configure(CommentPartSeederOptions options)
        {
            _options = options ?? new CommentPartSeederOptions();

            if (_options.Languages == null || _options.Languages.Length == 0)
            {
                _options.Languages = new[]
                {
                    "eng", "deu", "ita", "fra", "spa"
                };
            }
            if (_options.Categories == null || _options.Categories.Length == 0)
            {
                _options.Categories = new[]
                {
                    "alpha", "beta", "gamma"
                };
            }
        }

        private List<IndexKeyword> GetKeywords(int count)
        {
            List<IndexKeyword> keywords = new List<IndexKeyword>();

            for (int n = 1; n <= count; n++)
            {
                keywords.Add(new Faker<IndexKeyword>()
                    .RuleFor(k => k.IndexId, f => f.PickRandom(null, "ixa", "ixb"))
                    .RuleFor(k => k.Language, f => f.PickRandom(_options.Languages))
                    .RuleFor(k => k.Value, f => f.Lorem.Word())
                    .Generate());
            }

            return keywords;
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

            CommentPart part = new Faker<CommentPart>()
                .RuleFor(p => p.Tag, f => f.PickRandom("scholarly", "general"))
                .RuleFor(p => p.Text, f => f.Lorem.Sentence())
                .RuleFor(p => p.References, SeederHelper.GetDocReferences(1, 3))
                .RuleFor(p => p.ExternalIds, SeederHelper.GetExternalIds(1, 3))
                .RuleFor(p => p.Categories, f => new List<string>(
                    new[] { f.PickRandom(_options.Categories) }))
                .RuleFor(p => p.Keywords, f => GetKeywords(f.Random.Number(1, 3)))
                .Generate();
            SetPartMetadata(part, roleId, item);

            return part;
        }
    }

    /// <summary>
    /// Options for <see cref="CommentPartSeeder"/>.
    /// </summary>
    public sealed class CommentPartSeederOptions
    {
        /// <summary>
        /// Gets or sets the categories to pick from.
        /// </summary>
        public string[] Categories { get; set; }

        /// <summary>
        /// Gets or sets the languages codes to pick from.
        /// </summary>
        public string[] Languages { get; set; }
    }
}
