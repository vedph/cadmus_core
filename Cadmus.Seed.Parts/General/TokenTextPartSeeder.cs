using Bogus;
using Cadmus.Core;
using Cadmus.Parts.General;
using Fusi.Tools.Config;
using System;

namespace Cadmus.Seed.Parts.General
{
    /// <summary>
    /// Token-based text part, to be used as the base text for token-based
    /// text layers.
    /// Tag: <c>seed.net.fusisoft.token-text</c>.
    /// </summary>
    [Tag("seed.net.fusisoft.token-text")]
    public sealed class TokenTextPartSeeder : PartSeederBase
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

            TokenTextPart part = new TokenTextPart();
            SetPartMetadata(part, roleId, item);

            // from 2 to 10 lines
            string text = new Faker().Lorem.Sentences(
                Randomizer.Seed.Next(2, 11));
            int y = 1;
            foreach (string line in text.Split('\n'))
            {
                part.Lines.Add(new TextLine
                {
                    Y = y++,
                    Text = line.Trim()
                });
            }

            return part;
        }
    }
}
