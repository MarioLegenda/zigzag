using ZigZag.Lexer;

using ZigZag.Parser;

string user = Environment.UserName;
Console.WriteLine("Hello {0}, this is the ZigZag programming language.", user);
Console.WriteLine("Type your commands:");

Start(Console.In, Console.Out);

static void Start(TextReader input, TextWriter output)
{
    const string PROMPT = ">> ";
    
    while (true)
    {
        output.Write(PROMPT);

        string? line = input.ReadLine();
        if (line == null)
        {
            return;
        }
        
        Parser p = new Parser(new Lexer(line));
        ZigZag.Ast.Program program = p.ParseProgram();
        
        Console.WriteLine(program.TokenLiteral());
    }
}

