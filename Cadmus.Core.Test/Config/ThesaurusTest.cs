using Cadmus.Core.Config;
using System.Collections.Generic;
using Xunit;

namespace Cadmus.Core.Test.Config
{
    public sealed class ThesaurusTest
    {
        [Fact]
        public void VisitByLevel_Ok()
        {
            Thesaurus thesaurus = new("sample@en");
            thesaurus.AddEntry(new ThesaurusEntry
            {
                Id = "colors",
                Value = "colors"
            });
            thesaurus.AddEntry(new ThesaurusEntry
            {
                Id = "colors.red",
                Value = "colors: red"
            });
            thesaurus.AddEntry(new ThesaurusEntry
            {
                Id = "colors.green",
                Value = "colors: green"
            });
            thesaurus.AddEntry(new ThesaurusEntry
            {
                Id = "colors.blue",
                Value = "colors: blue"
            });
            thesaurus.AddEntry(new ThesaurusEntry
            {
                Id = "shapes",
                Value = "shapes"
            });
            thesaurus.AddEntry(new ThesaurusEntry
            {
                Id = "shapes.circle",
                Value = "shapes: circle"
            });
            thesaurus.AddEntry(new ThesaurusEntry
            {
                Id = "shapes.triangle",
                Value = "shapes: triangle"
            });

            List<ThesaurusTreeEntry> visited = new();
            thesaurus.VisitByLevel(entry =>
            {
                visited.Add(entry);
                return true;
            });
            Assert.Equal(7, visited.Count);

            Assert.Equal("colors", visited[0].Id);
            Assert.Null(visited[0].Parent);
            Assert.Equal("shapes", visited[1].Id);
            Assert.Null(visited[1].Parent);

            Assert.Equal("colors.red", visited[2].Id);
            Assert.Equal("colors.green", visited[3].Id);
            Assert.Equal("colors.blue", visited[4].Id);
            Assert.Same(visited[0], visited[2].Parent);
            Assert.Same(visited[0], visited[3].Parent);
            Assert.Same(visited[0], visited[4].Parent);

            Assert.Equal("shapes.circle", visited[5].Id);
            Assert.Equal("shapes.triangle", visited[6].Id);
            Assert.Same(visited[1], visited[5].Parent);
            Assert.Same(visited[1], visited[6].Parent);
        }
    }
}
