namespace ZigZag.Tests;

using ZigZag.Ast;
using ZigZag.Lexer;
using Evaluator;
using Parser;
using Object;
using Xunit;
using Xunit.Abstractions;

class ExpectedEvalInteger
{
    public string input;
    public int expected;
    
    public ExpectedEvalInteger(string type, int expected)
    {
        input = type;
        this.expected = expected;
    }
}

class ExpectedEvalBangOperator
{
    public string input;
    public bool expected;
    
    public ExpectedEvalBangOperator(string type, bool expected)
    {
        input = type;
        this.expected = expected;
    }
}

class ExpectedEvalBoolean
{
    public string input;
    public bool expected;
    
    public ExpectedEvalBoolean(string type, bool expected)
    {
        input = type;
        this.expected = expected;
    }
}


public class EvaluationTest
{
    private readonly ITestOutputHelper _output;

    public EvaluationTest(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact]
    public void TestBangOperator()
    {
        ExpectedEvalBangOperator[] tests =
        {
            new ExpectedEvalBangOperator("!true", false),
            new ExpectedEvalBangOperator("!false", true),
            new ExpectedEvalBangOperator("!5", false),
            new ExpectedEvalBangOperator("!!true", true),
            new ExpectedEvalBangOperator("!!false", false),
            new ExpectedEvalBangOperator("!!5", true),
        };

        foreach (var test in tests)
        {
            IObject evaluated = testEval(test.input);
            Object.Boolean boolean = (Object.Boolean)evaluated;
            Assert.NotNull(boolean);
            
            Assert.Equal(test.expected, boolean.Value);
        }
    }

    [Fact]
    public void TestEvalIntegerExpression()
    {
        ExpectedEvalInteger[] tests =
        {
            new ExpectedEvalInteger("5", 5),
            new ExpectedEvalInteger("10", 10),
            new ExpectedEvalInteger("-5", -5),
            new ExpectedEvalInteger("-10", -10),
        };

        foreach (var test in tests)
        {
            IObject evaluated = testEval(test.input);
            Integer integer = (Integer)evaluated;
            Assert.NotNull(integer);
            
            Assert.Equal(test.expected, integer.Value);
        }
    }
    
    [Fact]
    public void TestEvalBooleanExpression()
    {
        ExpectedEvalBoolean[] tests =
        {
            new ExpectedEvalBoolean("true", true),
            new ExpectedEvalBoolean("false", false),
        };

        foreach (var test in tests)
        {
            IObject evaluated = testEval(test.input);
            Object.Boolean boolean = (Object.Boolean)evaluated;
            Assert.NotNull(boolean);
            
            Assert.Equal(test.expected, boolean.Value);
        }
    }

    private IObject testEval(string input)
    {
        Parser p = new Parser(new Lexer(input));
        Program program = p.ParseProgram();

        Assert.NotNull(program);
        Assert.Empty(p.Errors());

        return new Eval().Evaluate(program);
    }
}