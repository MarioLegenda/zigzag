namespace ZigZag.Tests;

using ZigZag.Ast;
using ZigZag.Lexer;
using Evaluator;
using Parser;
using Object;
using Xunit;
using Xunit.Abstractions;
class ExpectedEval
{
    public string input;
    public int expected;
    
    public ExpectedEval(string type, int expected)
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
    public void TestEvalIntegerExpression()
    {
        ExpectedEval[] tests =
        {
            new ExpectedEval("5", 5),
            new ExpectedEval("10", 10),
        };

        foreach (var test in tests)
        {
            IObject evaluated = testEval(test.input);
            Integer integer = (Integer)evaluated;
            Assert.NotNull(integer);
            
            Assert.Equal(test.expected, integer.Value);
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