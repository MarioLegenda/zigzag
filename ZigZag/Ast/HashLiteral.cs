namespace ZigZag.Ast;

using Token;

public class HashLiteral: IExpression
{
    public Token Token { get; set; }
    
    public Dictionary<IExpression, IExpression> Pairs = new ();

    public string String()
    {
        string str = "";

        List<string> pairs = new();
        foreach (var (key, value) in this.Pairs)
        {
            pairs.Add(key.String() + ":" + value.String());
        }

        str += "{";
        str += string.Join(", ", pairs);
        str += "}";

        return str;
    }

    public string TokenLiteral()
    {
        return this.Token.Literal;
    }
}