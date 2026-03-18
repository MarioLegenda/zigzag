namespace ZigZag.Ast;

using Token;

public class IntegerLiteral : IExpression
{
    public Token Token { get; set; }
    public int Value { get; set; }
    
    public string String()
    {
        return this.Token.Literal;
    }

    public string TokenLiteral()
    {
        return this.Token.Literal;
    }
}