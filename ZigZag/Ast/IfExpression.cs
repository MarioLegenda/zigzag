namespace ZigZag.Ast;

using Token;

public class IfExpression: IExpression
{
    public Token Token;
    public IExpression Condition;
    public BlockStatement Consequence;
    public BlockStatement? Alternative;

    public string String()
    {
        string str = "";
        str += "if";
        str += this.Condition.String();
        str += " ";
        str += this.Consequence.String();

        if (this.Alternative != null)
        {
            str += "else ";
            str += this.Alternative.String();

        }

        return str;
    }

    public string TokenLiteral()
    {
        return this.Token.Literal;
    }
}