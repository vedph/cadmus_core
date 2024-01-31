using Xunit;

namespace Cadmus.Graph.Test;

public sealed class TripleObjectSupplierTest
{
    [Fact]
    public void Supply_NoLiteral_Unchanged()
    {
        Triple triple = new()
        {
            Id = 1,
            SubjectId = 1,
            PredicateId = 2,
            ObjectId = 3
        };

        TripleObjectSupplier.Supply(triple);

        Assert.Null(triple.ObjectLiteral);
        Assert.Null(triple.ObjectLiteralIx);
        Assert.Null(triple.LiteralLanguage);
        Assert.Null(triple.LiteralNumber);
        Assert.Null(triple.LiteralType);
    }

    [Fact]
    public void Supply_LiteralStringWithValues_Unchanged()
    {
        Triple triple = new()
        {
            Id = 1,
            SubjectId = 1,
            PredicateId = 2,
            ObjectLiteral = "Ciao, Mondo!",
            ObjectLiteralIx = "ciao mondo",
            LiteralLanguage = "it",
        };

        TripleObjectSupplier.Supply(triple, "en");

        // assert all values unchanged from triple
        Assert.Equal("Ciao, Mondo!", triple.ObjectLiteral);
        Assert.Equal("ciao mondo", triple.ObjectLiteralIx);
        Assert.Equal("it", triple.LiteralLanguage);
        Assert.Null(triple.LiteralNumber);
        Assert.Null(triple.LiteralType);
    }

    [Fact]
    public void Supply_LiteralFloatWithValues_Unchanged()
    {
        Triple triple = new()
        {
            Id = 1,
            SubjectId = 1,
            PredicateId = 2,
            ObjectLiteral = "1.5",
            ObjectLiteralIx = "1.5",
            LiteralLanguage = "it",
        };

        TripleObjectSupplier.Supply(triple, "en");

        // assert all values unchanged from triple
        Assert.Equal("1.5", triple.ObjectLiteral);
        Assert.Equal("1.5", triple.ObjectLiteralIx);
        Assert.Equal("it", triple.LiteralLanguage);
        Assert.Null(triple.LiteralNumber);
        Assert.Null(triple.LiteralType);
    }

    [Fact]
    public void Supply_LiteralImplicitString_Ok()
    {
        Triple triple = new()
        {
            Id = 1,
            SubjectId = 1,
            PredicateId = 2,
            ObjectLiteral = "Hello, World!",
        };

        TripleObjectSupplier.Supply(triple);

        Assert.Equal("Hello, World!", triple.ObjectLiteral);
        Assert.Equal("hello world", triple.ObjectLiteralIx);
        Assert.Null(triple.LiteralLanguage);
        Assert.Null(triple.LiteralNumber);
        Assert.Null(triple.LiteralType);
    }

    [Fact]
    public void Supply_LiteralImplicitAlphanumString_Ok()
    {
        Triple triple = new()
        {
            Id = 1,
            SubjectId = 1,
            PredicateId = 2,
            ObjectLiteral = "Hello, 123 World!",
        };

        TripleObjectSupplier.Supply(triple);

        Assert.Equal("Hello, 123 World!", triple.ObjectLiteral);
        Assert.Equal("hello 123 world", triple.ObjectLiteralIx);
        Assert.Null(triple.LiteralLanguage);
        Assert.Null(triple.LiteralNumber);
        Assert.Null(triple.LiteralType);
    }

    [Fact]
    public void Supply_LiteralExplicitString_Ok()
    {
        Triple triple = new()
        {
            Id = 1,
            SubjectId = 1,
            PredicateId = 2,
            ObjectLiteral = "Hello, World!",
            LiteralType = "xsd:string"
        };

        TripleObjectSupplier.Supply(triple);

        Assert.Equal("Hello, World!", triple.ObjectLiteral);
        Assert.Equal("hello world", triple.ObjectLiteralIx);
        Assert.Null(triple.LiteralLanguage);
        Assert.Null(triple.LiteralNumber);
        Assert.Equal("xsd:string", triple.LiteralType);
    }

    [Fact]
    public void Supply_LiteralStringWithLang_Ok()
    {
        Triple triple = new()
        {
            Id = 1,
            SubjectId = 1,
            PredicateId = 2,
            ObjectLiteral = "Hello, World!",
        };

        TripleObjectSupplier.Supply(triple, "en");

        Assert.Equal("Hello, World!", triple.ObjectLiteral);
        Assert.Equal("hello world", triple.ObjectLiteralIx);
        Assert.Equal("en", triple.LiteralLanguage);
        Assert.Null(triple.LiteralNumber);
        Assert.Null(triple.LiteralType);
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("True", true)]
    [InlineData("False", false)]
    [InlineData("1", true)]
    [InlineData("0", false)]
    public void Supply_LiteralBoolean_Ok(string text, bool expected)
    {
        Triple triple = new()
        {
            Id = 1,
            SubjectId = 1,
            PredicateId = 2,
            ObjectLiteral = text,
            LiteralType = "xsd:boolean"
        };

        TripleObjectSupplier.Supply(triple);

        Assert.Equal(text, triple.ObjectLiteral);
        Assert.Equal(text.ToLowerInvariant(), triple.ObjectLiteralIx);
        Assert.Null(triple.LiteralLanguage);
        Assert.Equal(expected? 1D : 0D, triple.LiteralNumber);
        Assert.Equal("xsd:boolean", triple.LiteralType);
    }

    [Theory]
    [InlineData("123", 123)]
    [InlineData("+123", 123)]
    [InlineData("0", 0)]
    [InlineData("00", 0)]
    [InlineData("-123", -123)]
    public void Supply_LiteralInt_Ok(string text, int expected)
    {
        Triple triple = new()
        {
            Id = 1,
            SubjectId = 1,
            PredicateId = 2,
            ObjectLiteral = text,
            LiteralType = "xsd:integer"
        };

        TripleObjectSupplier.Supply(triple);

        Assert.Equal(text, triple.ObjectLiteral);
        Assert.Equal(text, triple.ObjectLiteralIx);
        Assert.Null(triple.LiteralLanguage);
        Assert.Equal((double)expected, triple.LiteralNumber);
        Assert.Equal("xsd:integer", triple.LiteralType);
    }

    [Theory]
    [InlineData("123.5", 123.5f)]
    [InlineData("0", 0)]
    [InlineData("00", 0)]
    [InlineData("+123.5", 123.5f)]
    [InlineData("-123.5", -123.5f)]
    public void Supply_LiteralFloat_Ok(string text, float expected)
    {
        Triple triple = new()
        {
            Id = 1,
            SubjectId = 1,
            PredicateId = 2,
            ObjectLiteral = text,
            LiteralType = "xsd:float"
        };

        TripleObjectSupplier.Supply(triple);

        Assert.Equal(text, triple.ObjectLiteral);
        Assert.Equal(text, triple.ObjectLiteralIx);
        Assert.Null(triple.LiteralLanguage);
        Assert.Equal((double)expected, triple.LiteralNumber);
        Assert.Equal("xsd:float", triple.LiteralType);
    }
}
