namespace ZigZag.Ast;

using Token;

public class ReturnStatement: IStatement
{
    public Token Token { get; set; }
    public IExpression? ReturnValue { get; set; }

    public string TokenLiteral()
    {
        return this.Token.Literal;
    }

    public void String()
    {
        Console.Write(this.Token.Literal + " ");
        if (this.ReturnValue != null)
        {
            this.ReturnValue.String();
        }
        
        Console.Write(";");
    }
}