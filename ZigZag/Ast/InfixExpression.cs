namespace ZigZag.Parser;

using ZigZag.Ast;
using Token;

public class InfixExpression: IExpression
{        
    public Token Token { get; set; }
    public IExpression Left { get; set; }
    public string Operator;
    public IExpression Right { get; set; }

    public string String()
    {
        string str = "";
        str += "(";
        str += this.Left.String();
        str += " " + this.Operator + " ";
        str += this.Right.String();
        str += ")";

        return str;
    }

    public string TokenLiteral()
    {
        return this.Token.Literal;
    }
}