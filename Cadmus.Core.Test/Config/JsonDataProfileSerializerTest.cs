using Cadmus.Core.Config;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using Xunit;

namespace Cadmus.Core.Test.Config
{
    public sealed class JsonDataProfileSerializerTest
    {
        private static string LoadProfile(string resourceName)
        {
            using StreamReader reader = new StreamReader(
                Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(
                    $"Cadmus.Core.Test.Assets.{resourceName}"),
                Encoding.UTF8);
            return reader.ReadToEnd();
        }

        [Fact]
        public void Read_Profile_Ok()
        {
            string json = LoadProfile("SampleProfile.json");
            IDataProfileSerializer serializer = new JsonDataProfileSerializer();

            DataProfile profile = serializer.Read(json);

            // facets
            Assert.Single(profile.Facets);
            FacetDefinition facetDef = profile.Facets[0];
            Assert.Equal("facet-default", facetDef.Id);
            Assert.Equal("default", facetDef.Label);
            Assert.Equal("The default facet", facetDef.Description);
            Assert.Equal("FF0000", facetDef.ColorKey);
            Assert.Equal(7, facetDef.PartDefinitions.Count);

            // TODO: check each facet definition

            // flags
            Assert.Single(profile.Flags);
            FlagDefinition flagDef = profile.Flags[0];
            Assert.Equal(1, flagDef.Id);
            Assert.Equal("to revise", flagDef.Label);
            Assert.Equal("The item must be revised.", flagDef.Description);
            Assert.Equal("F08080", flagDef.ColorKey);

            // thesauri
            Assert.Equal(2, profile.Thesauri.Length);
            Thesaurus thesaurus = Array.Find(profile.Thesauri,
                t => t.Id == "categories@en");
            Assert.NotNull(thesaurus);
            Assert.Equal(16, thesaurus.GetEntries().Count);
            // TODO: check each entry

            thesaurus = Array.Find(profile.Thesauri,
                t => t.Id == "languages@en");
            Assert.NotNull(thesaurus);
            Assert.Equal(8, thesaurus.GetEntries().Count);
            // TODO: check each entry

            Assert.NotNull(profile.GraphPinFilter);
            Assert.Single(profile.GraphPinFilter.Clauses);
            Assert.NotNull(profile.NonGraphPinFilter);
            Assert.Single(profile.NonGraphPinFilter.Clauses);
        }
    }
}
