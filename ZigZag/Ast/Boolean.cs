namespace ZigZag.Ast;

using Token;

public class Boolean: IExpression
{
    public Token Token { get; set; }
    public bool Value;

    public string String()
    {
        return this.Token.Literal;
    }

    public string TokenLiteral()
    {
        return this.Token.Literal;
    }
}