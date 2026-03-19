using ZigZag.Ast;
using ZigZag.Token;

namespace ZigZag.Parser;

public class GroupedExpressionParser: IPrefixParser
{
    public IExpression Parse(Token.Token token, Parser parser)
    {
        parser.NextToken();

        IExpression exp = parser.ParseExpression(ParsingTokens.LOWEST);

        if (!parser.expectPeek(Tokens.RPAREN))
        {
            return null;
        }

        return exp;
    }
}