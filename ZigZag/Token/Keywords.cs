using System.Collections;

namespace ZigZag.Token;

public class Keywords
{
    private static Dictionary<string, string> keywords = new Dictionary<string, string>()
    {
        { "let", Tokens.LET },
        { "fn", Tokens.FUNCTION },
        { "true", Tokens.TRUE },
        { "false", Tokens.FALSE },
        { "if", Tokens.IF },
        { "else", Tokens.ELSE },
        { "return", Tokens.RETURN },
    };

    public static string Lookup(string ident)
    {
        if (Keywords.keywords.TryGetValue(ident, out string? token))
        {
            return token;
        }

        return Tokens.IDENT;
    }
}