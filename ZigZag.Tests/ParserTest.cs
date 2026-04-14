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

record ExpectedFunctionLiterals(string input, string expectedParams);

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
    public void TestParsingIndexExpressions()
    {
        string input = "myArray[1 + 1]";
        
        Parser parser = new Parser(new Lexer(input));
        Program program = parser.ParseProgram();
        
        Assert.NotNull(program);
        Assert.Empty(parser.Errors());
        
        Ast.ExpressionStatement expressionStatement = (Ast.ExpressionStatement)program.Statements[0];
        IndexExpression indexExpression = (IndexExpression)expressionStatement.Expression;
        Assert.NotNull(indexExpression);
        
        Identifier identifier = (Identifier)indexExpression.Left;
        
        Assert.Equal("myArray", identifier.Value);
        Assert.Equal("myArray", identifier.TokenLiteral());
    }

    [Fact]
    public void TestParsingArrayLiterals()
    {
        string input = "[1, 2 * 2, 3 + 3]";
        
        Parser parser = new Parser(new Lexer(input));
        Program program = parser.ParseProgram();
        
        Assert.NotNull(program);
        Assert.Empty(parser.Errors());
        
        Ast.ExpressionStatement expressionStatement = (Ast.ExpressionStatement)program.Statements[0];
        ArrayLiteral arrayLiteral = (ArrayLiteral)expressionStatement.Expression;
        Assert.NotNull(arrayLiteral);
        
        Assert.Equal(3, arrayLiteral.Elements.Count);
        
        testIntegerLiteral(arrayLiteral.Elements[0], 1);
        testInfixExpression(arrayLiteral.Elements[1], "*", 2, 2);
        testInfixExpression(arrayLiteral.Elements[2], "+", 3, 3);
    }
    
    [Fact]
    public void TestStringLiteralExpression()
    {
        string input = "\"hello world\"";
        
        Parser parser = new Parser(new Lexer(input));
        Program program = parser.ParseProgram();
        
        Assert.NotNull(program);
        Assert.Empty(parser.Errors());
        
        Ast.ExpressionStatement expressionStatement = (Ast.ExpressionStatement)program.Statements[0];
        StringLiteral stringLiteral = (StringLiteral)expressionStatement.Expression;
        Assert.NotNull(stringLiteral);
        
        Assert.Equal("hello world", stringLiteral.Value);
    }

    [Fact]
    public void TestCallExpressionParsing()
    {
        string input = "add(1, 2 * 3, 4 + 5);";
        
        Parser parser = new Parser(new Lexer(input));
        Program program = parser.ParseProgram();
        
        Assert.NotNull(program);
        Assert.Empty(parser.Errors());
        
        Ast.ExpressionStatement expressionStatement = (Ast.ExpressionStatement)program.Statements[0];
        CallExpression callExpression = (CallExpression)expressionStatement.Expression;
        Assert.NotNull(callExpression);
        
        Identifier identifier = (Identifier)callExpression.Function;
        
        Assert.Equal("add", identifier.Value);
        Assert.Equal("add", identifier.TokenLiteral());
        
        Assert.Equal(3, callExpression.Arguments.Count);

        testIntegerLiteral(callExpression.Arguments[0], 1);

        testInfixExpression(callExpression.Arguments[1], "*", 2, 3);
        testInfixExpression(callExpression.Arguments[2], "+", 4, 5);
    }

    [Fact]
    public void TestFunctionLiteralParsing()
    {
        string input = "fn(x, y) { x + y; }";
        
        Parser parser = new Parser(new Lexer(input));
        Program program = parser.ParseProgram();
            
        Assert.NotNull(program);
        Assert.Empty(parser.Errors());
        
        Ast.ExpressionStatement expressionStatement = (Ast.ExpressionStatement)program.Statements[0];
        FunctionLiteral fnExpression = (FunctionLiteral)expressionStatement.Expression;
        Assert.NotNull(fnExpression);
        
        Assert.Equal(2, fnExpression.Parameters.Count);
        
        Identifier identifierStatement1 = (Identifier)fnExpression.Parameters[0];
        Assert.Equal("x", identifierStatement1.Value);
        
        Identifier identifierStatement2 = (Identifier)fnExpression.Parameters[1];
        Assert.Equal("y", identifierStatement2.Value);

        Assert.Single(fnExpression.Body.Statements);
        
        _output.WriteLine(fnExpression.Body.String());
    }

    [Fact]
    public void TestFunctionParameterParsing()
    {
        ExpectedFunctionLiterals[] tests = new[]
        {
            new ExpectedFunctionLiterals("fn() {};", ""),
            new ExpectedFunctionLiterals("fn(x) {};", "x"),
            new ExpectedFunctionLiterals("fn(x, y, z) {};", "x, y, z"),
        };

        foreach (var input in tests)
        {
            Parser parser = new Parser(new Lexer(input.input));
            Program program = parser.ParseProgram();
            
            Assert.NotNull(program);
            Assert.Empty(parser.Errors());
            
            Ast.ExpressionStatement expressionStatement = (Ast.ExpressionStatement)program.Statements[0];
            FunctionLiteral fnExpression = (FunctionLiteral)expressionStatement.Expression;
            
            string expectedParams = input.expectedParams;
            string joined = string.Join(", ", fnExpression.Parameters.Select(p => p.TokenLiteral()));

            Assert.Equal(expectedParams, joined);
        }
    }

    [Fact]
    public void TestIfExpressions()
    {
        string input = "if (x < y) { x }";
        
        Parser parser = new Parser(new Lexer(input));
        Program program = parser.ParseProgram();
            
        Assert.NotNull(program);
        Assert.Empty(parser.Errors());
        
        Assert.Single(program.Statements);
        
        Ast.ExpressionStatement expressionStatement = (Ast.ExpressionStatement)program.Statements[0];
        IfExpression ifExpression = (IfExpression)expressionStatement.Expression;

        InfixExpression condition = (InfixExpression)ifExpression.Condition;
        Assert.Equal("<", condition.Operator);
        Identifier xIdentifier = (Identifier)condition.Left;
        Assert.Equal("x", xIdentifier.TokenLiteral());
        Identifier yIdentifier = (Identifier)condition.Right;
        Assert.Equal("y", yIdentifier.TokenLiteral());

        Assert.Single(ifExpression.Consequence.Statements);
        ExpressionStatement consequenceExpression1 = (ExpressionStatement)ifExpression.Consequence.Statements[0];
        Assert.Equal("x", consequenceExpression1.TokenLiteral());
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
            new ExpectedPrecendenceParsing("1 + (2 + 3) + 4", "((1 + (2 + 3)) + 4)"),
            new ExpectedPrecendenceParsing("(5 + 5) * 2", "((5 + 5) * 2)"),
            new ExpectedPrecendenceParsing("-(5 + 5)", "(-(5 + 5))"),
            new ExpectedPrecendenceParsing("!(true == true)", "(!(true == true))"),
            new ExpectedPrecendenceParsing("a + add(b * c) + d", "((a + add((b * c))) + d)"),
            new ExpectedPrecendenceParsing("add(a, b, 1, 2 * 3, 4 + 5, add(6, 7 * 8))", "add(a, b, 1, (2 * 3), (4 + 5), add(6, (7 * 8)))"),
            new ExpectedPrecendenceParsing("add(a + b + c * d / f + g)", "add((((a + b) + ((c * d) / f)) + g))"),
            new ExpectedPrecendenceParsing("a * [1, 2, 3, 4][b * c] * d", "((a * ([1, 2, 3, 4][(b * c)])) * d)"),
            new ExpectedPrecendenceParsing("add(a * b[2], b[1], 2 * [1, 2][1])", "add((a * (b[2])), (b[1]), (2 * ([1, 2][1])))"),
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

        testIdentifier(program.Statements[0], "foobar");
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

    private void testIdentifier(IStatement stmt, string ident)
    {
        ExpressionStatement expressionStatement = (ExpressionStatement)stmt;
        Identifier identifier = (Identifier)expressionStatement.Expression;
        
        Assert.Equal(ident, identifier.Value);
        Assert.Equal(ident, identifier.TokenLiteral());
    }

    private void testInfixExpression(IExpression exp, string op, int left, int right)
    {
        InfixExpression infixExpression = (InfixExpression)exp;
        Assert.NotNull(infixExpression);
        
        Assert.Equal(infixExpression.Operator, op);
        
        testIntegerLiteral(infixExpression.Right, right);
        testIntegerLiteral(infixExpression.Left, left);
    }
}