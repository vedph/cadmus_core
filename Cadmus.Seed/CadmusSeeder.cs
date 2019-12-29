using Cadmus.Core;
using Cadmus.Core.Config;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cadmus.Seed
{
    /// <summary>
    /// Cadmus fake data seeder. This uses a <see cref="PartSeederFactory"/>
    /// to seed parts and fragments in each item, according to a specific
    /// configuration.
    /// </summary>
    public sealed class CadmusSeeder
    {
        private readonly PartSeederFactory _factory;
        private readonly IItemSortKeyBuilder _itemSortKeyBuilder;
        private readonly SeedOptions _options;
        private Random _random;
        private Dictionary<string, IPartSeeder> _partSeeders;

        /// <summary>
        /// Initializes a new instance of the <see cref="CadmusSeeder"/> class.
        /// </summary>
        /// <param name="factory">The part seeder factory.</param>
        /// <exception cref="ArgumentNullException">factory</exception>
        public CadmusSeeder(PartSeederFactory factory)
        {
            _factory = factory
                ?? throw new ArgumentNullException(nameof(factory));

            _itemSortKeyBuilder = _factory.GetItemSortKeyBuilder();
            _options = _factory.GetSeedOptions();
        }

        private IPart GetPart(IItem item, PartDefinition definition)
        {
            if (!_partSeeders.ContainsKey(definition.TypeId)) return null;
            return _partSeeders[definition.TypeId].GetPart(item, _factory);
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <param name="count">The desired count of items to get.</param>
        /// <returns>The items.</returns>
        public IEnumerable<IItem> GetItems(int count)
        {
            if (count < 1
                || _options.FacetDefinitions == null
                || _options.FacetDefinitions.Length == 0)
            {
                yield break;
            }

            // init
            _random = _options.Seed.HasValue ?
                new Random(_options.Seed.Value) : new Random();
            ItemSeeder itemSeeder = _factory.GetItemSeeder();
            _partSeeders = _factory.GetPartSeeders();

            // generate items
            for (int n = 1; n <= count; n++)
            {
                // pick a facet
                FacetDefinition facet = _options.FacetDefinitions[
                    _random.Next(0, _options.FacetDefinitions.Length)];

                IItem item = itemSeeder.GetItem(n, facet);

                // generate its required parts
                foreach (PartDefinition partDef in facet.PartDefinitions
                    .Where(p => p.IsRequired))
                {
                    IPart part = GetPart(item, partDef);
                    if (part != null) item.Parts.Add(part);
                }

                // eventually generate its optional parts
                foreach (PartDefinition partDef in facet.PartDefinitions
                    .Where(p => !p.IsRequired))
                {
                    if (_random.Next(0, 2) == 0) continue;
                    IPart part = GetPart(item, partDef);
                    if (part != null) item.Parts.Add(part);
                }

                yield return item;
            }
        }
    }
}
