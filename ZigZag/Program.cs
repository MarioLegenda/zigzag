using ZigZag.Lexer;

using ZigZag.Token;

Lexer lexer = new Lexer(@"let five = 5;
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
");

string input = "-!*/<>";

foreach (char t in input)
{
    Console.WriteLine((byte)t);
}

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

        string line = input.ReadLine();
        if (line == null)
        {
            return;
        }

        var lexer = new Lexer(line);

        for (var tok = lexer.NextToken(); tok.Type != Tokens.EOF; tok = lexer.NextToken())
        {
            output.WriteLine(tok.Type, tok.Literal);
        }
    }
}

