using System;
using System.Text.Json;

namespace Cadmus.Core.Config
{
    /// <summary>
    /// JSON-based data profile serializer.
    /// </summary>
    /// <seealso cref="Cadmus.Core.Config.IDataProfileSerializer" />
    public sealed class JsonDataProfileSerializer : IDataProfileSerializer
    {
        /// <summary>
        /// Reads the profile from the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The profile.</returns>
        /// <exception cref="ArgumentNullException">text</exception>
        public DataProfile Read(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            DataProfileOptions options = JsonSerializer.Deserialize
                <DataProfileOptions>(text, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                });

            DataProfile profile = new DataProfile
            {
                // facets
                Facets = options.Facets,
                // flags
                Flags = options.Flags
            };

            // thesauri
            if (options.Thesauri != null)
            {
                profile.Thesauri = new Thesaurus[options.Thesauri.Length];
                int i = 0;
                foreach (ThesaurusOptions to in options.Thesauri)
                {
                    Thesaurus thesaurus = new Thesaurus(to.Id)
                    {
                        TargetId = to.TargetId
                    };
                    foreach (ThesaurusEntryOptions eo in to.Entries)
                        thesaurus.AddEntry(new ThesaurusEntry(eo.Id, eo.Value));

                    profile.Thesauri[i++] = thesaurus;
                }
            }

            return profile;
        }
    }
}
