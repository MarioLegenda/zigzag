namespace ZigZag.Ast;

using Token;

public class CallExpression: IExpression
{
    public Token Token { get; set; }
    
    public IExpression Function { get; set; }

    public List<IExpression> Arguments { get; set; } = new();
    
    public string String()
    {
        string str = "";
        
        List<string> prms = new();
        foreach (var param in Arguments)
        {
            prms.Add(param.String());
        }

        str += Function.String();
        str += "(";
        str += string.Join(", ", Arguments.Select(p => p.String()));
        str += ")";

        return str;
    }

    public string TokenLiteral()
    {
        return this.Token.Literal;
    }
}