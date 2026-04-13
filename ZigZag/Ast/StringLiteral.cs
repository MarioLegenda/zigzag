namespace ZigZag.Ast;

using Token;

public class StringLiteral : IExpression
{
    public Token Token { get; set; }
    public string Value { get; set; }
    
    public string String()
    {
        return this.Token.Literal;
    }

    public string TokenLiteral()
    {
        return this.Token.Literal;
    }
}