namespace ZigZag.Ast;

using Token;

public class LetStatement : IStatement
{
    public Token Token { get; set; }
    public Identifier Name { get; set; }
    public IExpression? Value { get; set; }

    public string TokenLiteral()
    {
        return this.Token.Literal;
    }

    public void String()
    {
        Console.Write(this.TokenLiteral() + " ");
        this.Name.String();
        Console.Write(" = ");

        if (this.Value != null)
        {
            this.Value.String();
        }

        Console.Write(";");
    }
}