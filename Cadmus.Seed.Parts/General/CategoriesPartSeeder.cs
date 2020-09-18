﻿using Bogus;
using Cadmus.Core;
using Cadmus.Parts.General;
using Fusi.Tools.Config;
using System;

namespace Cadmus.Seed.Parts.General
{
    /// <summary>
    /// Categories part seeder.
    /// Tag: <c>seed.it.vedph.categories</c>.
    /// </summary>
    /// <seealso cref="Cadmus.Seed.PartSeederBase" />
    [Tag("seed.it.vedph.categories")]
    public sealed class CategoriesPartSeeder : PartSeederBase,
        IConfigurable<CategoriesPartSeederOptions>
    {
        private CategoriesPartSeederOptions _options;

        /// <summary>
        /// Configures the object with the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        public void Configure(CategoriesPartSeederOptions options)
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

            if (_options?.Categories == null
                || _options.Categories.Length == 0
                || _options.MaxCategoriesPerItem < 1)
            {
                return null;
            }

            CategoriesPart part = new CategoriesPart();
            SetPartMetadata(part, roleId, item);

            // pick from 1 to 3 categories, all different
            int count = Randomizer.Seed.Next(1, _options.MaxCategoriesPerItem + 1);
            foreach (string category in SeedHelper.RandomPickOf(
                _options.Categories, count))
            {
                part.Categories.Add(category);
            }

            return part;
        }
    }

    /// <summary>
    /// Options for <see cref="CategoriesPartSeeder"/>.
    /// </summary>
    public sealed class CategoriesPartSeederOptions
    {
        /// <summary>
        /// Gets or sets the maximum categories per item.
        /// </summary>
        public int MaxCategoriesPerItem { get; set; }

        /// <summary>
        /// Gets or sets the categories to pick from.
        /// </summary>
        public string[] Categories { get; set; }
    }
}
