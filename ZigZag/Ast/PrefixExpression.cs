namespace ZigZag.Ast;

using Token;

public class PrefixExpression : IExpression
{
    public Token Token;
    public string Operator;
    public IExpression Right;

    public void String()
    {
        Console.Write("(");
        Console.Write(this.Operator);
        this.Right.String();
        Console.Write(")");
    }

    public string TokenLiteral()
    {
        return this.Token.Literal;
    }
}