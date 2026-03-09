namespace ZigZag.Ast;

using Token;

public class ExpressionStatement: IExpression
{
    public Token Token { get; set; }
    public IExpression? Expression { get; set; }

    public string TokenLiteral()
    {
        return this.Token.Literal;
    }

    public void String()
    {
        if (this.Expression != null)
        {
            this.Expression.String();
        }
    }
}