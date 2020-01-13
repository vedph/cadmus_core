using Bogus;
using Cadmus.Core;
using Cadmus.Core.Layers;
using Cadmus.Philology.Parts.Layers;
using Fusi.Tools.Config;
using System;
using System.Collections.Generic;

namespace Cadmus.Seed.Philology.Parts.Layers
{
    /// <summary>
    /// Seeder for <see cref="ApparatusLayerFragment"/>'s.
    /// Tag: <c>seed.fr.net.fusisoft.apparatus</c>.
    /// </summary>
    /// <seealso cref="FragmentSeederBase" />
    /// <seealso cref="IConfigurable{ApparatusLayerFragmentSeederOptions}" />
    [Tag("seed.fr.net.fusisoft.apparatus")]
    public sealed class ApparatusLayerFragmentSeeder : FragmentSeederBase,
        IConfigurable<ApparatusLayerFragmentSeederOptions>
    {
        private ApparatusLayerFragmentSeederOptions _options;

        /// <summary>
        /// Gets the type of the fragment.
        /// </summary>
        /// <returns>Type.</returns>
        public override Type GetFragmentType() => typeof(ApparatusLayerFragment);

        /// <summary>
        /// Configures the object with the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        public void Configure(ApparatusLayerFragmentSeederOptions options)
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

            if (_options == null || _options.Authors.Length == 0) return null;

            HashSet<string> authors = new HashSet<string>(
                SeedHelper.RandomPickOf(
                _options.Authors,
                Randomizer.Seed.Next(1, 4)));

            LemmaVariantType type = (LemmaVariantType)Randomizer.Seed.Next(0, 4);
            switch (type)
            {
                case LemmaVariantType.Note:
                    return new Faker<ApparatusLayerFragment>()
                        .RuleFor(fr => fr.Type, type)
                        .RuleFor(fr => fr.Location, location)
                        .RuleFor(fr => fr.Authors, authors)
                        .RuleFor(fr => fr.Note, f => f.Lorem.Sentence())
                        .Generate();
                default:
                    return new Faker<ApparatusLayerFragment>()
                        .RuleFor(fr => fr.Type, type)
                        .RuleFor(fr => fr.Location, location)
                        .RuleFor(fr => fr.Authors, authors)
                        .RuleFor(fr => fr.Value, f => f.Lorem.Word())
                        .Generate();
            }
        }
    }

    /// <summary>
    /// Options for <see cref="ApparatusLayerFragmentSeeder"/>.
    /// </summary>
    public sealed class ApparatusLayerFragmentSeederOptions
    {
        /// <summary>
        /// Gets or sets the authors to pick from.
        /// </summary>
        public string[] Authors { get; set; }
    }
}
