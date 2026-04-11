namespace ZigZag.Ast;

using Token;

public class FunctionLiteral: IExpression
{
    public Token Token { get; set; }

    public List<Identifier> Parameters { get; set; } = new();

    public BlockStatement Body { get; set; }

    public string String()
    {
        string str = "";

        List<string> prms = new();
        foreach (var param in Parameters)
        {
            prms.Add(param.String());
        }

        str += this.TokenLiteral();
        str += "(";
        str += string.Join(", ", Parameters.Select(p => p.String()));
        str += ")";
        str += this.Body.TokenLiteral();

        return str;
    }

    public string TokenLiteral()
    {
        return this.Token.Literal;
    }
}