namespace ZigZag.Ast;

using Token;

public class LetStatement : IStatement
{
    public Token Token { get; set; }
    public Identifier Name { get; set; }
    public IExpression Expression { get; set; }

    public string TokenLiteral()
    {
        return this.Token.Literal;
    }
}