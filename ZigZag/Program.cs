using ZigZag.Lexer;

using ZigZag.Token;

Lexer lexer = new Lexer("=+(){},;");

for (Token t = lexer.NextToken(); t.Type != Tokens.EOF ; t = lexer.NextToken())
{
    Console.WriteLine(t.Type);
}

