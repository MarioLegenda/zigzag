
using Xunit.Sdk;

namespace ZigZag.Tests;

using ZigZag.Ast;
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
    public void TestIdentifierStatement()
    {
        string input = "foobar;";
        
        Parser p = new Parser(new Lexer(input));
        Program program = p.ParseProgram();

        Assert.NotNull(program);
        Assert.Empty(p.Errors());

        Assert.Single(program.Statements);

        ExpressionStatement expressionStatement = (ExpressionStatement)program.Statements[0];
        Identifier identifier = (Identifier)expressionStatement.Expression;
        
        Assert.Equal("foobar", identifier.Value);
        Assert.Equal("foobar", identifier.TokenLiteral());
    }
    
    [Fact]
    public void TestReturnStatement()
    {
        string input = @"
return 5;
return 10;
return 838383;
";
        
        Parser p = new Parser(new Lexer(input));
        Program program = p.ParseProgram();

        Assert.NotNull(program);
        Assert.Empty(p.Errors());
        
        Assert.Equal(3, program.Statements.Count);
        
        foreach (INode stmt in program.Statements)
        {
            Assert.IsType<ReturnStatement>(stmt);
            ReturnStatement returnStatement = (ReturnStatement)stmt;
            Assert.Equal("return", returnStatement.TokenLiteral());
        }
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
        Assert.Empty(p.Errors());
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