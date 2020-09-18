using Bogus;
using Cadmus.Core;
using Cadmus.Parts.General;
using Fusi.Tools.Config;
using System;
using System.Globalization;

namespace Cadmus.Seed.Parts.General
{
    /// <summary>
    /// Tiled text part seeder.
    /// </summary>
    /// <seealso cref="PartSeederBase" />
    [Tag("seed.it.vedph.tiled-text")]
    public sealed class TiledTextPartSeeder : PartSeederBase
    {
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

            TiledTextPart part = new TiledTextPart();
            SetPartMetadata(part, roleId, item);

            // citation
            part.Citation = Randomizer.Seed.Next(1, 100)
                .ToString(CultureInfo.InvariantCulture);

            // from 2 to 10 rows
            var faker = new Faker();
            string text = faker.Lorem.Sentences(
                Randomizer.Seed.Next(2, 11));
            int y = 1;
            foreach (string line in text.Split('\n'))
            {
                // row
                TextTileRow row = new TextTileRow
                {
                    Y = y++,
                };
                // row's data
                for (int i = 0; i < Randomizer.Seed.Next(0, 4); i++)
                {
                    row.Data[$"d{(char)(i + 97)}"] = faker.Lorem.Word();
                }

                // add tiles in row
                int x = 1;
                foreach (string token in line.Trim().Split(' '))
                {
                    TextTile tile = new TextTile
                    {
                        X = x++,
                    };
                    // text
                    tile.Data[TextTileRow.TEXT_DATA_NAME] = token;
                    // other data
                    for (int i = 0; i < Randomizer.Seed.Next(0, 11); i++)
                    {
                        tile.Data[$"d{(char)(i + 97)}"] = faker.Lorem.Word();
                    }
                    row.Tiles.Add(tile);
                }

                part.Rows.Add(row);
            }

            return part;
        }
    }
}
