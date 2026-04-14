namespace ZigZag.Ast;

using Token;

public class ArrayLiteral: IExpression
{
    public Token Token { get; set; }
    
    public List<IExpression> Elements { get; set; }

    public string String()
    {
        string str = "";

        List<string> elems = new();
        foreach (var elem in this.Elements)
        {
            elems.Add(elem.String());
        }

        str += "[";
        str += string.Join(", ", elems);
        str += "]";

        return str;
    }

    public string TokenLiteral()
    {
        return this.Token.Literal;
    }
}