namespace ZigZag.Parser;

using ZigZag.Ast;
using ZigZag.Token;

public class BooleanParser: IPrefixParser
{
    public IExpression Parse(Token token, Parser parser)
    {
        Boolean b = new Boolean();
        b.Token = token;
        b.Value = parser.CurTokenIs(Tokens.TRUE);
        return b;
    }
}