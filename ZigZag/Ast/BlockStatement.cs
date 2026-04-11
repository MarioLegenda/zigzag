namespace ZigZag.Ast;

using Token;

public class BlockStatement : IStatement
{
    public Token? Token { get; set; }
    public List<IStatement> Statements = new List<IStatement>();
    
    public string String()
    {
        string str = "";

        foreach (IStatement stmt in this.Statements)
        {
            str += stmt.String();
        }

        return str;
    }

    public string TokenLiteral()
    {
        return this.Token.Literal;
    }
}