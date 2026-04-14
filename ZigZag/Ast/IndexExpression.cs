namespace ZigZag.Ast;

using Token;

public class IndexExpression: IExpression
{
    public Token Token { get; set; }

    public IExpression Left { get; set; }
    
    public IExpression Index { get; set; }
    
    public string String()
    {
        string str = "";

        str += "(";
        str += this.Left.String();
        str += "[";
        str += this.Index.String();
        str += "]";
        str += ")";
        
        return str;
    }

    public string TokenLiteral()
    {
        return this.Token.Literal;
    }
}