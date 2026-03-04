using System.Diagnostics;

namespace ZigZag.Tests;

using ZigZag.Token;
using ZigZag.Lexer;
using Xunit;
using Xunit.Abstractions;

class Expected
{
    public string expectedType;
    public string expectedLiteral;
    
    public Expected(string type, string literal)
    {
        expectedType = type;
        expectedLiteral = literal;
    }
}

public class LexterTest
{
    private readonly ITestOutputHelper _output;

    public LexterTest(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact]
    public void TestNextToken()
    {
        string input = @"let five = 5;
let ten = 10;
let add = fn(x, y) {
x + y;
};
let result = add(five, ten);";

        Expected[] expecteds =
        {
            new Expected(Tokens.LET, "let"),
        };

        Lexer lexer = new Lexer(input);

        foreach (Expected t in expecteds)
        {
            Token token = lexer.NextToken();

            Assert.Equal(token.Type, t.expectedType);
            Assert.Equal(token.Literal.ToString(), t.expectedLiteral);
        }
    }
}