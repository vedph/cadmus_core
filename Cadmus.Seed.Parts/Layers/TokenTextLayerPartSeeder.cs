using Bogus;
using Cadmus.Core;
using Cadmus.Core.Layers;
using Cadmus.Parts.General;
using Cadmus.Parts.Layers;
using Fusi.Tools.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cadmus.Seed.Parts.Layers
{
    /// <summary>
    /// Seeder for <see cref="TokenTextLayerPart{TFragment}"/>.
    /// Tag: <c>seed.net.fusisoft.token-text-layer</c>.
    /// </summary>
    /// <seealso cref="Cadmus.Seed.PartSeederBase" />
    [Tag("seed.net.fusisoft.token-text-layer")]
    public sealed class TokenTextLayerPartSeeder : PartSeederBase,
        IConfigurable<TokenTextLayerPartSeederOptions>
    {
        private TokenTextLayerPartSeederOptions _options;

        /// <summary>
        /// Configures the object with the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        public void Configure(TokenTextLayerPartSeederOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Pick (if possible) the specified count of random locations in the
        /// base text part, each with its corresponding piece of text.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <param name="count">The count.</param>
        /// <returns>A list of tuples where 1=location, 2=base text.</returns>
        private IList<Tuple<string, string>> PickLocAndTexts(
            TokenTextPart part, int count)
        {
            HashSet<Tuple<int, int>> usedCoords = new HashSet<Tuple<int, int>>();
            Dictionary<int, string[]> tokens = new Dictionary<int, string[]>();
            List<Tuple<string, string>> results = new List<Tuple<string, string>>();

            while (count > 0)
            {
                for (int attempt = 0; attempt < 10; attempt++)
                {
                    // pick a line
                    int y = Randomizer.Seed.Next(1, part.Lines.Count + 1);

                    // pick a token in that line
                    if (!tokens.ContainsKey(y))
                        tokens[y] = part.Lines[y - 1].GetTokens();
                    int x = Randomizer.Seed.Next(1, tokens[y].Length + 1);

                    // select if not already used
                    Tuple<int, int> yx = Tuple.Create(y, x);
                    if (!usedCoords.Contains(yx))
                    {
                        usedCoords.Add(yx);
                        results.Add(Tuple.Create($"{y}.{x}", tokens[y][x - 1]));
                        break;
                    }
                }
                count--;
            }
            return results;
        }

        private static string StripColonSuffix(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            int i = text.LastIndexOf(':');
            return i > -1 ? text.Substring(0, i) : text;
        }

        /// <summary>
        /// Creates and seeds a new part with its fragments. The fragment
        /// type comes from the part's role ID.
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

            if (_options == null || _options.MaxFragmentCount < 1) return null;

            // get the base text part; nothing to do if none
            TokenTextPart textPart = item.Parts
                .OfType<TokenTextPart>()
                .FirstOrDefault();
            if (textPart == null) return null;

            // get the seeder; nothing to do if none
            string frTypeId = StripColonSuffix(roleId);
            IFragmentSeeder seeder =
                factory.GetFragmentSeeder("seed." + frTypeId);
            if (seeder == null) return null;

            // get the layer part for the specified fragment type
            Type constructedType = typeof(TokenTextLayerPart<>)
                .MakeGenericType(seeder.GetFragmentType());
            IPart part = (IPart)Activator.CreateInstance(constructedType);

            // seed metadata
            SetPartMetadata(part, frTypeId, item);

            // seed by adding fragments
            int count = Randomizer.Seed.Next(1, _options.MaxFragmentCount);
            IList<Tuple<string, string>> locAndTexts =
                PickLocAndTexts(textPart, count);

            // must invoke AddFragment via reflection, as the closed type
            // is known only at runtime
            Type t = part.GetType();

            foreach (var lt in locAndTexts)
            {
                ITextLayerFragment fr = seeder.GetFragment(
                    item, lt.Item1, lt.Item2);
                if (fr != null)
                {
                    t.InvokeMember("AddFragment",
                        BindingFlags.InvokeMethod,
                        null,
                        part,
                        new[] { fr });
                }
            }

            return part;
        }
    }

    /// <summary>
    /// Options for <see cref="TokenTextLayerPartSeeder"/>.
    /// </summary>
    public sealed class TokenTextLayerPartSeederOptions
    {
        /// <summary>
        /// Gets or sets the maximum fragments count. The seeder will add
        /// from 1 to this count of fragments to the text layer part.
        /// </summary>
        public int MaxFragmentCount { get; set; }
    }
}
