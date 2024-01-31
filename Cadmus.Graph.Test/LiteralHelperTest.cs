using Xunit;

namespace Cadmus.Graph.Test;

public sealed class LiteralHelperTest
{
    [Fact]
    public void ConvertToBoolean()
    {
        Assert.True(LiteralHelper.ConvertToBoolean("1"));
        Assert.True(LiteralHelper.ConvertToBoolean("true"));
        Assert.False(LiteralHelper.ConvertToBoolean("0"));
        Assert.False(LiteralHelper.ConvertToBoolean("false"));
        Assert.False(LiteralHelper.ConvertToBoolean("x"));
    }

    [Fact]
    public void AdjustLiteral_Boolean()
    {
        Triple triple = new()
        {
            ObjectLiteral = "\"1\"^^xs:boolean"
        };
        LiteralHelper.AdjustLiteral(triple);
        Assert.Equal("1", triple.ObjectLiteral);
        Assert.Equal(1, triple.LiteralNumber);

        triple = new()
        {
            ObjectLiteral = "\"0\"^^xs:boolean"
        };
        LiteralHelper.AdjustLiteral(triple);
        Assert.Equal("0", triple.ObjectLiteral);
        Assert.Equal(0, triple.LiteralNumber);

        triple = new()
        {
            ObjectLiteral = "\"true\"^^xs:boolean"
        };
        LiteralHelper.AdjustLiteral(triple);
        Assert.Equal("true", triple.ObjectLiteral);
        Assert.Equal(1, triple.LiteralNumber);

        triple = new()
        {
            ObjectLiteral = "\"false\"^^xs:boolean"
        };
        LiteralHelper.AdjustLiteral(triple);
        Assert.Equal("false", triple.ObjectLiteral);
        Assert.Equal(0, triple.LiteralNumber);
    }

    [Fact]
    public void AdjustLiteral_IntegerNumbers()
    {
        Triple triple = new()
        {
            ObjectLiteral = "\"123\"^^xsd:int"
        };
        LiteralHelper.AdjustLiteral(triple);
        Assert.Equal("123", triple.ObjectLiteral);
        Assert.Equal(123, triple.LiteralNumber);

        triple = new()
        {
            ObjectLiteral = "\"123\"^^xsd:integer"
        };
        LiteralHelper.AdjustLiteral(triple);
        Assert.Equal("123", triple.ObjectLiteral);
        Assert.Equal(123, triple.LiteralNumber);

        triple = new()
        {
            ObjectLiteral = "\"123\"^^xsd:long"
        };
        LiteralHelper.AdjustLiteral(triple);
        Assert.Equal("123", triple.ObjectLiteral);
        Assert.Equal(123, triple.LiteralNumber);

        triple = new()
        {
            ObjectLiteral = "\"123\"^^xsd:short"
        };
        LiteralHelper.AdjustLiteral(triple);
        Assert.Equal("123", triple.ObjectLiteral);
        Assert.Equal(123, triple.LiteralNumber);

        triple = new()
        {
            ObjectLiteral = "\"123\"^^xsd:byte"
        };
        LiteralHelper.AdjustLiteral(triple);
        Assert.Equal("123", triple.ObjectLiteral);
        Assert.Equal(123, triple.LiteralNumber);

        triple = new()
        {
            ObjectLiteral = "\"123\"^^xsd:unsignedInt"
        };
        LiteralHelper.AdjustLiteral(triple);
        Assert.Equal("123", triple.ObjectLiteral);
        Assert.Equal(123, triple.LiteralNumber);

        triple = new()
        {
            ObjectLiteral = "\"123\"^^xsd:unsignedLong"
        };
        LiteralHelper.AdjustLiteral(triple);
        Assert.Equal("123", triple.ObjectLiteral);
        Assert.Equal(123, triple.LiteralNumber);

        triple = new()
        {
            ObjectLiteral = "\"123\"^^xsd:unsignedShort"
        };
        LiteralHelper.AdjustLiteral(triple);
        Assert.Equal("123", triple.ObjectLiteral);
        Assert.Equal(123, triple.LiteralNumber);

        triple = new()
        {
            ObjectLiteral = "\"123\"^^xsd:unsignedByte"
        };
        LiteralHelper.AdjustLiteral(triple);
        Assert.Equal("123", triple.ObjectLiteral);
        Assert.Equal(123, triple.LiteralNumber);
    }

    [Fact]
    public void AdjustLiteral_FloatNumbers()
    {
        Triple triple = new()
        {
            ObjectLiteral = "\"123.45\"^^xsd:float"
        };
        LiteralHelper.AdjustLiteral(triple);
        Assert.Equal("123.45", triple.ObjectLiteral);
        Assert.Equal(123.45, triple.LiteralNumber);

        triple = new()
        {
            ObjectLiteral = "\"123.45\"^^xsd:double"
        };
        LiteralHelper.AdjustLiteral(triple);
        Assert.Equal("123.45", triple.ObjectLiteral);
        Assert.Equal(123.45, triple.LiteralNumber);
    }
}
