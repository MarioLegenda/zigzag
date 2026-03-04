namespace ZigZag.Token;

public class Token
{
    public Token(string type, string literal)
    {
        Type = type;
        Literal = literal;
    }
    
    public string Type { get; set; }
    
    public string Literal { get; set; }
}