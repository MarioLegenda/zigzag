namespace ZigZag.Ast;

using Token;

public class PrefixExpression : IExpression
{
    public Token Token;
    public string Operator;
    public IExpression Right;

    public string String()
    {
        string str = "(";
        str += this.Operator;
        str += this.Right.String();
        str += ")";

        return str;
    }

    public string TokenLiteral()
    {
        return this.Token.Literal;
    }
}