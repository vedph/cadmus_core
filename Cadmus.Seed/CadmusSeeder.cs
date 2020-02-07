using Bogus;
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
        private readonly SeedOptions _options;
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
            _options = _factory.GetSeedOptions();
        }

        private IPart GetPart(IItem item, PartDefinition definition)
        {
            if (!_partSeeders.ContainsKey(definition.TypeId)) return null;

            return _partSeeders[definition.TypeId]
                ?.GetPart(item, definition.RoleId, _factory);
        }

        private static bool IsLayerPart(PartDefinition def) =>
            def.RoleId?.StartsWith(PartBase.FR_PREFIX) == true;

        private void AddParts(IEnumerable<PartDefinition> partDefinitions,
            IItem item, bool optional)
        {
            foreach (PartDefinition partDef in partDefinitions)
            {
                if (optional && Randomizer.Seed.Next(0, 2) == 0) continue;

                IPart part = GetPart(item, partDef);
                if (part != null) item.Parts.Add(part);
            }
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
            if (_options.Seed.HasValue)
                Randomizer.Seed = new Random(_options.Seed.Value);

            ItemSeeder itemSeeder = _factory.GetItemSeeder();
            _partSeeders = _factory.GetPartSeeders();
            IItemSortKeyBuilder sortKeyBuilder = _factory.GetItemSortKeyBuilder();

            // generate items
            for (int n = 1; n <= count; n++)
            {
                // pick a facet
                FacetDefinition facet = _options.FacetDefinitions[
                    _options.FacetDefinitions.Length == 1
                    ? 0
                    : Randomizer.Seed.Next(0, _options.FacetDefinitions.Length)];

                // get item
                IItem item = itemSeeder.GetItem(n, facet.Id);

                // add parts: first non layer parts, then the others.
                // This ensures that the item already has the base text part
                // before adding the layer parts, which refer to it.

                // 1) non-layer parts, required
                AddParts(facet.PartDefinitions
                    .Where(def => !IsLayerPart(def) && def.IsRequired),
                    item,
                    false);

                // 2) non-layer parts, optional
                AddParts(facet.PartDefinitions
                    .Where(def => !IsLayerPart(def) && !def.IsRequired),
                    item,
                    true);

                // 3) layer-parts
                // we must have a base text definition to have layers
                PartDefinition baseTextDef = facet.PartDefinitions.Find(
                    def => def.RoleId == PartBase.BASE_TEXT_ROLE_ID);

                if (baseTextDef != null && Randomizer.Seed.Next(0, 2) == 1)
                {
                    // ensure there is a base text. This is required for
                    // the text layer part seeder, which must rely on a base text.
                    IPart baseTextPart = item.Parts.Find(
                        p => p.TypeId == baseTextDef.TypeId);

                    // add a base text if none found
                    if (baseTextPart == null)
                    {
                        baseTextPart = GetPart(item, baseTextDef);
                        if (baseTextPart != null) item.Parts.Add(baseTextPart);
                    }

                    // once we have one, eventually add layer(s)
                    if (baseTextPart != null)
                    {
                        // 3) layer parts, required
                        AddParts(facet.PartDefinitions
                            .Where(def => IsLayerPart(def) && def.IsRequired),
                            item,
                            false);

                        // 4) layer parts, optional
                        AddParts(facet.PartDefinitions
                            .Where(def => IsLayerPart(def) && !def.IsRequired),
                            item,
                            true);
                    }
                }

                // once all the parts are in place, override the item's sort key
                // if requested in the config.
                // Note that we do not provide a repository, as while seeding
                // there might be no database, and all the item's parts are
                // in the item itself.
                if (sortKeyBuilder != null)
                {
                    item.SortKey = sortKeyBuilder.BuildKey(item, null);
                }

                yield return item;
            }
        }
    }
}
