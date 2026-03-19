
using Xunit.Sdk;

namespace ZigZag.Tests;

using ZigZag.Ast;
using ZigZag.Lexer;
using Parser;
using Xunit;
using Xunit.Abstractions;

class ExpectedPrefixExpression
{
    public string input;
    public string theOperator;
    public int integerValue;

    public ExpectedPrefixExpression(string input, string op, int integerValue)
    {
        this.input = input;
        this.theOperator = op;
        this.integerValue = integerValue;
    }
}


record ExpectedPrecendenceParsing(string input, string expected);

record ExpectedBooleanParsing(string input, string op, bool expected);

public class ParserTest
{
    private readonly ITestOutputHelper _output;

    public ParserTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void TestBooleanParsing()
    {
        ExpectedBooleanParsing[] tests = new[]
        {
            new ExpectedBooleanParsing("!true", "!", true),
            new ExpectedBooleanParsing("!false", "!", false),
        };
        
        foreach (ExpectedBooleanParsing exp in tests)
        {
            Parser parser = new Parser(new Lexer(exp.input));
            Program program = parser.ParseProgram();
            
            Assert.NotNull(program);
            Assert.Empty(parser.Errors());
            
            Assert.Single(program.Statements);
            
            Ast.ExpressionStatement expressionStatement = (Ast.ExpressionStatement)program.Statements[0];
            Ast.PrefixExpression prefixExpression = (Ast.PrefixExpression)expressionStatement.Expression;

            Ast.Boolean boolean = (Boolean)prefixExpression.Right;
            if (boolean.Value)
            {
                Assert.True(boolean.Value);
            }
            else
            {
                Assert.False(boolean.Value);
            }
        }
    }

    [Fact]
    public void TestOperatorPrecedenceParsing()
    {
        var tests = new[]
        {
            new ExpectedPrecendenceParsing("-a * b", "((-a) * b)"),
            new ExpectedPrecendenceParsing("!-a", "(!(-a))"),
            new ExpectedPrecendenceParsing("a + b + c", "((a + b) + c)"),
            new ExpectedPrecendenceParsing("a + b - c", "((a + b) - c)"),
            new ExpectedPrecendenceParsing("a * b * c", "((a * b) * c)"),
            new ExpectedPrecendenceParsing("a * b / c", "((a * b) / c)"),
            new ExpectedPrecendenceParsing("a + b / c", "(a + (b / c))"),
            new ExpectedPrecendenceParsing("a + b * c + d / e - f", "(((a + (b * c)) + (d / e)) - f)"),
            new ExpectedPrecendenceParsing("3 + 4; -5 * 5", "(3 + 4)((-5) * 5)"),
            new ExpectedPrecendenceParsing("5 > 4 == 3 < 4", "((5 > 4) == (3 < 4))"),
            new ExpectedPrecendenceParsing("5 < 4 != 3 > 4", "((5 < 4) != (3 > 4))"),
            new ExpectedPrecendenceParsing("3 + 4 * 5 == 3 * 1 + 4 * 5", "((3 + (4 * 5)) == ((3 * 1) + (4 * 5)))"),
            new ExpectedPrecendenceParsing("3 + 4 * 5 == 3 * 1 + 4 * 5", "((3 + (4 * 5)) == ((3 * 1) + (4 * 5)))"),
            new ExpectedPrecendenceParsing("true", "true"),
            new ExpectedPrecendenceParsing("false", "false"),
            new ExpectedPrecendenceParsing("3 > 5 == false", "((3 > 5) == false)"),
            new ExpectedPrecendenceParsing("3 < 5 == true", "((3 < 5) == true)"),
        };

        foreach (ExpectedPrecendenceParsing test in tests)
        {
            Parser parser = new Parser(new Lexer(test.input));
            Program program = parser.ParseProgram();
            
            Assert.NotNull(program);
            Assert.Empty(parser.Errors());
            
            string actual = program.String();
            
            Assert.Equal(actual, test.expected);
        }
    }

    [Fact]
    public void TestParsingPrefixExpressions()
    {
        ExpectedPrefixExpression[] expecteds =
        {
            new ExpectedPrefixExpression("!5", "!", 5),
            new ExpectedPrefixExpression("-15", "-", 15),
        };

        foreach (ExpectedPrefixExpression exp in expecteds)
        {
            Parser parser = new Parser(new Lexer(exp.input));
            Program program = parser.ParseProgram();
            
            Assert.NotNull(program);
            Assert.Empty(parser.Errors());
            
            Assert.Single(program.Statements);
            
            Ast.ExpressionStatement expressionStatement = (Ast.ExpressionStatement)program.Statements[0];
            Ast.PrefixExpression prefixExpression = (Ast.PrefixExpression)expressionStatement.Expression;
            
            Assert.Equal(prefixExpression.Operator, exp.theOperator);
            testIntegerLiteral(prefixExpression.Right, exp.integerValue);
        }
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

    private void testIntegerLiteral(IExpression expression, int integerValue)
    {
        IntegerLiteral literal = (IntegerLiteral)expression;
        Assert.Equal(literal.Value, integerValue);
        Assert.Equal(literal.TokenLiteral(), integerValue + "");
    }

    private void testBooleanLiteral(IExpression exp, bool value)
    {
        
    }
}