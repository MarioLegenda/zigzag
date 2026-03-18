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

    public string String()
    {
        string str = "";
        str += this.Token.Literal + " ";
        if (this.ReturnValue != null)
        {
            str += this.ReturnValue.String();
        }

        str += ";";

        return str;
    }
}