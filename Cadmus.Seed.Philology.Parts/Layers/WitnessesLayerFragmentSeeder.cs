using Bogus;
using Cadmus.Core;
using Cadmus.Core.Layers;
using Cadmus.Philology.Parts.Layers;
using Fusi.Tools.Config;
using System;

namespace Cadmus.Seed.Philology.Parts.Layers
{
    /// <summary>
    /// Seeder for <see cref="WitnessesLayerFragment"/>.
    /// Tag: <c>seed.fr.net.fusisoft.witnesses</c>.
    /// </summary>
    [Tag("seed.fr.net.fusisoft.witnesses")]
    public sealed class WitnessesLayerFragmentSeeder :
        FragmentSeederBase,
        IConfigurable<WitnessesLayerFragmentSeederOptions>
    {
        private WitnessesLayerFragmentSeederOptions _options;

        /// <summary>
        /// Gets the type of the fragment.
        /// </summary>
        /// <returns>Type.</returns>
        public override Type GetFragmentType() => typeof(WitnessesLayerFragment);

        /// <summary>
        /// Configures the object with the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        public void Configure(WitnessesLayerFragmentSeederOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Creates and seeds a new part.
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

            if (_options?.Ids == null || _options.Ids.Length == 0)
                return null;

            WitnessesLayerFragment fragment = new WitnessesLayerFragment
            {
                Location = location
            };

            int count = Randomizer.Seed.Next(1, 4);
            while (count > 0)
            {
                fragment.Witnesses.Add(new Faker<Witness>()
                    .RuleFor(w => w.Id, f => f.PickRandom(_options.Ids))
                    .RuleFor(w => w.Citation, f => $"p.{f.Random.Int(1, 101)}")
                    .RuleFor(w => w.Text, f=> f.Lorem.Sentence())
                    .RuleFor(w => w.Note,
                        f => f.Random.Bool()? f.Lorem.Sentence() : null)
                    .Generate());
                count--;
            }

            return fragment;
        }
    }

    /// <summary>
    /// Options for <see cref="WitnessesLayerFragmentSeeder"/>.
    /// </summary>
    public sealed class WitnessesLayerFragmentSeederOptions
    {
        /// <summary>
        /// Gets or sets the source IDs to pick from.
        /// </summary>
        public string[] Ids { get; set; }
    }
}
