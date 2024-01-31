using Cadmus.Core;
using Cadmus.General.Parts;
using Cadmus.Refs.Bricks;
using Fusi.Antiquity.Chronology;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace Cadmus.Graph.Test;

public sealed class GraphUpdaterTest
{
    [Fact]
    public void Explain_Ok()
    {
        IGraphRepository repository = new MockGraphRepository();
        // load mappings
        string json = TestHelper.LoadResourceText("WorkMappingsDoc.json");
        JsonSerializerOptions options = new()
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
        };
        options.Converters.Add(new NodeMappingOutputJsonConverter());
        NodeMappingDocument doc = JsonSerializer.Deserialize<NodeMappingDocument>
            (json, options)!;
        // save mappings into DB
        foreach (NodeMapping mapping in doc.GetMappings())
            repository.AddMapping(mapping);
        // create source data
        IItem item = new Item
        {
            Title = "Alpha work",
            Description = "Alpha work description",
            FacetId = "work",
            SortKey = "alphawork",
            UserId = "zeus",
            CreatorId = "zeus"
        };
        HistoricalEventsPart part = new()
        {
            ItemId = item.Id,
            RoleId = "txt",
            UserId = "zeus",
            CreatorId = "zeus"
        };
        part.Events.Add(new HistoricalEvent
        {
            Eid = "alpha-send",
            Type = "text.send",
            Description = "The alpha work is sent to Petrarch.",
            Chronotopes = new List<AssertedChronotope>(new[]
            {
                new AssertedChronotope
                {
                    Date = new AssertedDate
                    {
                        A = Datation.Parse("1250 AD")!
                    },
                    Place = new AssertedPlace
                    {
                        // for the URI, mapping will prefix this with itn:places/
                        Value = "Arezzo"
                    }
                }
            }),
            RelatedEntities = new List<RelatedEntity>
            {
                new RelatedEntity
                {
                    // dbr = http://dbpedia.org/resource/
                    Id = new AssertedCompositeId
                    {
                        Target = new PinTarget
                        {
                            Gid = "dbr:Petrarch",
                            Label = "Petrarch"
                        }
                    },
                    Relation = "text:reception:recipient"
                }
            },
            Note = "An editorial note about this event."
        });
        item.Parts.Add(part);
        // setup updater (note that we discard metadata supplier for item's EID,
        // as it's not needed for this test and it would require additional
        // dependencies)
        GraphUpdater updater = new(repository);

        GraphUpdaterExplanation explanation = updater.Explain(item, part)!;

        Assert.NotNull(explanation);
        // metadata
        Assert.Equal(6, explanation.Metadata.Count);
        string eventUri = $"itn:events/{part.Id}/alpha-send";
        // the expected work URI should end with the item's EID, but here
        // the mapper has no access to it because for simplicity we have
        // omitted the EID metadata supplier
        string workUriPrefix = $"itn:works/{item.Id}/";

        // nodes
        Assert.Equal(3, explanation.Set.Nodes.Count);
        // event node
        UriNode? node = explanation.Set.Nodes.FirstOrDefault(
            n => n.Label == "itn:events/alpha-send");
        Assert.NotNull(node);
        // place node
        node = explanation.Set.Nodes.FirstOrDefault(
            n => n.Label == "itn:places/arezzo");
        Assert.NotNull(node);
        // timespan node
        node = explanation.Set.Nodes.FirstOrDefault(
            n => n.Label == "itn:timespans/ts");
        Assert.NotNull(node);

        // triples
        Assert.Equal(9, explanation.Set.Triples.Count);
        // event a E7_activity
        UriTriple? triple = explanation.Set.Triples.FirstOrDefault(
            t => t.SubjectUri == eventUri &&
            t.PredicateUri == "rdf:type" &&
            t.ObjectUri == "crm:e7_activity");
        Assert.NotNull(triple);
        // event P2_has_type text.send
        triple = explanation.Set.Triples.FirstOrDefault(
            t => t.SubjectUri == eventUri &&
            t.PredicateUri == "crm:p2_has_type" &&
            t.ObjectUri == "itn:event-types/text.send");
        Assert.NotNull(triple);
        // event P16_used_specific_object work
        triple = explanation.Set.Triples.FirstOrDefault(
            t => t.SubjectUri == eventUri &&
            t.PredicateUri == "crm:p16_used_specific_object" &&
            t.ObjectUri == workUriPrefix);
        Assert.NotNull(triple);
        // event P3_has_note literal
        triple = explanation.Set.Triples.FirstOrDefault(
            t => t.SubjectUri == eventUri &&
            t.PredicateUri == "crm:p3_has_note" &&
            t.ObjectLiteral == part.Events[0].Note);
        Assert.NotNull(triple);
        // itn:places/arezzo a crm:e53_place
        triple = explanation.Set.Triples.FirstOrDefault(
            t => t.SubjectUri == "itn:places/arezzo" &&
            t.PredicateUri == "rdf:type" &&
            t.ObjectUri == "crm:e53_place");
        Assert.NotNull(triple);
        // event P7_took_place_at itn:places/arezzo
        triple = explanation.Set.Triples.FirstOrDefault(
            t => t.SubjectUri == eventUri &&
            t.PredicateUri == "crm:p7_took_place_at" &&
            t.ObjectUri == "itn:places/arezzo");
        Assert.NotNull(triple);
        // event P4_has_time-span itn:timespans/ts
        triple = explanation.Set.Triples.FirstOrDefault(
            t => t.SubjectUri == eventUri &&
            t.PredicateUri == "crm:p4_has_time-span" &&
            t.ObjectUri == "itn:timespans/ts");
        Assert.NotNull(triple);
        // timespan/ts P82_at_some_time_within literal
        triple = explanation.Set.Triples.FirstOrDefault(
            t => t.SubjectUri == "itn:timespans/ts" &&
            t.PredicateUri == "crm:p82_at_some_time_within" &&
            t.ObjectLiteral == "1250");
        Assert.NotNull(triple);
        Assert.Equal(1250, triple.LiteralNumber);
        Assert.Equal("xs:float", triple.LiteralType);
        // timespan/ts P87_is_identified_by literal
        triple = explanation.Set.Triples.FirstOrDefault(
            t => t.SubjectUri == "itn:timespans/ts" &&
            t.PredicateUri == "crm:p87_is_identified_by" &&
            t.ObjectLiteral == "1250 AD");
        Assert.NotNull(triple);
    }
}
