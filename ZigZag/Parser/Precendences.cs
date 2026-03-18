
namespace ZigZag.Parser;

using Token;
using ZigZag.Token;

class Precendences
{
    public static readonly Dictionary<string, ParsingTokens> precedences = new()
    {
        { Tokens.EQ, ParsingTokens.EQUALS },
        { Tokens.NOT_EQ, ParsingTokens.EQUALS },
        { Tokens.LT, ParsingTokens.LESSGREATER },
        { Tokens.GT, ParsingTokens.LESSGREATER },
        { Tokens.PLUS, ParsingTokens.SUM },
        { Tokens.MINUS, ParsingTokens.SUM },
        { Tokens.SLASH, ParsingTokens.PRODUCT },
        { Tokens.ASTERIX, ParsingTokens.PRODUCT }
    };
}