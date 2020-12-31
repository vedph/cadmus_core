﻿using Bogus;
using Cadmus.Core;
using Cadmus.Core.Layers;
using Cadmus.Parts.General;
using Cadmus.Parts.Layers;
using Cadmus.Seed.Parts.General;
using Fusi.Tools.Config;
using System;
using System.Collections.Generic;

namespace Cadmus.Seed.Parts.Layers
{
    /// <summary>
    /// Seeder for <see cref="CommentLayerFragment"/>'s.
    /// Tag: <c>seed.fr.it.vedph.comment</c>.
    /// </summary>
    /// <seealso cref="FragmentSeederBase" />
    /// <seealso cref="IConfigurable{CommentLayerFragmentSeederOptions}" />
    [Tag("seed.fr.it.vedph.comment")]
    public sealed class CommentLayerFragmentSeeder : FragmentSeederBase,
        IConfigurable<CommentPartSeederOptions>
    {
        private CommentPartSeederOptions _options;

        /// <summary>
        /// Gets the type of the fragment.
        /// </summary>
        /// <returns>Type.</returns>
        public override Type GetFragmentType() => typeof(CommentLayerFragment);

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
        /// <param name="location">The location.</param>
        /// <param name="baseText">The base text.</param>
        /// <returns>A new fragment.</returns>
        /// <exception cref="ArgumentNullException">item or location or
        /// baseText</exception>
        public override ITextLayerFragment GetFragment(
            IItem item, string location, string baseText)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (location == null)
                throw new ArgumentNullException(nameof(location));
            if (baseText == null)
                throw new ArgumentNullException(nameof(baseText));

            return new Faker<CommentLayerFragment>()
                .RuleFor(fr => fr.Location, location)
                .RuleFor(p => p.Tag, f => f.PickRandom("scholarly", "general"))
                .RuleFor(p => p.Text, f => f.Lorem.Sentence())
                .RuleFor(p => p.References, SeederHelper.GetDocReferences(1, 3))
                .RuleFor(p => p.ExternalIds, SeederHelper.GetExternalIds(1, 3))
                .RuleFor(p => p.Categories, f => new List<string>(
                    new[] { f.PickRandom(_options.Categories) }))
                .RuleFor(p => p.Keywords, f => GetKeywords(f.Random.Number(1, 3)))
                .Generate();
        }
    }
}
