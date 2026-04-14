using ZigZag.Ast;
using ZigZag.Token;

namespace ZigZag.Parser;

public class HashParser: IParser
{
    public IExpression Parse(Token.Token token, Parser parser)
    {
        HashLiteral hashLiteral = new HashLiteral();
        hashLiteral.Token = token;
        hashLiteral.Pairs = new Dictionary<IExpression, IExpression>();

        while (!parser.peekTokenIs(Tokens.RBRACE))
        {
            parser.NextToken();
            IExpression key = parser.ParseExpression(ParsingTokens.LOWEST);

            if (!parser.expectPeek(Tokens.COLON))
            {
                return null;
            }
            
            parser.NextToken();

            IExpression value = parser.ParseExpression(ParsingTokens.LOWEST);

            hashLiteral.Pairs[key] = value;

            if (!parser.peekTokenIs(Tokens.RBRACE) && !parser.expectPeek(Tokens.COMMA))
            {
                return null;
            }
        }

        if (!parser.expectPeek(Tokens.RBRACE))
        {
            return null;
        }

        return hashLiteral;
    }
}