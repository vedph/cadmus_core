using System;
using System.Text.Json;

namespace Cadmus.Core.Config;

/// <summary>
/// JSON-based data profile serializer.
/// </summary>
/// <seealso cref="IDataProfileSerializer" />
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
            })!;

        DataProfile profile = new()
        {
            // facets
            Facets = options.Facets,
            // flags
            Flags = options.Flags
        };

        // thesauri
        if (options.Thesauri != null)
        {
            profile.Thesauri = new Thesaurus[options.Thesauri.Count];
            int i = 0;
            foreach (ThesaurusOptions to in options.Thesauri)
            {
                Thesaurus thesaurus = new(to.Id!)
                {
                    TargetId = to.TargetId
                };
                if (to.Entries?.Count > 0)
                {
                    foreach (ThesaurusEntryOptions eo in to.Entries)
                        thesaurus.AddEntry(new ThesaurusEntry(eo.Id!, eo.Value!));
                }

                profile.Thesauri[i++] = thesaurus;
            }
        }

        return profile;
    }
}
