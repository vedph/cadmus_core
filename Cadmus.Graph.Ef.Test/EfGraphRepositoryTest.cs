using Cadmus.Core;
using Cadmus.Core.Config;
using Cadmus.General.Parts;
using Cadmus.Refs.Bricks;
using Fusi.Antiquity.Chronology;
using Fusi.DbManager;
using Fusi.Tools.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace Cadmus.Graph.Ef.Test;

/// <summary>
/// Base class for SQL-based graph repositories tests.
/// </summary>
public abstract class EfGraphRepositoryTest
{
    private const string DB_NAME = "cadmus-graph-test";

    /// <summary>
    /// Gets the connection string template.
    /// </summary>
    public abstract string ConnectionStringTemplate { get; }

    /// <summary>
    /// Gets the database manager.
    /// </summary>
    public abstract IDbManager DbManager { get; }

    /// <summary>
    /// Gets the connection string.
    /// </summary>
    public string ConnectionString =>
        string.Format(ConnectionStringTemplate, DB_NAME);

    /// <summary>
    /// Gets the database schema DDL SQL.
    /// </summary>
    /// <returns>SQL code.</returns>
    protected abstract string GetSchema();

    /// <summary>
    /// Gets the repository.
    /// </summary>
    /// <returns>Repository.</returns>
    protected abstract IGraphRepository GetRepository();

    /// <summary>
    /// Resets the database.
    /// </summary>
    protected void Reset()
    {
        if (DbManager.Exists(DB_NAME))
        {
            DbManager.ClearDatabase(DB_NAME);
        }
        else
        {
            DbManager.CreateDatabase(DB_NAME, GetSchema(), null);
        }
    }

    #region Namespace
    private static void AddNamespaces(int count, IGraphRepository repository)
    {
        for (int n = 1; n <= count; n++)
            repository.AddNamespace("p" + n, $"http://www.ns{n}.org");
    }

    protected void DoGetNamespaces_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddNamespaces(3, repository);

        DataPage<NamespaceEntry> page = repository.GetNamespaces(
            new NamespaceFilter
            {
                PageNumber = 1,
                PageSize = 10,
            });

        Assert.Equal(3, page.Total);
        Assert.Equal(3, page.Items.Count);
    }

    protected void DoGetNamespaces_Prefix_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddNamespaces(3, repository);

        DataPage<NamespaceEntry> page = repository.GetNamespaces(
            new NamespaceFilter
            {
                PageNumber = 1,
                PageSize = 10,
                Prefix = "p1"
            });

        Assert.Equal(1, page.Total);
        Assert.Single(page.Items);
    }

    protected void DoGetNamespaces_Uri_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddNamespaces(3, repository);

        DataPage<NamespaceEntry> page = repository.GetNamespaces(
            new NamespaceFilter
            {
                PageNumber = 1,
                PageSize = 10,
                Uri = "ns2"
            });

        Assert.Equal(1, page.Total);
        Assert.Single(page.Items);
    }

    protected void DoLookupNamespace_NotExisting_Null()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddNamespaces(2, repository);

        string? uri = repository.LookupNamespace("not-existing");

        Assert.Null(uri);
    }

    protected void DoLookupNamespace_Existing_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddNamespaces(2, repository);

        string? uri = repository.LookupNamespace("p1");

        Assert.Equal("http://www.ns1.org", uri);
    }

    protected void DoDeleteNamespaceByPrefix_NotExisting_Nope()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddNamespaces(2, repository);

        repository.DeleteNamespaceByPrefix("not-existing");

        Assert.Equal(2, repository.GetNamespaces(new NamespaceFilter()).Total);
    }

    protected void DoDeleteNamespaceByPrefix_Existing_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddNamespaces(2, repository);

        repository.DeleteNamespaceByPrefix("p2");

        Assert.NotNull(repository.LookupNamespace("p1"));
        Assert.Null(repository.LookupNamespace("p2"));
    }

    protected void DoDeleteNamespaceByUri_NotExisting_Nope()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddNamespaces(2, repository);

        repository.DeleteNamespaceByUri("not-existing");

        Assert.Equal(2, repository.GetNamespaces(new NamespaceFilter()).Total);
    }

    protected void DoDeleteNamespaceByUri_Existing_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddNamespaces(2, repository);
        repository.AddNamespace("p3", "http://www.ns1.org");

        repository.DeleteNamespaceByUri("http://www.ns1.org");

        Assert.Null(repository.LookupNamespace("p1"));
        Assert.Null(repository.LookupNamespace("p3"));
        Assert.NotNull(repository.LookupNamespace("p2"));
    }
    #endregion

    #region Uid
    protected void DoAddUid_NoClash_AddedNoSuffix()
    {
        Reset();
        IGraphRepository repository = GetRepository();

        string uid = repository.BuildUid("x:persons/john_doe",
            Guid.NewGuid().ToString());

        Assert.Equal("x:persons/john_doe", uid);
    }

    protected void DoAddUid_ClashUnique_AddedWithSuffix()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        string sid = Guid.NewGuid().ToString();
        string uid1 = repository.BuildUid("x:timespans/ts##", sid);

        string uid2 = repository.BuildUid("x:timespans/ts##",
            Guid.NewGuid().ToString());

        Assert.NotEqual(uid1, uid2);
        Assert.Equal("x:timespans/ts", uid1);
        Assert.Equal("x:timespans/ts#2", uid2);
    }

    protected void DoAddUid_ClashNotUnique_ReusedWithSuffix()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        string sid = Guid.NewGuid().ToString();
        string uid1 = repository.BuildUid("x:persons/john_doe", sid);

        string uid2 = repository.BuildUid("x:persons/john_doe", sid);

        Assert.Equal(uid1, uid2);
    }
    #endregion

    #region Uri
    protected void DoAddUri_NotExisting_Added()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        const string uri = "http://www.sample.com";

        int id = repository.AddUri(uri);

        string? uri2 = repository.LookupUri(id);
        Assert.Equal(uri, uri2);
    }

    protected void DoAddUri_Existing_Nope()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        const string uri = "http://www.sample.com";
        int id = repository.AddUri(uri);

        int id2 = repository.AddUri(uri);

        Assert.Equal(id, id2);
    }
    #endregion

    #region Node
    private static void AddNodes(int count, IGraphRepository repository,
        bool tag = false)
    {
        const string itemId = "e0cd9166-d005-404d-8f18-65be1f17b48f";
        const string partId = "f321f320-26da-4164-890b-e3974e9272ba";

        for (int i = 0; i < count; i++)
        {
            string uid = $"x:node{i + 1}";
            int id = repository.AddUri(uid);

            Node node = new()
            {
                Id = id,
                Label = $"Node {i + 1:00}",
                SourceType = i == 0
                    ? Node.SOURCE_ITEM
                    : Node.SOURCE_PART,
                Sid = i == 0 ? itemId : partId,
                Tag = tag ? ((i + 1) % 2 == 0 ? "even" : "odd") : null
            };
            repository.AddNode(node);
        }
    }

    protected void DoGetNodes_NoFilter_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddNodes(10, repository);

        DataPage<UriNode> page = repository.GetNodes(new NodeFilter());

        Assert.Equal(10 + 2, page.Total);
        Assert.Equal(10 + 2, page.Items.Count);
    }

    protected void DoGetNodes_NoFilterPage2_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddNodes(10, repository);

        DataPage<UriNode> page = repository.GetNodes(new NodeFilter
        {
            PageNumber = 2,
            PageSize = 5
        });

        Assert.Equal(10 + 2, page.Total);
        Assert.Equal(5, page.Items.Count);
        Assert.Equal("Node 04", page.Items[0].Label);
        Assert.Equal("Node 08", page.Items[4].Label);
    }

    protected void DoGetNodes_ByLabel_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddNodes(10, repository);

        DataPage<UriNode> page = repository.GetNodes(new NodeFilter
        {
            Label = "05"
        });

        Assert.Equal(1, page.Total);
        Assert.Single(page.Items);
        Assert.Equal("Node 05", page.Items[0].Label);
    }

    protected void DoGetNodes_ByLabelAndClass_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddNodes(10, repository);

        DataPage<UriNode> page = repository.GetNodes(new NodeFilter
        {
            Label = "05",
            IsClass = true
        });

        Assert.Equal(0, page.Total);
        Assert.Empty(page.Items);
    }

    protected void DoGetNodes_BySourceType_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddNodes(10, repository);

        DataPage<UriNode> page = repository.GetNodes(new NodeFilter
        {
            SourceType = Node.SOURCE_ITEM
        });

        Assert.Equal(1, page.Total);
        Assert.Single(page.Items);
        Assert.Equal("Node 01", page.Items[0].Label);
    }

    protected void DoGetNodes_BySidExact_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddNodes(10, repository);

        DataPage<UriNode> page = repository.GetNodes(new NodeFilter
        {
            Sid = "e0cd9166-d005-404d-8f18-65be1f17b48f"
        });

        Assert.Equal(1, page.Total);
        Assert.Single(page.Items);
        Assert.Equal("Node 01", page.Items[0].Label);
    }

    protected void DoGetNodes_BySidPrefix_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddNodes(10, repository);

        DataPage<UriNode> page = repository.GetNodes(new NodeFilter
        {
            Sid = "f321f320-26da-4164-890b-e3974e9272ba",
            IsSidPrefix = true
        });

        Assert.Equal(9, page.Total);
        Assert.Equal(9, page.Items.Count);
        Assert.Equal("Node 02", page.Items[0].Label);
    }

    protected void DoGetNodes_ByNoTag_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddNodes(3, repository);

        DataPage<UriNode> page = repository.GetNodes(new NodeFilter
        {
            Tag = ""
        });

        Assert.Equal(3, page.Total);
        Assert.Equal(3, page.Items.Count);
    }

    protected void DoGetNodes_ByTag_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddNodes(3, repository, true);

        DataPage<UriNode> page = repository.GetNodes(new NodeFilter
        {
            Tag = "odd"
        });

        Assert.Equal(2, page.Total);
        Assert.Equal(2, page.Items.Count);
    }

    protected void DoGetNodes_ByLinkedNode_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        // nodes
        Node argos = new()
        {
            Id = repository.AddUri("x:dogs/argos"),
            Label = "Argos"
        };
        repository.AddNode(argos);
        Node a = new()
        {
            Id = repository.AddUri("rdf:type"),
            Label = "a"
        };
        repository.AddNode(a);
        Node animal = new()
        {
            Id = repository.AddUri("x:animal"),
            Label = "animal",
            IsClass = true
        };
        repository.AddNode(animal);
        // triple
        repository.AddTriple(new Triple
        {
            SubjectId = argos.Id,
            PredicateId = a.Id,
            ObjectId = animal.Id
        });

        DataPage<UriNode> page = repository.GetNodes(new NodeFilter
        {
            LinkedNodeId = animal.Id
        });

        Assert.Equal(1, page.Total);
        Assert.Single(page.Items);
        Assert.Equal("Argos", page.Items[0].Label);
    }

    protected void DoGetNodes_ByLinkedNodeWithMatchingRole_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        // nodes
        Node argos = new()
        {
            Id = repository.AddUri("x:dogs/argos"),
            Label = "Argos",
        };
        repository.AddNode(argos);
        Node a = new()
        {
            Id = repository.AddUri("rdf:type"),
            Label = "a",
        };
        repository.AddNode(a);
        Node animal = new()
        {
            Id = repository.AddUri("x:animal"),
            Label = "animal",
            IsClass = true,
        };
        repository.AddNode(animal);
        // triple
        repository.AddTriple(new Triple
        {
            SubjectId = argos.Id,
            PredicateId = a.Id,
            ObjectId = animal.Id
        });

        DataPage<UriNode> page = repository.GetNodes(new NodeFilter
        {
            LinkedNodeId = animal.Id,
            LinkedNodeRole = 'O'
        });

        Assert.Equal(1, page.Total);
        Assert.Single(page.Items);
        Assert.Equal("Argos", page.Items[0].Label);
    }

    protected void DoGetNodes_ByLinkedNodeWithNonMatchingRole_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        // nodes
        Node argos = new()
        {
            Id = repository.AddUri("x:dogs/argos"),
            Label = "Argos",
        };
        repository.AddNode(argos);
        Node a = new()
        {
            Id = repository.AddUri("rdf:type"),
            Label = "a",
        };
        repository.AddNode(a);
        Node animal = new()
        {
            Id = repository.AddUri("x:animal"),
            Label = "animal",
            IsClass = true,
        };
        repository.AddNode(animal);
        // triple
        repository.AddTriple(new Triple
        {
            SubjectId = argos.Id,
            PredicateId = a.Id,
            ObjectId = animal.Id
        });

        DataPage<UriNode> page = repository.GetNodes(new NodeFilter
        {
            LinkedNodeId = animal.Id,
            LinkedNodeRole = 'S'
        });

        Assert.Equal(0, page.Total);
        Assert.Empty(page.Items);
    }

    protected void DoDeleteNode_NotExisting_Nope()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        Node argos = new()
        {
            Id = repository.AddUri("x:dogs/argos"),
            Label = "Argos",
        };
        repository.AddNode(argos);

        repository.DeleteNode(argos.Id + 10);

        Assert.NotNull(repository.GetNode(argos.Id));
    }

    protected void DoDeleteNode_Existing_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        Node argos = new()
        {
            Id = repository.AddUri("x:dogs/argos"),
            Label = "Argos",
        };
        repository.AddNode(argos);

        repository.DeleteNode(argos.Id);

        Assert.Null(repository.GetNode(argos.Id));
    }

    protected void DoGetLinkedNodes_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddPetrarchGraph(repository);
        // origin node
        UriNode? petrarch = repository.GetNodeByUri("x:guys/francesco_petrarca");
        Assert.NotNull(petrarch);

        // outbound (non-literal) nodes from property group rdf:type:
        // - x:guys/francesco_petrarca (S) rdf:type foaf:Person
        // rdf:type
        UriNode? a = repository.GetNodeByUri("rdf:type");
        Assert.NotNull(a);

        DataPage<UriNode> nodePage = repository.GetLinkedNodes(
            new LinkedNodeFilter
        {
            PredicateId = a!.Id,
            OtherNodeId = petrarch!.Id,
            IsObject = true
        });
        Assert.Equal(1, nodePage.Total);
        Assert.Equal("foaf:Person", nodePage.Items[0].Uri);

        // outbound literals nodes from property group rdfs:label:
        // - x:guys/francesco_petrarca rdfs:label "Francesco Petrarca"@it
        // rdfs:label
        UriNode? label = repository.GetNodeByUri("rdfs:label");
        Assert.NotNull(label);

        DataPage<UriTriple> triplePage = repository.GetLinkedLiterals(
            new LinkedLiteralFilter
        {
            PredicateId = label!.Id,
            SubjectId = petrarch.Id
        });
        Assert.Equal(1, triplePage.Total);
        Assert.Equal("Francesco Petrarca", triplePage.Items[0].ObjectLiteral);

        // inbound nodes from property group p98:
        // - crm:P98_brought_into_life x:guys/francesco_petrarca
        UriNode? p98 = repository.GetNodeByUri("crm:p98_brought_into_life");
        Assert.NotNull(p98);

        nodePage = repository.GetLinkedNodes(
            new LinkedNodeFilter
            {
                PredicateId = p98!.Id,
                OtherNodeId = petrarch!.Id,
                IsObject = false
            });
        Assert.Equal(1, nodePage.Total);
        Assert.Equal("x:events/birth", nodePage.Items[0].Uri);
    }
    #endregion

    #region Property
    private static void AddProperties(IGraphRepository repository)
    {
        Node comment = new()
        {
            Id = repository.AddUri("rdfs:comment"),
            Label = "comment"
        };
        repository.AddNode(comment);
        Node date = new()
        {
            Id = repository.AddUri("x:date"),
            Label = "Date"
        };
        repository.AddNode(date);

        repository.AddProperty(new Property
        {
            Id = comment.Id,
            DataType = "xsd:string",
            Description = "A comment.",
            LiteralEditor = "qed.md"
        });
        repository.AddProperty(new Property
        {
            Id = date.Id,
            DataType = "xs:date",
            Description = "A year-based date.",
            LiteralEditor = "qed.date"
        });
    }

    protected void DoGetProperties_NoFilter_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddProperties(repository);

        var page = repository.GetProperties(new PropertyFilter());

        Assert.Equal(2, page.Total);
        Assert.Equal(2, page.Items.Count);
    }

    protected void DoGetProperties_ByUid_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddProperties(repository);

        var page = repository.GetProperties(new PropertyFilter
        {
            Uid = "rdfs"
        });

        Assert.Equal(1, page.Total);
        Assert.Single(page.Items);
    }

    protected void DoGetProperties_ByDataType_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddProperties(repository);

        var page = repository.GetProperties(new PropertyFilter
        {
            DataType = "xsd:string"
        });

        Assert.Equal(1, page.Total);
        Assert.Single(page.Items);
    }

    protected void DoGetProperties_ByLiteralEditor_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddProperties(repository);

        var page = repository.GetProperties(new PropertyFilter
        {
            LiteralEditor = "qed.date"
        });

        Assert.Equal(1, page.Total);
        Assert.Single(page.Items);
    }

    protected void DoGetProperty_NotExisting_Null()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddProperties(repository);

        Assert.Null(repository.GetProperty(123));
    }

    protected void DoGetProperty_Existing_NotNull()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddProperties(repository);

        Assert.NotNull(repository.GetProperty(1));
    }

    protected void DoGetPropertyByUri_NotExisting_Null()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddProperties(repository);

        Assert.Null(repository.GetPropertyByUri("not-existing"));
    }

    protected void DoGetPropertyByUri_Existing_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddProperties(repository);

        Assert.NotNull(repository.GetPropertyByUri("rdfs:comment"));
    }

    protected void DoDeleteProperty_NotExisting_Nope()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddProperties(repository);

        repository.DeleteProperty(123);

        Assert.Equal(2, repository.GetProperties(new PropertyFilter()).Total);
    }

    protected void DoDeleteProperty_Existing_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddProperties(repository);

        repository.DeleteProperty(1);

        Assert.Equal(1, repository.GetProperties(new PropertyFilter()).Total);
    }
    #endregion

    #region Triple
    private static Triple AddMichelangeloArtist(IGraphRepository repository)
    {
        Node michelangelo = new()
        {
            Id = repository.AddUri("x:persons/michelangelo"),
            Label = "Michelangelo"
        };
        repository.AddNode(michelangelo);
        Node a = new()
        {
            Id = repository.AddUri("rdf:type"),
            Label = "a",
            Tag = Node.TAG_PROPERTY
        };
        repository.AddNode(a);
        Node artist = new()
        {
            Id = repository.AddUri("x:artist"),
            IsClass = true,
            Label = "Artist class"
        };
        repository.AddNode(artist);

        // triple: michelangelo a artist
        Triple triple = new()
        {
            SubjectId = michelangelo.Id,
            PredicateId = a.Id,
            ObjectId = artist.Id
        };
        repository.AddTriple(triple);

        return triple;
    }

    protected void DoAddTriple_NotExisting_Added()
    {
        Reset();
        IGraphRepository repository = GetRepository();

        Triple triple = AddMichelangeloArtist(repository);

        UriTriple? triple2 = repository.GetTriple(triple.Id);
        Assert.NotNull(triple2);
        Assert.Equal(triple.SubjectId, triple2!.SubjectId);
        Assert.Equal(triple.PredicateId, triple2.PredicateId);
        Assert.Equal(triple.ObjectId, triple2.ObjectId);
        Assert.Equal(triple.ObjectLiteral, triple2.ObjectLiteral);
        Assert.Equal(triple.Sid, triple2.Sid);
        Assert.Equal(triple.Tag, triple2.Tag);
    }

    protected void DoAddTriple_Same_Unchanged()
    {
        Reset();
        IGraphRepository repository = GetRepository();

        Triple triple = AddMichelangeloArtist(repository);
        Triple triple2 = AddMichelangeloArtist(repository);

        Assert.Equal(triple.Id, triple2.Id);
        Assert.Equal(1, repository.GetTriples(new TripleFilter()).Total);
    }

    private static void AddTriples(IGraphRepository repository)
    {
        // nodes
        Node michelangelo = new()
        {
            Id = repository.AddUri("x:persons/michelangelo"),
            Label = "Michelangelo"
        };
        repository.AddNode(michelangelo);

        Node a = new()
        {
            Id = repository.AddUri("rdf:type"),
            Label = "a",
            Tag = Node.TAG_PROPERTY
        };
        repository.AddNode(a);

        Node name = new()
        {
            Id = repository.AddUri("foaf:name"),
            Label = "Person name",
            Tag = Node.TAG_PROPERTY
        };
        repository.AddNode(name);

        Node artist = new()
        {
            Id = repository.AddUri("x:artist"),
            IsClass = true,
            Label = "Artist class"
        };
        repository.AddNode(artist);

        // michelangelo a artist
        repository.AddTriple(new Triple
        {
            SubjectId = michelangelo.Id,
            PredicateId = a.Id,
            ObjectId = artist.Id
        });
        // michelangelo hasName "Michelangelo Buonarroti"
        repository.AddTriple(new Triple
        {
            SubjectId = michelangelo.Id,
            PredicateId = name.Id,
            ObjectLiteral = "Michelangelo Buonarroti",
            Tag = "fake",
            Sid = "d33d98de-7e61-4c67-8ddb-0cd1b4f03dae"
        });
    }

    protected void DoGetTriples_NoFilter_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddTriples(repository);

        var page = repository.GetTriples(new TripleFilter());

        Assert.Equal(2, page.Total);
        Assert.Equal(2, page.Items.Count);
    }

    protected void DoGetTriples_BySubjectId_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddTriples(repository);

        var page = repository.GetTriples(new TripleFilter
        {
            SubjectId = 1
        });

        Assert.Equal(2, page.Total);
        Assert.Equal(2, page.Items.Count);
    }

    protected void DoGetTriples_ByPredicateId_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddTriples(repository);

        var page = repository.GetTriples(new TripleFilter
        {
            PredicateIds = new HashSet<int> { 4 }
        });

        Assert.Equal(1, page.Total);
        Assert.Single(page.Items);
        Assert.NotNull(page.Items[0].ObjectLiteral);
    }

    protected void DoGetTriples_ByObjectId_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddTriples(repository);

        var page = repository.GetTriples(new TripleFilter
        {
            ObjectId = 5
        });

        Assert.Equal(1, page.Total);
        Assert.Single(page.Items);
    }

    protected void DoGetTriples_ByLiteral_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddTriples(repository);

        var page = repository.GetTriples(new TripleFilter
        {
            LiteralPattern = "^Michelangelo"
        });

        Assert.Equal(1, page.Total);
        Assert.Single(page.Items);
        Assert.NotNull(page.Items[0].ObjectLiteral);
    }

    protected void DoGetTriples_BySidExact_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddTriples(repository);

        DataPage<UriTriple> page = repository.GetTriples(new TripleFilter
        {
            Sid = "d33d98de-7e61-4c67-8ddb-0cd1b4f03dae"
        });

        Assert.Equal(1, page.Total);
        Assert.Single(page.Items);
    }

    protected void DoGetTriples_BySidPrefix_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddTriples(repository);

        DataPage<UriTriple> page = repository.GetTriples(new TripleFilter
        {
            Sid = "d33d98de-7e61-4c67-8ddb-",
            IsSidPrefix = true
        });

        Assert.Equal(1, page.Total);
        Assert.Single(page.Items);
    }

    protected void DoGetTriples_ByNoTag_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddTriples(repository);

        var page = repository.GetTriples(new TripleFilter
        {
            Tag = ""
        });

        Assert.Equal(1, page.Total);
        Assert.Single(page.Items);
        Assert.Null(page.Items[0].ObjectLiteral);
    }

    protected void DoGetTriples_ByTag_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddTriples(repository);

        var page = repository.GetTriples(new TripleFilter
        {
            Tag = "fake"
        });

        Assert.Equal(1, page.Total);
        Assert.Single(page.Items);
        Assert.NotNull(page.Items[0].ObjectLiteral);
    }

    protected void DoGetTriple_NotExisting_Null()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddTriples(repository);

        UriTriple? triple = repository.GetTriple(123);

        Assert.Null(triple);
    }

    protected void DoGetTriple_Existing_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddTriples(repository);

        UriTriple? triple = repository.GetTriple(2);

        Assert.NotNull(triple);
        Assert.NotNull(triple!.ObjectLiteral);
        Assert.Equal("x:persons/michelangelo", triple.SubjectUri);
        Assert.Equal("foaf:name", triple.PredicateUri);
        Assert.Null(triple.ObjectUri);
    }

    protected void DoDeleteTriple_NotExisting_Nope()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddTriples(repository);

        repository.DeleteTriple(123);

        Assert.Equal(2, repository.GetTriples(new TripleFilter()).Total);
    }

    protected void DoDeleteTriple_Existing_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddTriples(repository);

        repository.DeleteTriple(2);

        Assert.NotNull(repository.GetTriple(1));
        Assert.Null(repository.GetTriple(2));
    }

    private static void AddPetrarchGraph(IGraphRepository repository)
    {
        IGraphPresetReader reader = new JsonGraphPresetReader();

        // nodes
        using (Stream stream = GetResourceStream("Petrarch-n.json"))
        using (ItemFlusher<UriNode> nodeFlusher = new(nodes =>
            repository.ImportNodes(nodes)))
        {
            foreach (UriNode node in reader.ReadNodes(stream))
                nodeFlusher.Add(node);
        }

        // triples
        using (Stream stream = GetResourceStream("Petrarch-t.json"))
        using (ItemFlusher<UriTriple> tripleFlusher = new(triples =>
            repository.ImportTriples(triples)))
        {
            foreach (UriTriple triple in reader.ReadTriples(stream))
                tripleFlusher.Add(triple);
        }
    }

    protected void DoGetTripleGroups_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        AddPetrarchGraph(repository);
        // origin node
        UriNode? petrarch = repository.GetNodeByUri("x:guys/francesco_petrarca");
        Assert.NotNull(petrarch);

        // 1) get outbound links
        DataPage<TripleGroup> page = repository.GetTripleGroups(new TripleFilter
        {
            SubjectId = petrarch!.Id,
            HasLiteralObject = false,
        });
        Assert.Equal(1, page.Total);
        // x:guys/francesco_petrarca rdf:type foaf:Person
        Assert.Equal("rdf:type", page.Items[0].PredicateUri);
        Assert.Equal(1, page.Items[0].Count);

        // 2) get inbound links
        page = repository.GetTripleGroups(new TripleFilter
        {
            ObjectId = petrarch!.Id
        });
        Assert.Equal(2, page.Total);
        // x:events/death crm:p98_brought_into_life x:guys/francesco_petrarca
        Assert.Equal("crm:p93_took_out_of_existence", page.Items[0].PredicateUri);
        Assert.Equal(1, page.Items[0].Count);
        // x:events/birth crm:p98_brought_into_life x:guys/francesco_petrarca
        Assert.Equal("crm:p98_brought_into_life", page.Items[1].PredicateUri);
        Assert.Equal(1, page.Items[1].Count);

        // 3) get outbound literals
        page = repository.GetTripleGroups(new TripleFilter
        {
            SubjectId = petrarch!.Id,
            HasLiteralObject = true,
        });
        Assert.Equal(1, page.Total);
        // x:guys/francesco_petrarca rdfs:label "Francesco Petrarca"@it
        Assert.Equal("rdfs:label", page.Items[0].PredicateUri);
        Assert.Equal(1, page.Items[0].Count);
    }
    #endregion

    #region Thesaurus
    static private Stream GetResourceStream(string name) =>
        typeof(EfGraphRepositoryTest).Assembly.GetManifestResourceStream(
        "Cadmus.Graph.Ef.Test.Assets." + name)!;

    private static Thesaurus GetThesaurus()
    {
        Thesaurus thesaurus = new("geometry@en");
        thesaurus.AddEntry(new ThesaurusEntry
        {
            Id = "shapes",
            Value = "shapes"
        });
        thesaurus.AddEntry(new ThesaurusEntry
        {
            Id = "shapes.2d",
            Value = "shapes: 2D"
        });
        thesaurus.AddEntry(new ThesaurusEntry
        {
            Id = "shapes.2d.circle",
            Value = "shapes: 2D: circle"
        });
        thesaurus.AddEntry(new ThesaurusEntry
        {
            Id = "shapes.2d.triangle",
            Value = "shapes: 2D: triangle"
        });
        thesaurus.AddEntry(new ThesaurusEntry
        {
            Id = "shapes.3d",
            Value = "shapes: 3D"
        });
        thesaurus.AddEntry(new ThesaurusEntry
        {
            Id = "shapes.3d.cube",
            Value = "shapes: 3D: cube"
        });

        return thesaurus;
    }

    protected void DoAddThesaurus_Root_Ok(string? prefix)
    {
        Reset();
        IGraphRepository repository = GetRepository();
        Thesaurus thesaurus = GetThesaurus();

        repository.AddThesaurus(thesaurus, true, prefix);

        // get nodes and triples
        IList<UriNode> nodes = repository.GetNodes(new NodeFilter
        {
            IsClass = true
        }).Items;
        Assert.Equal(8, nodes.Count);

        IList<UriTriple> triples =
            repository.GetTriples(new TripleFilter()).Items;
        Assert.Equal(6, triples.Count);

        const string sub = "rdfs:subClassOf";

        // geometry (root from ID)
        UriNode? geometry = nodes.FirstOrDefault(
            n => n.Uri == prefix + "geometry");
        Assert.NotNull(geometry);

        // shapes (root entry)
        UriNode? shapes = nodes.FirstOrDefault(
            n => n.Uri == prefix + "shapes");
        Assert.NotNull(shapes);
        // shapes subclass of geometry
        UriTriple? triple = triples.FirstOrDefault(
            t => t.SubjectUri == shapes!.Uri
            && t.PredicateUri == sub
            && t.ObjectUri == geometry!.Uri);
        Assert.NotNull(triple);

        // shapes.2d
        UriNode? shapes2d = nodes.FirstOrDefault(
            n => n.Uri == prefix + "shapes.2d");
        Assert.NotNull(shapes2d);
        // shapes.2d subclass of shapes
        triple = triples.FirstOrDefault(
            t => t.SubjectUri == shapes2d!.Uri
            && t.PredicateUri == sub
            && t.ObjectUri == shapes!.Uri);
        Assert.NotNull(triple);

        // shapes.3d
        UriNode? shapes3d = nodes.FirstOrDefault(
            n => n.Uri == prefix + "shapes.3d");
        Assert.NotNull(shapes3d);
        // shapes.3d subclass of shapes
        triple = triples.FirstOrDefault(
            t => t.SubjectUri == shapes3d!.Uri
            && t.PredicateUri == sub
            && t.ObjectUri == shapes!.Uri);
        Assert.NotNull(triple);

        // shapes.2d.circle
        UriNode? node = nodes.FirstOrDefault(
            n => n.Uri == prefix + "shapes.2d.circle");
        Assert.NotNull(node);
        // shapes.2d.circle subclass of shapes.2d
        triple = triples.FirstOrDefault(
            t => t.SubjectUri == node!.Uri
            && t.PredicateUri == sub
            && t.ObjectUri == shapes2d!.Uri);
        Assert.NotNull(triple);

        // shapes.2d.triangle
        node = nodes.FirstOrDefault(n => n.Uri == prefix + "shapes.2d.triangle");
        Assert.NotNull(node);
        // shapes.2d.triangle subclass of shapes.2d
        triple = triples.FirstOrDefault(
            t => t.SubjectUri == node!.Uri
            && t.PredicateUri == sub
            && t.ObjectUri == shapes2d!.Uri);
        Assert.NotNull(triple);

        // shapes.3d.cube
        node = nodes.FirstOrDefault(n => n.Uri == prefix + "shapes.3d.cube");
        Assert.NotNull(node);
        // shapes.3d.cube subclass of shapes.3d
        triple = triples.FirstOrDefault(
            t => t.SubjectUri == node!.Uri
            && t.PredicateUri == sub
            && t.ObjectUri == shapes3d!.Uri);
        Assert.NotNull(triple);
    }

    protected void DoAddRealThesaurus_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        IGraphPresetReader reader = new JsonGraphPresetReader();

        using var stream = GetResourceStream("Thesauri.json");
        foreach (Thesaurus thesaurus in reader.ReadThesauri(stream))
        {
            repository.AddThesaurus(thesaurus, false, "x:classes/");
        }
    }
    #endregion

    #region Mapping
    // MappingsDoc.json:
    // - event_note: T #4
    // - event_chronotopes: #5
    //   - event_chronotopes/place: NT #6
    //   - event_chronotopes/date: MNT #7
    // - person: NT #1
    // - work: NT #2
    // - text_sent_event: MNT #3
    //   - *event_note #4
    //   - *event_chronotopes #5
    //   - text_sent_event/related/carried_out_by: NT #8
    //   - text_sent_event/related/has_participant: NT #9
    // - text_reception_event: MNT #10
    //   - *event_note #4
    //   - *event_chronotopes #5
    //   - text_reception_event/related/carried_out_by: NT #11
    //   - text_reception_event/related/has_participant: NT #12
    // - text_transcription_event: MNT #13
    //   - *event_note #4
    //   - *event_chronotopes #5
    //   - text_transcription_event/related/carried_out_by: NT #14
    //   - text_transcription_event/related/has_produced: NT #15
    // - text_collection_event: MNT #16
    //   - *event_note #4
    //   - *event_chronotopes #5
    //   - text_collection_event/related/has_component: NT #17
    // - text_version_event: MNT #18
    //   - *event_note #4
    //   - *event_chronotopes #5
    //   - text_collection_event/related/refers_to: NT #19
    // - work_info: NT #20
    //   - work_info/author-ids@eg #21
    //   - work_info/author-ids@el #22
    //   - work_info/author-ids@i #23

    protected void DoLoadMappings_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();

        // load mappings
        string json = TestHelper.LoadResourceText("MappingsDoc.json");
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

        // root mappings
        DataPage<NodeMapping> page = repository.GetMappings(new NodeMappingFilter
        {
            PageSize = 30
        }, true);
        Assert.Equal(8, page.Total);

        // child mappings
        page = repository.GetMappings(new NodeMappingFilter
        {
            PageSize = 30,
            ParentId = 5
        }, true);
        Assert.Equal(2, page.Total);
    }

    private static void AssertMappingsEqual(NodeMapping expected,
        NodeMapping actual, bool output, bool children)
    {
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.ParentId, actual.ParentId);
        Assert.Equal(expected.Ordinal, actual.Ordinal);
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.SourceType, actual.SourceType);
        Assert.Equal(expected.FacetFilter, actual.FacetFilter);
        Assert.Equal(expected.GroupFilter, actual.GroupFilter);
        Assert.Equal(expected.FlagsFilter, actual.FlagsFilter);
        Assert.Equal(expected.TitleFilter, actual.TitleFilter);
        Assert.Equal(expected.PartTypeFilter, actual.PartTypeFilter);
        Assert.Equal(expected.PartRoleFilter, actual.PartRoleFilter);
        Assert.Equal(expected.Description, actual.Description);
        Assert.Equal(expected.Source, actual.Source);
        Assert.Equal(expected.Sid, actual.Sid);

        // output
        if (output)
        {
            if (expected.Output == null) Assert.Null(actual.Output);
            else
            {
                Assert.NotNull(actual.Output);
                Assert.Equal(expected.Output.HasNodes, actual.Output!.HasNodes);
                Assert.Equal(expected.Output.HasNodes, actual.Output.HasTriples);
                Assert.Equal(expected.Output.HasNodes, actual.Output.HasMetadata);
            }
        }

        // children
        if (children)
        {
            Assert.Equal(expected.HasChildren, actual.HasChildren);
            if (expected.HasChildren)
            {
                for (int i = 0; i < expected.Children.Count; i++)
                {
                    AssertMappingsEqual(expected.Children[i], actual.Children[i],
                        output, true);
                }
            }
        }
    }

    private static IList<NodeMapping> GetMappings(int count)
    {
        List<NodeMapping> mappings = new(count);
        for (int n = 1; n <= count; n++)
        {
            NodeMapping mapping = new()
            {
                Ordinal = n,
                SourceType = Node.SOURCE_ITEM,
                Name = "m" + n,
                FacetFilter = "person",
                Description = "Mapping " + n,
                Source = "source-" + n,
                Sid = "{?item-id}",
                Output = new NodeMappingOutput
                {
                    Nodes = new Dictionary<string, MappedNode>()
                    {
                        [$"node{n}"] = new MappedNode
                        {
                            Label = "Node " + n,
                            Uid = "x:nodes/n" + n,
                            Tag = n == 1? "one" : null
                        }
                    },
                    Triples = new List<MappedTriple>()
                    {
                        new MappedTriple
                        {
                            S = "{?node" + n + "}",
                            P = "rdfs:label",
                            OL = "Node " + n
                        }
                    },
                    Metadata = new Dictionary<string, string>()
                    {
                        ["n"] = $"{n}"
                    }
                }
            };
            mappings.Add(mapping);
        }
        return mappings;
    }

    protected void DoAddMapping_NotExisting_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();

        // item mapping
        NodeMapping mapping = GetMappings(1)[0];
        repository.AddMapping(mapping);

        Assert.True(mapping.Id > 0);
        NodeMapping? mapping2 = repository.GetMapping(mapping.Id);
        Assert.NotNull(mapping2);
        AssertMappingsEqual(mapping, mapping2!, true, true);
    }

    protected void DoAddMapping_Existing_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();

        // item mapping
        NodeMapping mapping = new()
        {
            FacetFilter = "person",
            Name = "Item",
            Description = "Description"
        };
        repository.AddMapping(mapping);

        // update
        mapping.Description = "Updated!";
        repository.AddMapping(mapping);

        NodeMapping? mapping2 = repository.GetMapping(mapping.Id);
        Assert.NotNull (mapping2);
        Assert.Equal(mapping.SourceType, mapping2!.SourceType);
        Assert.Equal(mapping.Name, mapping2!.Name);
        Assert.Equal(mapping.FacetFilter, mapping2.FacetFilter);
        Assert.Equal(mapping.Description, mapping2.Description);
    }

    protected void DoAddMappingByName_NotExisting_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();

        // item mapping
        NodeMapping mapping = GetMappings(1)[0];
        repository.AddMapping(mapping);

        Assert.True(mapping.Id > 0);
        NodeMapping? mapping2 = repository.GetMapping(mapping.Id);
        Assert.NotNull(mapping2);
        AssertMappingsEqual(mapping, mapping2!, true, true);
    }

    protected void DoAddMappingByName_Existing_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();

        // item mapping
        NodeMapping mapping = new()
        {
            FacetFilter = "person",
            Name = "Item",
            Description = "Description"
        };
        repository.AddMapping(mapping);

        // update
        mapping.Id = 0;
        mapping.Description = "Updated!";
        repository.AddMapping(mapping);

        NodeMapping? mapping2 = repository.GetMapping(mapping.Id);
        Assert.NotNull(mapping2);
        Assert.Equal(mapping.SourceType, mapping2!.SourceType);
        Assert.Equal(mapping.Name, mapping2!.Name);
        Assert.Equal(mapping.FacetFilter, mapping2.FacetFilter);
        Assert.Equal(mapping.Description, mapping2.Description);
    }

    protected void DoDeleteMapping_NotExisting_Nope()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        // item mapping
        NodeMapping mapping = new()
        {
            FacetFilter = "person",
            Name = "Item",
            Description = "Description"
        };
        repository.AddMapping(mapping);

        repository.DeleteMapping(123);

        Assert.NotNull(repository.GetMapping(mapping.Id));
    }

    protected void DoDeleteMapping_Existing_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        // item mapping
        NodeMapping mapping = new()
        {
            FacetFilter = "person",
            Name = "Item",
            Description = "Description"
        };
        repository.AddMapping(mapping);

        repository.DeleteMapping(mapping.Id);

        Assert.Null(repository.GetMapping(mapping.Id));
    }

    protected void DoGetMappings_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        IList<NodeMapping> mappings = GetMappings(3);
        mappings[0].Children.Add(new NodeMapping
        {
            Name = "m1-child"
        });
        foreach (NodeMapping mapping in mappings)
            repository.AddMapping(mapping);

        DataPage<NodeMapping> page = repository.GetMappings(
            new NodeMappingFilter
            {
                PageSize = 2,
                SourceType = Node.SOURCE_ITEM
            }, true);

        Assert.Equal(3, page.Total);
        Assert.Equal(2, page.Items.Count);

        Assert.Equal("m1", page.Items[0].Name);
        Assert.NotNull(page.Items[0].Output);
        Assert.True(page.Items[0].HasChildren);
        Assert.Equal("m1-child", page.Items[0].Children[0].Name);

        Assert.Equal("m2", page.Items[1].Name);
        Assert.NotNull(page.Items[1].Output);
        Assert.False(page.Items[1].HasChildren);
    }

    protected void DoFindMappings_Ok()
    {
        Reset();
        IGraphRepository repository = GetRepository();
        IList<NodeMapping> mappings = GetMappings(3);
        mappings[0].Children.Add(new NodeMapping
        {
            Name = "child",
            Output = new NodeMappingOutput
            {
                Metadata = new Dictionary<string, string>
                {
                    ["x"] = "y"
                }
            }
        });
        mappings[1].FacetFilter = "x";
        foreach (NodeMapping mapping in mappings)
            repository.AddMapping(mapping);

        IList<NodeMapping> results = repository.FindMappings(
            new RunNodeMappingFilter
            {
                SourceType = Node.SOURCE_ITEM,
                Facet = "person"
            });

        Assert.Equal(2, results.Count);

        Assert.Equal("m1", results[0].Name);
        Assert.NotNull(results[0].Output);
        Assert.True(results[0].HasChildren);
        Assert.Equal("child", results[0].Children[0].Name);
        Assert.NotNull(results[0].Children[0].Output);

        Assert.Equal("m3", results[1].Name);
        Assert.NotNull(results[1].Output);
        Assert.False(results[1].HasChildren);
    }

    protected void DoUpdateGraph_Event_Ok()
    {
        const string ITEM_ID = "125b510b-6159-4396-b61c-5a4a40488ee8";
        const string PART_ID = "dbf5c6aa-7781-45fd-accb-5c5365e2596f";
        Reset();
        IGraphRepository repository = GetRepository();
        // load mappings
        string json = TestHelper.LoadResourceText("MappingsDoc.json");
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
        IItem item = new Item
        {
            Id = ITEM_ID,
            Title = "Alpha work",
            Description = "Alpha work description",
            FacetId = "work",
            SortKey = "alphawork",
            UserId = "zeus",
            CreatorId = "zeus"
        };
        HistoricalEventsPart part = new()
        {
            Id = PART_ID,
            ItemId = ITEM_ID,
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
        // setup updater
        GraphUpdater updater = new(repository)
        {
            MetadataSupplier = new MetadataSupplier()
                .AddMetadataSource(new MockItemEidMetadataSource(part.Id, "alpha"))
        };
        updater.Update(item, part);

        // nodes
        string eventUri = $"itn:events/{part.Id}/alpha-send";
        string sid = $"{part.Id}/alpha-send";
        // the expected work URI ends with the item's EID (here provided by
        // the mock supplier)
        string workUri = $"itn:works/{PART_ID}/alpha";
        Node? nodeEvent = repository.GetNodeByUri(eventUri);
        Assert.NotNull(nodeEvent);
        Assert.False(nodeEvent.IsClass);
        Assert.Equal(2, nodeEvent.SourceType);
        Assert.Equal(sid, nodeEvent.Sid);
        // place node
        Node? nodePlace = repository.GetNodeByUri("itn:places/arezzo");
        Assert.NotNull(nodePlace);
        Assert.False(nodePlace.IsClass);
        Assert.Equal(2, nodePlace.SourceType);
        Assert.Equal(sid + "/chronotopes", nodePlace.Sid);
        // timespan node
        Node? nodeTs = repository.GetNodeByUri("itn:timespans/ts");
        Assert.NotNull(nodeTs);
        Assert.False(nodeTs.IsClass);
        Assert.Equal(2, nodeTs.SourceType);
        Assert.Equal(sid + "/chronotopes", nodeTs.Sid);
        // work node
        Node? nodeWork = repository.GetNodeByUri(workUri);
        Assert.NotNull(nodeWork);
        Assert.False(nodeWork.IsClass);
        Assert.Equal(4, nodeWork.SourceType);
        Assert.Equal(sid, nodeWork.Sid);

        // triples
        Node? nodeA = repository.GetNodeByUri("rdf:type");
        Assert.NotNull(nodeA);
        // event a E7_activity
        DataPage<UriTriple> page = repository.GetTriples(new TripleFilter
        {
            SubjectId = nodeEvent.Id,
            PredicateIds = new HashSet<int> { nodeA.Id },
            ObjectId = repository.GetNodeByUri("crm:e7_activity")?.Id ?? 0,
        });
        Assert.Equal(1, page.Total);
        // event P2_has_type text.send
        page = repository.GetTriples(new TripleFilter
        {
            SubjectId = nodeEvent.Id,
            PredicateIds = new HashSet<int>
                { repository.GetNodeByUri("crm:p2_has_type")?.Id ?? 0 },
            ObjectId = repository.GetNodeByUri("itn:event_types/text.send")?.Id ?? 0,
        });
        Assert.Equal(1, page.Total);
        // event P16_used_specific_object work
        page = repository.GetTriples(new TripleFilter
        {
            SubjectId = nodeEvent.Id,
            PredicateIds = new HashSet<int>
            { repository.GetNodeByUri("crm:p16_used_specific_object")?.Id ?? 0 },
            ObjectId = nodeWork.Id,
        });
        Assert.Equal(1, page.Total);
        // event P3_has_note literal
        page = repository.GetTriples(new TripleFilter
        {
            SubjectId = nodeEvent.Id,
            PredicateIds = new HashSet<int>
                { repository.GetNodeByUri("crm:p3_has_note")?.Id ?? 0 },
        });
        Assert.Equal(1, page.Total);
        Assert.Equal(part.Events[0].Note, page.Items[0].ObjectLiteral);
        // itn:places/arezzo a crm:e53_place
        page = repository.GetTriples(new TripleFilter
        {
            SubjectId = nodePlace.Id,
            PredicateIds = new HashSet<int> { nodeA.Id },
            ObjectId = repository.GetNodeByUri("crm:e53_place")?.Id ?? 0,
        });
        Assert.Equal(1, page.Total);
        // event P7_took_place_at itn:places/arezzo
        page = repository.GetTriples(new TripleFilter
        {
            SubjectId = nodeEvent.Id,
            PredicateIds = new HashSet<int>
                { repository.GetNodeByUri("crm:p7_took_place_at")?.Id ?? 0 },
            ObjectId = nodePlace.Id,
        });
        Assert.Equal(1, page.Total);
        // event P4_has_time-span itn:timespans/ts
        page = repository.GetTriples(new TripleFilter
        {
            SubjectId = nodeEvent.Id,
            PredicateIds = new HashSet<int>
                { repository.GetNodeByUri("crm:p4_has_time-span")?.Id ?? 0 },
            ObjectId = nodeTs.Id,
        });
        Assert.Equal(1, page.Total);
        // timespan/ts P82_at_some_time_within literal
        page = repository.GetTriples(new TripleFilter
        {
            SubjectId = nodeTs.Id,
            PredicateIds = new HashSet<int>
                { repository.GetNodeByUri("crm:p82_at_some_time_within")?.Id ?? 0 },
        });
        Assert.Equal(1, page.Total);
        Assert.Equal("1250", page.Items[0].ObjectLiteral);
        Assert.Equal(1250, page.Items[0].LiteralNumber);
        // timespan/ts P87_is_identified_by literal
        page = repository.GetTriples(new TripleFilter
        {
            SubjectId = nodeTs.Id,
            PredicateIds = new HashSet<int>
                { repository.GetNodeByUri("crm:p87_is_identified_by")?.Id ?? 0 },
        });
        Assert.Equal(1, page.Total);
        Assert.Equal("1250 AD", page.Items[0].ObjectLiteral);
    }

    protected void DoUpdateGraph_Work_Ok()
    {
        const string ITEM_ID = "125b510b-6159-4396-b61c-5a4a40488ee8";
        const string PART_ID = "dbf5c6aa-7781-45fd-accb-5c5365e2596f";
        Reset();
        IGraphRepository repository = GetRepository();
        // load mappings
        string json = TestHelper.LoadResourceText("MappingsDoc.json");
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
        // item with metadata part with EID=alpha
        IItem item = new Item
        {
            Id = ITEM_ID,
            Title = "Alpha work",
            Description = "Alpha work description",
            FacetId = "work",
            SortKey = "alphawork",
            UserId = "zeus",
            CreatorId = "zeus"
        };
        MetadataPart part = new()
        {
            Id = PART_ID,
            ItemId = ITEM_ID,
            UserId = "zeus",
            CreatorId = "zeus",
        };
        part.Metadata.Add(new Metadatum
        {
            Name = "eid",
            Value = "alpha"
        });
        // setup updater
        GraphUpdater updater = new(repository)
        {
            MetadataSupplier = new MetadataSupplier()
                .AddMetadataSource(new MockItemEidMetadataSource(part.Id, "alpha"))
        };

        // update
        updater.Update(item, part);

        // work node
        string workUri = $"itn:works/{PART_ID}/alpha";
        Node? nodeWork = repository.GetNodeByUri(workUri);
        Assert.NotNull(nodeWork);
        Assert.False(nodeWork.IsClass);
        Assert.Equal(2, nodeWork.SourceType);
    }

    protected void DoUpdateGraph_RelatedWork_Ok()
    {
        const string ITEM_A_ID = "125b510b-6159-4396-b61c-5a4a40488ee8";
        const string ITEM_B_ID = "14b1048a-86ab-49db-a061-ffaedfe7d98e";
        const string PART_A_ID = "dbf5c6aa-7781-45fd-accb-5c5365e2596f";
        const string PART_B_ID = "38e9a400-4f83-436d-9a15-037a5111932b";
        Reset();
        IGraphRepository repository = GetRepository();
        // load mappings
        string json = TestHelper.LoadResourceText("MappingsDoc.json");
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

        // work alpha
        IItem workA = new Item
        {
            Id = ITEM_A_ID,
            Title = "Alpha work",
            Description = "Alpha work description",
            FacetId = "work",
            SortKey = "alphawork",
            UserId = "zeus",
            CreatorId = "zeus"
        };
        MetadataPart partA = new()
        {
            Id = PART_A_ID,
            ItemId = ITEM_A_ID,
            UserId = "zeus",
            CreatorId = "zeus",
        };
        partA.Metadata.Add(new Metadatum
        {
            Name = "eid",
            Value = "alpha"
        });
        // create node for work alpha
        MockItemEidMetadataSource metaSource = new(partA.Id, "alpha");
        GraphUpdater updater = new(repository)
        {
            MetadataSupplier = new MetadataSupplier()
                .AddMetadataSource(metaSource)
        };
        updater.Update(workA, partA);

        // work beta
        IItem workB = new Item
        {
            Id = ITEM_B_ID,
            Title = "Beta work",
            Description = "Beta work description",
            FacetId = "work",
            SortKey = "betawork",
            UserId = "zeus",
            CreatorId = "zeus"
        };
        MetadataPart partB = new()
        {
            Id = PART_B_ID,
            ItemId = ITEM_B_ID,
            UserId = "zeus",
            CreatorId = "zeus",
        };
        partB.Metadata.Add(new Metadatum
        {
            Name = "eid",
            Value = "beta"
        });
        // create node for work beta
        metaSource.Set(partB.Id, "beta");
        updater.Update(workB, partB);

        // event: work alpha is previous version of work beta
        HistoricalEventsPart eventsA = new()
        {
            ItemId = ITEM_A_ID,
            UserId = "zeus",
            CreatorId = "zeus"
        };
        eventsA.Events.Add(new HistoricalEvent
        {
            Eid = "version",
            Type = "text.version",
            RelatedEntities = new List<RelatedEntity>
            {
                new RelatedEntity
                {
                    Relation = "text:version:previous",
                    Id = new AssertedCompositeId
                    {
                        Target = new PinTarget
                        {
                            ItemId = ITEM_B_ID,
                            PartId = PART_B_ID,
                            PartTypeId = partB.TypeId,
                            Name = "eid",
                            Value = "beta"
                        }
                    }
                }
            }
        });
        workA.Parts.Add(eventsA);
        metaSource.Set(partA.Id, "alpha");

        updater.Update(workA, eventsA);

        // work nodes
        string workAUri = $"itn:works/{PART_A_ID}/alpha";
        Node? nodeWork = repository.GetNodeByUri(workAUri);
        Assert.NotNull(nodeWork);
        Assert.False(nodeWork.IsClass);
        Assert.Equal(2, nodeWork.SourceType);

        string workBUri = $"itn:works/{PART_B_ID}/beta";
        nodeWork = repository.GetNodeByUri(workBUri);
        Assert.NotNull(nodeWork);
        Assert.False(nodeWork.IsClass);
        Assert.Equal(2, nodeWork.SourceType);

        // event node
        string eventUri = $"itn:events/{eventsA.Id}/version";
        Node? nodeEvent = repository.GetNodeByUri(eventUri);
        Assert.NotNull(nodeEvent);
        Assert.False(nodeEvent.IsClass);
        Assert.Equal(2, nodeEvent.SourceType);

        // triples
        DataPage<UriTriple> page = repository.GetTriples(new TripleFilter());
        Assert.Equal(6, page.Total);

        // itn:works/alpha a crm:E90_symbolic_object
        Assert.NotNull(page.Items.FirstOrDefault(t =>
            t.SubjectUri == workAUri &&
            t.PredicateUri == "rdf:type" &&
            t.ObjectUri == "crm:e90_symbolic_object"));

        // itn:works/beta a crm:E90_symbolic_object
        Assert.NotNull(page.Items.FirstOrDefault(t =>
            t.SubjectUri == workBUri &&
            t.PredicateUri == "rdf:type" &&
            t.ObjectUri == "crm:e90_symbolic_object"));

        // itn:events/version a crm:E7_activity
        Assert.NotNull(page.Items.FirstOrDefault(t =>
            t.SubjectUri == eventUri &&
            t.PredicateUri == "rdf:type" &&
            t.ObjectUri == "crm:e7_activity"));

        // itn:events/version crm:P2_has_type itn:event-types/text.version
        Assert.NotNull(page.Items.FirstOrDefault(t =>
            t.SubjectUri == eventUri &&
            t.PredicateUri == "crm:p2_has_type" &&
            t.ObjectUri == "itn:event-types/text.version"));

        // itn:events/version P67_refers_to itn:works/alpha
        Assert.NotNull(page.Items.FirstOrDefault(t =>
            t.SubjectUri == eventUri &&
            t.PredicateUri == "crm:p67_refers_to" &&
            t.ObjectUri == workAUri));

        // itn:events/version P67_refers_to itn:works/beta
        Assert.NotNull(page.Items.FirstOrDefault(t =>
            t.SubjectUri == eventUri &&
            t.PredicateUri == "crm:p67_refers_to" &&
            t.ObjectUri == workBUri));
    }

    protected void DoUpdateGraph_RelatedWorkUpdate_Ok()
    {
        const string ITEM_A_ID = "125b510b-6159-4396-b61c-5a4a40488ee8";
        const string ITEM_B_ID = "14b1048a-86ab-49db-a061-ffaedfe7d98e";
        const string ITEM_G_ID = "14a31d6e-ea2b-4392-9d18-c45bbbfbe4c0";
        const string PART_A_ID = "dbf5c6aa-7781-45fd-accb-5c5365e2596f";
        const string PART_B_ID = "38e9a400-4f83-436d-9a15-037a5111932b";
        const string PART_G_ID = "b9b274cc-7bba-42d3-960c-cb442c625d80";
        Reset();
        IGraphRepository repository = GetRepository();
        // load mappings
        string json = TestHelper.LoadResourceText("MappingsDoc.json");
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

        // work alpha
        IItem workA = new Item
        {
            Id = ITEM_A_ID,
            Title = "Alpha work",
            Description = "Alpha work description",
            FacetId = "work",
            SortKey = "alphawork",
            UserId = "zeus",
            CreatorId = "zeus"
        };
        MetadataPart partA = new()
        {
            Id = PART_A_ID,
            ItemId = ITEM_A_ID,
            UserId = "zeus",
            CreatorId = "zeus",
        };
        partA.Metadata.Add(new Metadatum
        {
            Name = "eid",
            Value = "alpha"
        });
        // create node for work alpha
        MockItemEidMetadataSource metaSource = new(partA.Id, "alpha");
        GraphUpdater updater = new(repository)
        {
            MetadataSupplier = new MetadataSupplier()
                .AddMetadataSource(metaSource)
        };
        updater.Update(workA, partA);

        // work beta
        IItem workB = new Item
        {
            Id = ITEM_B_ID,
            Title = "Beta work",
            Description = "Beta work description",
            FacetId = "work",
            SortKey = "betawork",
            UserId = "zeus",
            CreatorId = "zeus"
        };
        MetadataPart partB = new()
        {
            Id = PART_B_ID,
            ItemId = ITEM_B_ID,
            UserId = "zeus",
            CreatorId = "zeus",
        };
        partB.Metadata.Add(new Metadatum
        {
            Name = "eid",
            Value = "beta"
        });
        // create node for work beta
        metaSource.Set(partB.Id, "beta");
        updater.Update(workB, partB);

        // work gamma
        IItem workG = new Item
        {
            Id = ITEM_G_ID,
            Title = "Gamma work",
            Description = "Gamma work description",
            FacetId = "work",
            SortKey = "gammawork",
            UserId = "zeus",
            CreatorId = "zeus"
        };
        MetadataPart partG = new()
        {
            Id = PART_G_ID,
            ItemId = ITEM_G_ID,
            UserId = "zeus",
            CreatorId = "zeus",
        };
        partG.Metadata.Add(new Metadatum
        {
            Name = "eid",
            Value = "gamma"
        });
        // create node for work gamma
        metaSource.Set(partG.Id, "gamma");
        updater.Update(workG, partG);

        // event: work alpha is previous version of work beta
        HistoricalEventsPart eventsA = new()
        {
            ItemId = ITEM_A_ID,
            UserId = "zeus",
            CreatorId = "zeus"
        };
        eventsA.Events.Add(new HistoricalEvent
        {
            Eid = "version",
            Type = "text.version",
            RelatedEntities = new List<RelatedEntity>
            {
                new RelatedEntity
                {
                    Relation = "text:version:previous",
                    Id = new AssertedCompositeId
                    {
                        Target = new PinTarget
                        {
                            ItemId = ITEM_B_ID,
                            PartId = PART_B_ID,
                            PartTypeId = partB.TypeId,
                            Name = "eid",
                            Value = "beta"
                        }
                    }
                }
            }
        });
        workA.Parts.Add(eventsA);
        metaSource.Set(partA.Id, "alpha");
        updater.Update(workA, eventsA);

        // update the event adding another related entity:
        // work alpha is previous version of work gamma
        eventsA.Events[0].RelatedEntities.Add(new RelatedEntity
        {
            Relation = "text:version:previous",
            Id = new AssertedCompositeId
            {
                Target = new PinTarget
                {
                    ItemId = ITEM_G_ID,
                    PartId = PART_G_ID,
                    PartTypeId = partG.TypeId,
                    Name = "eid",
                    Value = "gamma"
                }
            }
        });

        updater.Update(workA, eventsA);

        // work nodes
        string workAUri = $"itn:works/{PART_A_ID}/alpha";
        Node? nodeWork = repository.GetNodeByUri(workAUri);
        Assert.NotNull(nodeWork);
        Assert.False(nodeWork.IsClass);
        Assert.Equal(2, nodeWork.SourceType);

        string workBUri = $"itn:works/{PART_B_ID}/beta";
        nodeWork = repository.GetNodeByUri(workBUri);
        Assert.NotNull(nodeWork);
        Assert.False(nodeWork.IsClass);
        Assert.Equal(2, nodeWork.SourceType);

        string workGUri = $"itn:works/{PART_G_ID}/gamma";
        nodeWork = repository.GetNodeByUri(workGUri);
        Assert.NotNull(nodeWork);
        Assert.False(nodeWork.IsClass);
        Assert.Equal(2, nodeWork.SourceType);

        // event node
        string eventUri = $"itn:events/{eventsA.Id}/version";
        Node? nodeEvent = repository.GetNodeByUri(eventUri);
        Assert.NotNull(nodeEvent);
        Assert.False(nodeEvent.IsClass);
        Assert.Equal(2, nodeEvent.SourceType);

        // triples
        DataPage<UriTriple> page = repository.GetTriples(new TripleFilter());
        Assert.Equal(8, page.Total);

        // itn:works/alpha a crm:E90_symbolic_object
        Assert.NotNull(page.Items.FirstOrDefault(t =>
            t.SubjectUri == workAUri &&
            t.PredicateUri == "rdf:type" &&
            t.ObjectUri == "crm:e90_symbolic_object"));

        // itn:works/beta a crm:E90_symbolic_object
        Assert.NotNull(page.Items.FirstOrDefault(t =>
            t.SubjectUri == workBUri &&
            t.PredicateUri == "rdf:type" &&
            t.ObjectUri == "crm:e90_symbolic_object"));

        // itn:works/gamma a crm:E90_symbolic_object
        Assert.NotNull(page.Items.FirstOrDefault(t =>
            t.SubjectUri == workGUri &&
            t.PredicateUri == "rdf:type" &&
            t.ObjectUri == "crm:e90_symbolic_object"));

        // itn:events/version a crm:E7_activity
        Assert.NotNull(page.Items.FirstOrDefault(t =>
            t.SubjectUri == eventUri &&
            t.PredicateUri == "rdf:type" &&
            t.ObjectUri == "crm:e7_activity"));

        // itn:events/version crm:P2_has_type itn:event-types/text.version
        Assert.NotNull(page.Items.FirstOrDefault(t =>
            t.SubjectUri == eventUri &&
            t.PredicateUri == "crm:p2_has_type" &&
            t.ObjectUri == "itn:event-types/text.version"));

        // itn:events/version P67_refers_to itn:works/alpha
        Assert.NotNull(page.Items.FirstOrDefault(t =>
            t.SubjectUri == eventUri &&
            t.PredicateUri == "crm:p67_refers_to" &&
            t.ObjectUri == workAUri));

        // itn:events/version P67_refers_to itn:works/beta
        Assert.NotNull(page.Items.FirstOrDefault(t =>
            t.SubjectUri == eventUri &&
            t.PredicateUri == "crm:p67_refers_to" &&
            t.ObjectUri == workBUri));

        // itn:events/version P67_refers_to itn:works/gamma
        Assert.NotNull(page.Items.FirstOrDefault(t =>
            t.SubjectUri == eventUri &&
            t.PredicateUri == "crm:p67_refers_to" &&
            t.ObjectUri == workGUri));
    }

    protected void DoUpdateGraph_Delete_Ok()
    {
        const string PERSON_ITEM_ID = "125b510b-6159-4396-b61c-5a4a40488ee8";
        const string PERSON_METADATA_ID = "d16a0f72-6ab7-4ab9-8220-4c05a07c3e10";

        const string WORK_ITEM_ID = "c1155e4d-45ed-4595-aee8-a11a8af2ad46";
        const string WORK_METADATA_ID = "2add838c-1b4e-4596-a939-8f3737355ab7";
        const string WORK_INFO_ID = "6caf3210-81b8-4cfd-a326-d42e28f60752";

        Reset();
        IGraphRepository repository = GetRepository();
        // load mappings
        string json = TestHelper.LoadResourceText("MappingsDoc.json");
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

        // person (Petrarch)
        IItem person = new Item
        {
            Id = PERSON_ITEM_ID,
            Title = "Petrarch",
            Description = "Petrarch description",
            FacetId = "person",
            SortKey = "petrarch",
            UserId = "zeus",
            CreatorId = "zeus"
        };
        // person metadata
        MetadataPart personMetadata = new()
        {
            Id = PERSON_METADATA_ID,
            ItemId = PERSON_ITEM_ID,
            UserId = "zeus",
            CreatorId = "zeus",
        };
        personMetadata.Metadata.Add(new Metadatum
        {
            Name = "eid",
            Value = "petrarch"
        });

        // work (alpha)
        IItem work = new Item
        {
            Id = WORK_ITEM_ID,
            Title = "Alpha work",
            Description = "Alpha work description",
            FacetId = "work",
            SortKey = "alpha",
            UserId = "zeus",
            CreatorId = "zeus"
        };
        // work metadata
        MetadataPart workMetadata = new()
        {
            Id = WORK_METADATA_ID,
            ItemId = WORK_ITEM_ID,
            UserId = "zeus",
            CreatorId = "zeus",
        };
        workMetadata.Metadata.Add(new Metadatum
        {
            Name = "eid",
            Value = "alpha"
        });
        // work info
        LiteraryWorkInfoPart workInfo = new()
        {
            Id = WORK_INFO_ID,
            ItemId = WORK_ITEM_ID,
            UserId = "zeus",
            CreatorId = "zeus",
            AuthorIds = new List<AssertedCompositeId>
            {
                new AssertedCompositeId
                {
                    Target = new PinTarget
                    {
                        ItemId = PERSON_ITEM_ID,
                        PartId = PERSON_METADATA_ID,
                        Label = "Petrarch",
                        Name = "eid",
                        Value = "petrarch"
                    }
                }
            },
            Genre = "epic",
            Languages = new List<string> { "it" },
            Titles = new List<AssertedTitle>
            {
                new AssertedTitle
                {
                    Language = "it",
                    Value = "L'alfa"
                }
            }
        };

        // create node for author
        MockItemEidMetadataSource metaSource = new(personMetadata.Id, "petrarch");
        GraphUpdater updater = new(repository)
        {
            MetadataSupplier = new MetadataSupplier()
                .AddMetadataSource(metaSource)
        };
        updater.Update(person, personMetadata);

        // create node for work
        metaSource.Set(workMetadata.Id, "alpha");
        updater.Update(work, workMetadata);

        // add work info
        updater.Update(work, workInfo);

        // person nodes
        string personUri = $"itn:persons/{PERSON_METADATA_ID}/petrarch";
        Node? nodePerson = repository.GetNodeByUri(personUri);
        Assert.NotNull(nodePerson);
        Assert.False(nodePerson.IsClass);
        Assert.Equal(2, nodePerson.SourceType);

        // work nodes
        string workUri = $"itn:works/{WORK_METADATA_ID}/alpha";
        Node? nodeWork = repository.GetNodeByUri(workUri);
        Assert.NotNull(nodeWork);
        Assert.False(nodeWork.IsClass);
        Assert.Equal(2, nodeWork.SourceType);

        // triples
        DataPage<UriTriple> page = repository.GetTriples(new TripleFilter());
        Assert.Equal(5, page.Total);

        // itn:works/alpha a crm:E90_symbolic_object
        Assert.NotNull(page.Items.FirstOrDefault(t =>
            t.SubjectUri == workUri &&
            t.PredicateUri == "rdf:type" &&
            t.ObjectUri == "crm:e90_symbolic_object"));

        // itn:persons/petrarch a crm:E21_person
        Assert.NotNull(page.Items.FirstOrDefault(t =>
            t.SubjectUri == personUri &&
            t.PredicateUri == "rdf:type" &&
            t.ObjectUri == "crm:e21_person"));

        // delete info
        repository.DeleteGraphSet(workInfo.Id);

        // work info triples were deleted
        page = repository.GetTriples(new TripleFilter());
        Assert.Equal(2, page.Total);

        // work info nodes were deleted
        // (we filter by null tag because some properties get added by mappings)
        Assert.Equal(2, repository.GetNodes(new NodeFilter
            { Sid = PERSON_METADATA_ID + "/petrarch", Tag = "" }).Total);
        Assert.Equal(2, repository.GetNodes(new NodeFilter
            { Sid = WORK_METADATA_ID + "/alpha", Tag = "" }).Total);
        Assert.Equal(0, repository.GetNodes(new NodeFilter
            { Sid = WORK_INFO_ID, Tag = "" }).Total);

        // add work info back
        updater.Update(work, workInfo);

        Assert.Equal(10, repository.GetNodes(new NodeFilter()).Total);

        page = repository.GetTriples(new TripleFilter());
        Assert.Equal(5, page.Total);
    }
    #endregion
}
