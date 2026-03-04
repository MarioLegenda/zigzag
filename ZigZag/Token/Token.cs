namespace ZigZag.Token;

public class Token
{
    public Token(string type, char literal)
    {
        Type = type;
        Literal = literal;
    }
    
    public string Type { get; set; }
    
    public char Literal { get; set; }
}