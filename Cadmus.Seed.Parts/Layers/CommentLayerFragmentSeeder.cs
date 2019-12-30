using Bogus;
using Cadmus.Core;
using Cadmus.Core.Layers;
using Cadmus.Parts.Layers;
using Fusi.Tools.Config;
using System;

namespace Cadmus.Seed.Parts.Layers
{
    /// <summary>
    /// Seeder for <see cref="CommentLayerFragment"/>'s.
    /// Tag: <c>seed.fr.net.fusisoft.comment</c>.
    /// </summary>
    /// <seealso cref="FragmentSeederBase" />
    /// <seealso cref="IConfigurable{CommentLayerFragmentSeederOptions}" />
    [Tag("seed.fr.net.fusisoft.comment")]
    public sealed class CommentLayerFragmentSeeder : FragmentSeederBase,
        IConfigurable<CommentLayerFragmentSeederOptions>
    {
        private CommentLayerFragmentSeederOptions _options;

        /// <summary>
        /// Gets the type of the fragment.
        /// </summary>
        /// <returns>Type.</returns>
        public override Type GetFragmentType() => typeof(CommentLayerFragment);

        /// <summary>
        /// Configures the object with the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        public void Configure(CommentLayerFragmentSeederOptions options)
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

            return new Faker<CommentLayerFragment>()
                .RuleFor(fr => fr.Location, location)
                .RuleFor(fr => fr.Text, f => f.Lorem.Sentences())
                .RuleFor(fr => fr.Tag,
                    f => _options.Tags?.Length > 0
                    ? f.PickRandom(_options.Tags) : null)
                .Generate();
        }
    }

    /// <summary>
    /// Options for <see cref="CommentLayerFragmentSeeder"/>.
    /// </summary>
    public sealed class CommentLayerFragmentSeederOptions
    {
        /// <summary>
        /// Gets or sets the optional tags to pick for a comment.
        /// </summary>
        public string[] Tags { get; set; }
    }
}
