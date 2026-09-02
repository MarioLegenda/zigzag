using ZigZag.Lexer;

using ZigZag.Parser;
using ZigZag.Evaluator;
using ZigZag.Object;

if (args.Length == 0)
{
    Console.WriteLine("Please provide a file name.");
    return;
}

string filename = args[0];
string content = File.ReadAllText(filename);

Parser p = new Parser(new Lexer(content));
ZigZag.Ast.Program program = p.ParseProgram();

// Console.WriteLine(program.String());

IObject? evaluated = new Eval().Evaluate(program, new ObjectEnvironment());
if (evaluated is null)
{
    throw new ArgumentNullException();
}

//Console.WriteLine(evaluated.Inspect());

