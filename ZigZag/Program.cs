using ZigZag.Lexer;

using ZigZag.Token;

Lexer lexer = new Lexer(@"let five = 5;
let ten = 10;
let add = fn(x, y) {
x + y;
};
let result = add(five, ten);");

for (Token t = lexer.NextToken(); t.Type != Tokens.EOF ; t = lexer.NextToken())
{
    Console.WriteLine(t.Literal);
}

