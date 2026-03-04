using System.Diagnostics;

namespace ZigZag.Tests;

using ZigZag.Token;
using ZigZag.Lexer;
using Xunit;
using Xunit.Abstractions;

class Expected
{
    public string expectedType;
    public char expectedLiteral;
    
    public Expected(string type, char literal)
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
        string input = "=+(){},;";
        Expected[] expecteds =
        {
            new Expected(Tokens.ASSIGN, '='),
            new Expected(Tokens.PLUS, '+'),
            new Expected(Tokens.LPAREN, '('),
            new Expected(Tokens.RPAREN, ')'),
            new Expected(Tokens.LBRACE, '{'),
            new Expected(Tokens.RBRACE, '}'),
            new Expected(Tokens.COMMA, ','),
            new Expected(Tokens.SEMICOLON, ';'),
            new Expected(Tokens.EOF, ' ')
        };

        Lexer lexer = new Lexer(input);

        foreach (Expected t in expecteds)
        {
            Token token = lexer.NextToken();

            Assert.Equal(token.Type, t.expectedType);
            Assert.Equal(token.Literal, t.expectedLiteral);
        }
    }
}