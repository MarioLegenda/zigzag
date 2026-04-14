using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;

namespace ZigZag.Tests;

using ZigZag.Ast;
using ZigZag.Lexer;
using Evaluator;
using Parser;
using Object;
using Xunit;
using Xunit.Abstractions;

class Expected<T>
{
    public string input;
    public T expected;
    
    public Expected(string type, T expected)
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
    public void TestArrayLiterals()
    {
        string input = "[1, 2 * 2, 3 + 3]";

        IObject? evaluated = testEval(input);
        Assert.NotNull(evaluated);

        Array array = (Array)evaluated;
        
        Assert.Equal(3, array.Elements.Count);
        
        testIntegerObject(array.Elements[0], 1);
        testIntegerObject(array.Elements[1], 4);
        testIntegerObject(array.Elements[2], 6);
    }

    [Fact]
    public void TestBuiltinFunctions()
    {
        Expected<int>[] passingTests =
        {
            new Expected<int>("len(\"\")", 0),
            new Expected<int>("len(\"four\")", 4),
            new Expected<int>("len(\"hello world\")", 11),
        };
        
        foreach (var test in passingTests)
        {
            IObject? evaluated = testEval(test.input);
            Assert.NotNull(evaluated);

            Integer integer = (Integer)evaluated;
            Assert.Equal(test.expected, integer.Value);
        }
        
        Expected<string>[] failingTests =
        {
            new Expected<string>("len(1)", "argument to `len` not supported, got INTEGER_OBJ"),
            new Expected<string>("len(\"one\", \"two\")", "wrong number of arguments. got=2, want=1"),
        };
        
        foreach (var test in failingTests)
        {
            IObject? evaluated = testEval(test.input);
            Assert.NotNull(evaluated);

            Error integer = (Error)evaluated;
            Assert.Equal(test.expected, integer.Message);
        }
    }

    [Fact]
    public void TestStringConcatenation()
    {
        string input = "\"Hello\" + \" \" + \"world\"";

        IObject evaluated = testEval(input);
        String str = (String)evaluated;
        Assert.NotNull(str);
        
        Assert.Equal("Hello world", str.Value);
    }

    [Fact]
    public void TestStringLiteral()
    {
        string input = "\"Hello world\"";

        IObject evaluated = testEval(input);
        String str = (String)evaluated;
        Assert.NotNull(str);
        
        Assert.Equal("Hello world", str.Value);
    }

    [Fact]
    public void TestClosures()
    {
        string input = @"let newAdder = fn(x) {
            fn(y) { x + y };
        };
        let addTwo = newAdder(2);
        addTwo(2);";

        IObject evaluated = testEval(input);
        Integer integer = (Integer)evaluated;
        Assert.NotNull(integer);
        
        Assert.Equal(4, integer.Value);
    }

    [Fact]
    public void TestFunctionApplication()
    {
        Expected<int>[] tests =
        {
            new Expected<int>("let identity = fn(x) { x; }; identity(5);", 5),
            new Expected<int>("let identity = fn(x) { return x; }; identity(5);", 5),
            new Expected<int>("let double = fn(x) { x * 2; }; double(5);", 10),
            new Expected<int>("let add = fn(x, y) { x + y; }; add(5, 5);", 10),
            new Expected<int>("let add = fn(x, y) { x + y; }; add(5 + 5, add(5, 5));", 20),
            new Expected<int>("fn(x) { x; }(5)", 5),
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
    public void TestFunctionObject()
    {
        string input = "fn(x) { x + 2; };";

        IObject evaluated = testEval(input);
        Function fn = (Function)evaluated;
        Assert.NotNull(fn);
        
        Assert.Single(fn.Parameters);
        Assert.Equal("x", fn.Parameters[0].String());

        string expectedBody = "(x + 2)";
        
        Assert.Equal(expectedBody, fn.Body.String());
    }

    [Fact]
    public void TestErrorHandling()
    {
        Expected<string>[] tests =
        {
            new Expected<string>("5 + true;", "type mismatch: INTEGER_OBJ + BOOLEAN_OBJ"),
            new Expected<string>("5 + true; 5;", "type mismatch: INTEGER_OBJ + BOOLEAN_OBJ"),
            new Expected<string>("-true", "unknown operator: -BOOLEAN_OBJ"),
            new Expected<string>("true + false;", "unknown operator: BOOLEAN_OBJ + BOOLEAN_OBJ"),
            new Expected<string>("5; true + false; 5", "unknown operator: BOOLEAN_OBJ + BOOLEAN_OBJ"),
            new Expected<string>("if (10 > 1) { true + false; }", "unknown operator: BOOLEAN_OBJ + BOOLEAN_OBJ"),
            new Expected<string>(@"if (10 > 1) {
if (10 > 1) {
return true + false;
}
return 1;
}", "unknown operator: BOOLEAN_OBJ + BOOLEAN_OBJ"),
        };
        
        foreach (var test in tests)
        {
            IObject evaluated = testEval(test.input);
            Error err = (Error)evaluated;
            Assert.NotNull(err);
            
            Assert.Equal(err.Message, test.expected);
        }
    }

    [Fact]
    public void TestReturnStatements()
    {
        Expected<int>[] tests =
        {
            new Expected<int>("return 10;", 10),
            new Expected<int>("return 10; 9;", 10),
            new Expected<int>("return 2 * 5; 9;", 10),
            new Expected<int>("9; return 2 * 5; 9;", 10),
            new Expected<int>(@"
if (10 > 1) {
    if (10 > 1) {
        return 10;
    }
return 1;
}", 10),
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
    public void TestIfElseExpressions()
    {
        Expected<int?>[] tests =
        {
            new Expected<int?>("if (true) { 10 }", 10),
            new Expected<int?>("if (false) { 10 }", null),
            new Expected<int?>("if (1 > 2) { 10 }", null),
            new Expected<int?>("if (1) { 10 }", 10),
            new Expected<int?>("if (1 < 2) { 10 }", 10),
            new Expected<int?>("if (1 > 2) { 10 } else { 20 }", 20),
            new Expected<int?>("if (1 < 2) { 10 } else { 20 }", 10),
        };
        
        foreach (var test in tests)
        {
            IObject evaluated = testEval(test.input);
            if (evaluated is Integer it)
            {
                _output.WriteLine("is integer");
                Integer integer = (Integer)evaluated;
                Assert.NotNull(integer);
            
                Assert.Equal(test.expected, integer.Value);
            }
            else
            {
                Assert.Equal("null", evaluated.Inspect());
            }
        }
    }
    
    [Fact]
    public void TestBangOperator()
    {
        Expected<bool>[] tests =
        {
            new Expected<bool>("!true", false),
            new Expected<bool>("!false", true),
            new Expected<bool>("!5", false),
            new Expected<bool>("!!true", true),
            new Expected<bool>("!!false", false),
            new Expected<bool>("!!5", true),
        };

        foreach (var test in tests)
        {
            IObject evaluated = testEval(test.input);
        }
    }

    [Fact]
    public void TestEvalIntegerExpression()
    {
        Expected<int>[] tests =
        {
            new Expected<int>("5", 5),
            new Expected<int>("10", 10),
            new Expected<int>("-5", -5),
            new Expected<int>("-10", -10),
            new Expected<int>("5 + 5 + 5 + 5 - 10", 10),
            new Expected<int>("2 * 2 * 2 * 2 * 2", 32),
            new Expected<int>("-50 + 100 + -50", 0),
            new Expected<int>("5 * 2 + 10", 20),
            new Expected<int>("5 + 2 * 10", 25),
            new Expected<int>("20 + 2 * -10", 0),
            new Expected<int>("50 / 2 * 2 + 10", 60),
            new Expected<int>("2 * (5 + 10)", 30),
            new Expected<int>("3 * 3 * 3 + 10", 37),
            new Expected<int>("3 * (3 * 3) + 10", 37),
            new Expected<int>("(5 + 10 * 2 + 15 / 3) * 2 + -10", 50),
        };

        foreach (var test in tests)
        {
            IObject? evaluated = testEval(test.input);
            Assert.NotNull(evaluated);

            testIntegerObject(evaluated, test.expected);
        }
    }
    
    [Fact]
    public void TestEvalBooleanExpression()
    {
        Expected<bool>[] tests =
        {
            new Expected<bool>("true", true),
            new Expected<bool>("false", false),
            new Expected<bool>("1 < 2", true),
            new Expected<bool>("1 > 2", false),
            new Expected<bool>("1 < 1", false),
            new Expected<bool>("1 > 1", false),
            new Expected<bool>("1 > 1", false),
            new Expected<bool>("1 == 1", true),
            new Expected<bool>("1 != 1", false),
            new Expected<bool>("1 == 2", false),
            new Expected<bool>("1 != 2", true),
            
            new Expected<bool>("true == true", true),
            new Expected<bool>("false == false", true),
            new Expected<bool>("true == false", false),
            new Expected<bool>("true != false", true),
            new Expected<bool>("false != true", true),
            new Expected<bool>("(1 < 2) == true", true),
            new Expected<bool>("(1 < 2) == false", false),
            new Expected<bool>("(1 > 2) == true", false),
            new Expected<bool>("(1 > 2) == false", true),
        };

        foreach (var test in tests)
        {
            IObject evaluated = testEval(test.input);
            Object.Boolean boolean = (Object.Boolean)evaluated;
            Assert.NotNull(boolean);
            
            Assert.Equal(test.expected, boolean.Value);
        }
    }

    private void testIntegerObject(IObject evaluated, int expected)
    {
        Integer integer = (Integer)evaluated;
        Assert.NotNull(integer);
            
        Assert.Equal(expected, integer.Value);
    }

    private IObject? testEval(string input)
    {
        Parser p = new Parser(new Lexer(input));
        Program program = p.ParseProgram();

        Assert.NotNull(program);
        Assert.Empty(p.Errors());

        return new Eval().Evaluate(program, new ObjectEnvironment());
    }
}