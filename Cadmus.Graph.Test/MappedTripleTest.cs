using Xunit;

namespace Cadmus.Graph.Test;

public sealed class MappedTripleTest
{
    [Fact]
    public void Parse_Null_Null()
    {
        MappedTriple? triple = MappedTriple.Parse(null);
        Assert.Null(triple);
    }

    [Fact]
    public void Parse_Empty_Null()
    {
        MappedTriple? triple = MappedTriple.Parse("");
        Assert.Null(triple);
    }

    [Fact]
    public void Parse_S_Null()
    {
        MappedTriple? triple = MappedTriple.Parse("x:guy");
        Assert.Null(triple);
    }

    [Fact]
    public void Parse_SP_Null()
    {
        MappedTriple? triple = MappedTriple.Parse("x:guy a");
        Assert.Null(triple);
    }

    [Fact]
    public void Parse_SPO_Ok()
    {
        MappedTriple? triple = MappedTriple.Parse("x:guy a foaf:Person");
        Assert.NotNull(triple);
        Assert.Equal("x:guy", triple!.S);
        Assert.Equal("a", triple!.P);
        Assert.Equal("foaf:Person", triple!.O);
        Assert.Null(triple!.OL);
    }

    [Fact]
    public void ToString_SPO_Ok()
    {
        MappedTriple triple = new()
        {
            S = "x:guy",
            P = "a",
            O = "foaf:Person"
        };
        Assert.Equal("x:guy a foaf:Person", triple.ToString());
    }

    [Fact]
    public void ToString_SPOL_Ok()
    {
        MappedTriple triple = new()
        {
            S = "x:birth_of_Petrarch",
            P = "crm:P4_has_time-span",
            OL = "\"1304\"^^xs:int"
        };
        Assert.Equal("x:birth_of_Petrarch crm:P4_has_time-span \"1304\"^^xs:int",
            triple.ToString());
    }
}
