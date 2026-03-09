namespace ZigZag.Ast;

public class Program : INode
{
    public List<IStatement> Statements = new();

    public string TokenLiteral()
    {
        Console.WriteLine(this.Statements.Count);
        if (this.Statements.Count > 0)
        {
            return this.Statements[0].TokenLiteral();
        }
        else
        {
            return "";
        }
    }

    public void String()
    {
        foreach (INode stmt in this.Statements)
        {
            stmt.String();
        }
    }
}