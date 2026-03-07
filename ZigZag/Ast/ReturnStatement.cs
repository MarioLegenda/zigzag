namespace ZigZag.Ast;

using Token;

public class ReturnStatement: IStatement
{
    public Token Token { get; set; }
    public IExpression ReturnValue { get; set; }

    public string TokenLiteral()
    {
        return this.Token.Literal;
    }
}