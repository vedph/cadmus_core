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
        private readonly IItemSortKeyBuilder _itemSortKeyBuilder;
        private SeedOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemSeeder"/> class.
        /// </summary>
        /// <param name="itemSortKeyBuilder">The item sort key builder.</param>
        /// <param name="options">The seed options.</param>
        /// <exception cref="ArgumentNullException">itemSortKeyBuilder or
        /// options</exception>
        public ItemSeeder(IItemSortKeyBuilder itemSortKeyBuilder,
            SeedOptions options)
        {
            _itemSortKeyBuilder = itemSortKeyBuilder
                ?? throw new ArgumentNullException(nameof(itemSortKeyBuilder));
            _options = options
                ?? throw new ArgumentNullException(nameof(options));
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

            return item;
        }
    }
}
