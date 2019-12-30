using Cadmus.Core;
using Cadmus.Core.Layers;
using Cadmus.Philology.Parts.Layers;
using Fusi.Tools.Config;
using Fusi.Tools.Text;
using System;
using System.Text;

namespace Cadmus.Seed.Philology.Parts.Layers
{
    /// <summary>
    /// Seeder for <see cref="OrthographyLayerFragment"/>.
    /// </summary>
    /// <seealso cref="FragmentSeederBase" />
    /// <seealso cref="IConfigurable{OrthographyLayerFragmentSeederOptions}" />
    [Tag("seed.fr.net.fusisoft.orthography")]
    public sealed class OrthographyLayerFragmentSeeder : FragmentSeederBase,
        IConfigurable<OrthographyLayerFragmentSeederOptions>
    {
        private OrthographyLayerFragmentSeederOptions _options;

        /// <summary>
        /// Gets the type of the fragment.
        /// </summary>
        /// <returns>Type.</returns>
        public override Type GetFragmentType() => typeof(OrthographyLayerFragment);

        /// <summary>
        /// Configures the object with the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        public void Configure(OrthographyLayerFragmentSeederOptions options)
        {
            _options = options;
        }

        private static string ChangeLetterAt(string text, int index)
        {
            StringBuilder sb = new StringBuilder(text);
            char c = (char)(1 + char.ToLowerInvariant(text[index]));
            if (!char.IsLetter(c)) c = 'a';
            sb[index] = c;
            return sb.ToString();
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

            // find 1st letter
            int i = 0;
            while (i < baseText.Length)
            {
                if (char.IsLetter(baseText[i])) break;
                i++;
            }
            if (i == baseText.Length) return null;

            // change it
            string standard = ChangeLetterAt(baseText, i);

            // create operation
            MspOperation op = new MspOperation
            {
                Operator = MspOperator.Replace,
                RangeA = new TextRange(i, 1),
                ValueA = new string(baseText[i], 1),
                ValueB = new string(standard[i], 1),
                Tag = _options?.Tags?.Length > 0
                    ? SeedHelper.RandomPickOneOf(_options.Tags) : null
            };

            OrthographyLayerFragment fragment = new OrthographyLayerFragment
            {
                Location = location,
                Standard = standard
            };
            fragment.Operations.Add(op.ToString());

            return fragment;
        }
    }

    /// <summary>
    /// Options for <see cref="OrthographyLayerFragmentSeeder"/>.
    /// </summary>
    public sealed class OrthographyLayerFragmentSeederOptions
    {
        /// <summary>
        /// The optional tags to randomaly assign to msp operations.
        /// Leave this null to avoid assigning tags.
        /// </summary>
        public string[] Tags { get; set; }
    }
}
