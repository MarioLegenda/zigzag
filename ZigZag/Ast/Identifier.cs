namespace ZigZag.Ast;

using Token;

public class Identifier : IExpression
{
    private Token Token { get; set; }
    public string Value { get; set; }

    public Identifier(Token token, string value)
    {
        this.Token = token;
        this.Value = value;
    }

    public string TokenLiteral()
    {
        return this.Token.Literal;
    }
}