using Bogus;
using Cadmus.Core;
using Cadmus.Core.Layers;
using Cadmus.Philology.Parts.Layers;
using Fusi.Tools.Config;
using System;
using System.Collections.Generic;
using System.Linq;

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
        private readonly List<int> _witAndAuthorNumbers;
        private string[] _authors;
        private string[] _witnesses;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApparatusLayerFragmentSeeder"/>
        /// class.
        /// </summary>
        public ApparatusLayerFragmentSeeder()
        {
            _witAndAuthorNumbers = new List<int>(Enumerable.Range(1, 10));
            _authors = (from n in _witAndAuthorNumbers
                        select $"author{n}").ToArray();
            _witnesses = (from n in _witAndAuthorNumbers
                          select $"w{n}").ToArray();
        }

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
            _authors = options.Authors ??
                (from n in _witAndAuthorNumbers
                 select $"author{n}").ToArray();
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

            Faker f = new Faker();
            ApparatusLayerFragment fr = new ApparatusLayerFragment
            {
                Location = location,
                Tag = f.Lorem.Word()
            };
            int n = Randomizer.Seed.Next(1, 4);
            for (int i = 1; i <= n; i++)
            {
                ApparatusEntry entry = new ApparatusEntry
                {
                    Type = (ApparatusEntryType)Randomizer.Seed.Next(0, 4)
                };
                if (Randomizer.Seed.Next(1, 6) == 1) entry.Tag = f.Lorem.Word();

                // note
                if (entry.Type == ApparatusEntryType.Note)
                {
                    entry.Note = f.Lorem.Sentence();
                    // authors
                    foreach (string author in SeedHelper.RandomPickOf(_authors, 2))
                    {
                        entry.Authors.Add(
                            new ApparatusAnnotatedValue { Value = author });
                    }
                }
                // variant
                else
                {
                    entry.Value = f.Lorem.Word();
                    entry.NormValue = entry.Value.ToUpperInvariant();
                    entry.IsAccepted = i == 1;

                    // witnesses
                    foreach (string witness in SeedHelper.RandomPickOf(_witnesses, 2))
                    {
                        entry.Authors.Add(
                            new ApparatusAnnotatedValue { Value = witness });
                    }

                    // authors
                    foreach (string author in SeedHelper.RandomPickOf(_authors, 1))
                    {
                        entry.Authors.Add(
                            new ApparatusAnnotatedValue { Value = author });
                    }
                }

                fr.Entries.Add(entry);
            }

            return fr;
        }
    }

    /// <summary>
    /// Options for <see cref="ApparatusLayerFragmentSeeder"/>.
    /// </summary>
    public sealed class ApparatusLayerFragmentSeederOptions
    {
        /// <summary>
        /// Gets or sets the authors to pick from. If not specified, names
        /// like "author1", "author2", etc. will be used.
        /// </summary>
        public string[] Authors { get; set; }
    }
}
