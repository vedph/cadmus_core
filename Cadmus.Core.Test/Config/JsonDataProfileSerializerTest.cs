using Cadmus.Core.Config;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;

namespace Cadmus.Core.Test.Config;

public sealed class JsonDataProfileSerializerTest
{
    private static string LoadProfile(string resourceName)
    {
        using StreamReader reader = new(
            Assembly.GetExecutingAssembly()
            .GetManifestResourceStream(
                $"Cadmus.Core.Test.Assets.{resourceName}")!,
            Encoding.UTF8);
        return reader.ReadToEnd();
    }

    [Fact]
    public void Read_Profile_Ok()
    {
        string json = LoadProfile("SampleProfile.json");
        JsonDataProfileSerializer serializer = new();

        DataProfile profile = serializer.Read(json);

        // facets
        Assert.Single(profile.Facets!);
        FacetDefinition facetDef = profile.Facets![0];
        Assert.Equal("facet-default", facetDef.Id);
        Assert.Equal("default", facetDef.Label);
        Assert.Equal("The default facet", facetDef.Description);
        Assert.Equal("FF0000", facetDef.ColorKey);
        Assert.Equal(7, facetDef.PartDefinitions.Count);

        // flags
        Assert.Single(profile.Flags!);
        FlagDefinition flagDef = profile.Flags![0];
        Assert.Equal(1, flagDef.Id);
        Assert.Equal("to revise", flagDef.Label);
        Assert.Equal("The item must be revised.", flagDef.Description);
        Assert.Equal("F08080", flagDef.ColorKey);

        // thesauri
        Assert.Equal(2, profile.Thesauri!.Count);
        Thesaurus? thesaurus = profile.Thesauri.FirstOrDefault(
            t => t.Id == "categories@en");
        Assert.NotNull(thesaurus);
        Assert.Equal(16, thesaurus.Entries.Count);

        thesaurus = profile.Thesauri.FirstOrDefault(
            t => t.Id == "languages@en");
        Assert.NotNull(thesaurus);
        Assert.Equal(8, thesaurus.Entries.Count);
    }
}
