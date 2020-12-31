using Bogus;
using Cadmus.Parts;
using System.Collections.Generic;

namespace Cadmus.Seed.Parts
{
    internal static class SeederHelper
    {
        /// <summary>
        /// Gets a random number of document references.
        /// </summary>
        /// <param name="min">The min number of references to get.</param>
        /// <param name="max">The max number of references to get.</param>
        /// <returns>References.</returns>
        public static List<DocReference> GetDocReferences(int min, int max)
        {
            List<DocReference> refs = new List<DocReference>();

            for (int n = 1; n <= Randomizer.Seed.Next(min, max + 1); n++)
            {
                refs.Add(new Faker<DocReference>()
                    .RuleFor(r => r.Tag, f => f.PickRandom(null, "tag"))
                    .RuleFor(r => r.Author, f => f.Lorem.Word())
                    .RuleFor(r => r.Work, f => f.Lorem.Word())
                    .RuleFor(r => r.Location,
                        f => $"{f.Random.Number(1, 24)}.{f.Random.Number(1, 1000)}")
                    .RuleFor(r => r.Note, f => f.Lorem.Sentence())
                    .Generate());
            }

            return refs;
        }

        /// <summary>
        /// Gets a list of external IDs.
        /// </summary>
        /// <param name="min">The min number of IDs to get.</param>
        /// <param name="max">The max number of IDs to get.</param>
        /// <returns>IDs.</returns>
        public static List<string> GetExternalIds(int min, int max)
        {
            List<string> ids = new List<string>();
            Faker faker = new Faker();

            for (int n = 1; n <= Randomizer.Seed.Next(min, max + 1); n++)
            {
                ids.Add(faker.Random.Bool()
                    ? faker.Hacker.Abbreviation()
                    : faker.Internet.Url());
            }

            return ids;
        }
    }
}
