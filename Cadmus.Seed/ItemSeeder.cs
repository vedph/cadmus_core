using Bogus;
using Cadmus.Core;
using Cadmus.Core.Config;
using System;

namespace Cadmus.Seed
{
    /// <summary>
    /// Cadmus item seeder.
    /// </summary>
    public sealed class ItemSeeder
    {
        private readonly SeedOptions _options;
        private readonly IItemSortKeyBuilder _sortKeyBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemSeeder"/> class.
        /// </summary>
        /// <param name="options">The seed options.</param>
        /// <exception cref="ArgumentNullException">options</exception>
        public ItemSeeder(SeedOptions options)
        {
            _options = options
                ?? throw new ArgumentNullException(nameof(options));
            _sortKeyBuilder = new StandardItemSortKeyBuilder();
        }

        /// <summary>
        /// Create an item and seed it with random data.
        /// </summary>
        /// <param name="number">The item number. This is just the ordinal
        /// number of the requested item, and can be used by the seeder
        /// to populate item's data.</param>
        /// <param name="facet">The item's facet.</param>
        /// <returns>A new item.</returns>
        /// <exception cref="ArgumentNullException">facet</exception>
        public IItem GetItem(int number, FacetDefinition facet)
        {
            if (facet == null) throw new ArgumentNullException(nameof(facet));

            string oddEven = (number & 1) == 1 ? "odd" : "even";

            Item item = new Faker<Item>()
                .RuleFor(i => i.Title, $"Item #{number}")
                .RuleFor(i => i.Description,
                    $"Description for {oddEven} " +
                    $"item number {NumberToWords.Convert(number)}.")
                .RuleFor(i => i.FacetId, facet.Id)
                .RuleFor(i => i.CreatorId, f => f.PickRandom(_options.Users))
                .RuleFor(i => i.UserId, f => f.PickRandom(_options.Users))
                .RuleFor(i => i.Flags, (number & 1) == 1 ? 1 : 0);

            // use a standard sort key; you can override it later,
            // e.g. when parts have been added to the item.
            item.SortKey = _sortKeyBuilder.BuildKey(item, null);

            return item;
        }
    }
}
