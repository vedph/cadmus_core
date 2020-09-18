using Bogus;
using Cadmus.Core;
using Cadmus.Core.Layers;
using Cadmus.Philology.Parts.Layers;
using Fusi.Antiquity.Chronology;
using Fusi.Tools.Config;
using System;
using System.Linq;

namespace Cadmus.Seed.Philology.Parts.Layers
{
    /// <summary>
    /// Seeder for <see cref="ChronologyLayerFragment"/>.
    /// Tag: <c>seed.fr.it.vedph.chronology</c>.
    /// </summary>
    [Tag("seed.fr.it.vedph.chronology")]
    public sealed class ChronologyLayerFragmentSeeder : FragmentSeederBase,
        IConfigurable<ChronologyLayerFragmentSeederOptions>
    {
        private string[] _tags;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ChronologyLayerFragmentSeeder"/> class.
        /// </summary>
        public ChronologyLayerFragmentSeeder()
        {
            _tags = (from n in Enumerable.Range(1, 5)
                     select $"tag-{n}").ToArray();
        }

        /// <summary>
        /// Gets the type of the fragment.
        /// </summary>
        /// <returns>Type.</returns>
        public override Type GetFragmentType() => typeof(ChronologyLayerFragment);

        /// <summary>
        /// Configures the object with the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        public void Configure(ChronologyLayerFragmentSeederOptions options)
        {
            _tags = options.Tags ??
                (from n in Enumerable.Range(1, 5) select $"tag-{n}").ToArray();
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
            return new ChronologyLayerFragment
            {
                Location = location,
                Label = f.Lorem.Sentence(),
                EventId = f.Lorem.Word(),
                Date = new HistoricalDate
                {
                    A = new Datation
                    {
                        Value = Randomizer.Seed.Next(1, 576)
                    }
                },
                Tag = SeedHelper.RandomPickOneOf(_tags)
            };
        }
    }

    /// <summary>
    /// Options for <see cref="ChronologyLayerFragmentSeeder"/>.
    /// </summary>
    public sealed class ChronologyLayerFragmentSeederOptions
    {
        /// <summary>
        /// Gets or sets the optional list of tags to pick from.
        /// </summary>
        public string[] Tags { get; set; }
    }
}
