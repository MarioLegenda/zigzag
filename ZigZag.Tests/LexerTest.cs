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
let result = add(five, ten);
!-/*5;
5 < 10 > 5;

if (5 < 10) {
    return true;
} else {
    return false;
}

10 == 10;
10 != 9;
";

        Expected[] expecteds =
        {
            new Expected(Tokens.LET, "let"),
            new Expected(Tokens.IDENT, "five"),
            new Expected(Tokens.ASSIGN, "="),
            new Expected(Tokens.INT, "5"),
            new Expected(Tokens.SEMICOLON, ";"),
            new Expected(Tokens.LET, "let"),
            new Expected(Tokens.IDENT, "ten"),
            new Expected(Tokens.ASSIGN, "="),
            new Expected(Tokens.INT, "10"),
            new Expected(Tokens.SEMICOLON, ";"),
            new Expected(Tokens.LET, "let"),
            new Expected(Tokens.IDENT, "add"),
            new Expected(Tokens.ASSIGN, "="),
            new Expected(Tokens.FUNCTION, "fn"), 
            new Expected(Tokens.LPAREN, "("),
            new Expected(Tokens.IDENT, "x"),
            new Expected(Tokens.COMMA, ","),
            new Expected(Tokens.IDENT, "y"),
            new Expected(Tokens.RPAREN, ")"),
            new Expected(Tokens.LBRACE, "{"),
            new Expected(Tokens.IDENT, "x"),
            new Expected(Tokens.PLUS, "+"),
            new Expected(Tokens.IDENT, "y"),
            new Expected(Tokens.SEMICOLON, ";"),
            new Expected(Tokens.RBRACE, "}"),
            new Expected(Tokens.SEMICOLON, ";"),
            new Expected(Tokens.LET, "let"),
            new Expected(Tokens.IDENT, "result"),
            new Expected(Tokens.ASSIGN, "="),
            new Expected(Tokens.IDENT, "add"), 
            new Expected(Tokens.LPAREN, "("),
            new Expected(Tokens.IDENT, "five"),
            new Expected(Tokens.COMMA, ","),
            new Expected(Tokens.IDENT, "ten"),
            new Expected(Tokens.RPAREN, ")"),
            new Expected(Tokens.SEMICOLON, ";"),
            new Expected(Tokens.BANG, "!"),
            new Expected(Tokens.MINUS, "-"),
            new Expected(Tokens.SLASH, "/"),
            new Expected(Tokens.ASTERIX, "*"),
            new Expected(Tokens.INT, "5"),
            new Expected(Tokens.SEMICOLON, ";"),
            new Expected(Tokens.INT, "5"),
            new Expected(Tokens.LT, "<"),
            new Expected(Tokens.INT, "10"),
            new Expected(Tokens.GT, ">"),
            new Expected(Tokens.INT, "5"),
            new Expected(Tokens.SEMICOLON, ";"),
            new Expected(Tokens.IF, "if"),
            new Expected(Tokens.LPAREN, "("),
            new Expected(Tokens.INT, "5"),
            new Expected(Tokens.LT, "<"),
            new Expected(Tokens.INT, "10"),
            new Expected(Tokens.RPAREN, ")"),
            new Expected(Tokens.LBRACE, "{"),
            new Expected(Tokens.RETURN, "return"),
            new Expected(Tokens.TRUE, "true"),
            new Expected(Tokens.SEMICOLON, ";"),
            new Expected(Tokens.RBRACE, "}"),
            new Expected(Tokens.ELSE, "else"),
            new Expected(Tokens.LBRACE, "{"),
            new Expected(Tokens.RETURN, "return"),
            new Expected(Tokens.FALSE, "false"),
            new Expected(Tokens.SEMICOLON, ";"),
            new Expected(Tokens.RBRACE, "}"),
            new Expected(Tokens.INT, "10"),
            new Expected(Tokens.EQ, "=="),
            new Expected(Tokens.INT, "10"),
            new Expected(Tokens.SEMICOLON, ";"),
            new Expected(Tokens.INT, "10"),
            new Expected(Tokens.NOT_EQ, "!="),
            new Expected(Tokens.INT, "9"),
            new Expected(Tokens.SEMICOLON, ";"),
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