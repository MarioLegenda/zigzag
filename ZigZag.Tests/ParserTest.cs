
namespace ZigZag.Tests;

using ZigZag.Ast;
using ZigZag.Token;
using ZigZag.Lexer;
using Parser;
using Xunit;
using Xunit.Abstractions;

public class ParserTest
{
    private readonly ITestOutputHelper _output;

    public ParserTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void TestLetStatements()
    {
        string input = @"
let x = 5;
let y = 10;
let foobar = 838383;
";

        Parser p = new Parser(new Lexer(input));
        Program program = p.ParseProgram();

        Assert.NotNull(program);
        Assert.Equal(3, program.Statements.Count);

        string[] tests =
        {
            "x",
            "y",
            "foobar",
        };

        for (int i = 0; i < tests.Length; i++)
        {
            string t = tests[i];
            IStatement stmt = program.Statements[i];
            testLetStatement(stmt, t);
        }
    }

    private void testLetStatement(IStatement stmt, string name)
    {
        Assert.Equal("let", stmt.TokenLiteral());
        LetStatement letStatement = (LetStatement)stmt;
        
        Assert.Equal(name, letStatement.Name.Value);
        Assert.Equal(name, letStatement.Name.TokenLiteral());
        
    }
}