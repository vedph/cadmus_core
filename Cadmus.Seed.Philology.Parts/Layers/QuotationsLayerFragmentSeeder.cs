using Bogus;
using Cadmus.Core;
using Cadmus.Core.Layers;
using Cadmus.Philology.Parts.Layers;
using Fusi.Tools.Config;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Cadmus.Seed.Philology.Parts.Layers
{
    /// <summary>
    /// Seeder for <see cref="QuotationsLayerFragment"/>.
    /// Tag: <c>seed.fr.it.vedph.quotations</c>.
    /// </summary>
    /// <seealso cref="FragmentSeederBase" />
    /// <seealso cref="IConfigurable{QuotationLayerFragmentSeederOptions}" />
    [Tag("seed.fr.it.vedph.quotations")]
    public sealed class QuotationsLayerFragmentSeeder : FragmentSeederBase,
        IConfigurable<QuotationLayerFragmentSeederOptions>
    {
        private readonly List<int> _authorNumbers;
        private string[] _authors;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="QuotationsLayerFragmentSeeder"/> class.
        /// </summary>
        public QuotationsLayerFragmentSeeder()
        {
            _authorNumbers = Enumerable.Range(1, 10).ToList();
            _authors = (from n in _authorNumbers
                        select $"author{n}").ToArray();
        }

        /// <summary>
        /// Gets the type of the fragment.
        /// </summary>
        /// <returns>Type.</returns>
        public override Type GetFragmentType() => typeof(QuotationsLayerFragment);

        /// <summary>
        /// Configures the object with the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        public void Configure(QuotationLayerFragmentSeederOptions options)
        {
            _authors = options.Authors ??
                (from n in _authorNumbers select $"author{n}").ToArray();
        }

        /// <summary>
        /// Creates and seeds a new fragment.
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

            Faker f = new Faker();
            QuotationsLayerFragment fr = new QuotationsLayerFragment
            {
                Location = location
            };
            int n = Randomizer.Seed.Next(1, 4);
            for (int i = 1; i <= n; i++)
            {
                string author = SeedHelper.RandomPickOneOf(_authors);
                string work = f.Lorem.Word();

                fr.Entries.Add(new QuotationEntry
                {
                    Author = author,
                    Work = work,
                    Citation = f.Random.Int(1, 100).ToString(CultureInfo.InvariantCulture),
                    CitationUri = $"urn:{author.ToLowerInvariant()}/{work.ToLowerInvariant()}",
                    Variant = Randomizer.Seed.Next(1, 5) == 1 ?
                        f.Lorem.Sentence() : null,
                    Tag = Randomizer.Seed.Next(1, 5) == 1 ?
                        f.Lorem.Word() : null,
                    Note = Randomizer.Seed.Next(1, 5) == 1 ?
                        f.Lorem.Sentence() : null
                });
            }

            return fr;
        }
    }

    /// <summary>
    /// Options for <see cref="QuotationsLayerFragmentSeeder"/>.
    /// </summary>
    public sealed class QuotationLayerFragmentSeederOptions
    {
        /// <summary>
        /// Gets or sets the authors to pick from. If not specified, names
        /// like "author1", "author2", etc. will be used.
        /// </summary>
        public string[] Authors { get; set; }
    }
}
