namespace ZigZag.Ast;

using Token;

public class LetStatement : IStatement
{
    public Token Token { get; set; }
    public Identifier Name { get; set; }
    public IExpression? Value { get; set; }

    public string TokenLiteral()
    {
        return this.Token.Literal;
    }

    public string String()
    {
        string str = "";
        str += this.TokenLiteral() + " ";
        str += this.Name.String();
        str += " = ";

        if (this.Value != null)
        {
            str += this.Value.String();
        }

        str += ";";

        return str;
    }
}