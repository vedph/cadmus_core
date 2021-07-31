using Bogus;
using Cadmus.Bricks;
using Cadmus.Core;
using Cadmus.Parts.General;
using Fusi.Tools.Config;
using System;
using System.Collections.Generic;

namespace Cadmus.Seed.Parts.General
{
    /// <summary>
    /// Seeder for <see cref="ExternalIdsPart"/>.
    /// Tag: <c>seed.it.vedph.external-ids</c>.
    /// </summary>
    /// <seealso cref="PartSeederBase" />
    [Tag("seed.it.vedph.external-ids")]
    public sealed class ExternalIdsPartSeeder : PartSeederBase
    {
        private List<ExternalId> GetIds(int min, int max)
        {
            List<ExternalId> ids = new List<ExternalId>();
            for (int n = 1; n <= Randomizer.Seed.Next(min, max + 1); n++)
            {
                bool uri = Randomizer.Seed.Next(0, 2) == 0;

                ids.Add(new Faker<ExternalId>()
                    .RuleFor(i => i.Value, f => uri
                        ? f.Internet.Url()
                        : Guid.NewGuid().ToString())
                    .RuleFor(i => i.Type, uri? "uri" : "guid")
                    .RuleFor(i => i.Tag, f => f.Random.Bool(0.25F)? "test" : null)
                    .RuleFor(i => i.Note, f => f.Random.Bool(0.25F)
                        ? f.Lorem.Sentence() : null)
                    );
            }
            return ids;
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

            ExternalIdsPart part = new Faker<ExternalIdsPart>()
               .RuleFor(p => p.Ids, GetIds(1, 3))
               .Generate();
            SetPartMetadata(part, roleId, item);

            return part;
        }
    }
}
