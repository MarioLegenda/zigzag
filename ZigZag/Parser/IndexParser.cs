namespace ZigZag.Parser;

using ZigZag.Ast;
using ZigZag.Token;

public class IndexParser : IInfixParser
{
    public IExpression Parse(Token token, IExpression left, Parser parser)
    {
        IndexExpression indexExpression = new IndexExpression();
        indexExpression.Left = left;
        indexExpression.Token = token;
        
        parser.NextToken();

        indexExpression.Index = parser.ParseExpression(ParsingTokens.LOWEST);

        if (!parser.expectPeek(Tokens.RBRACKET))
        {
            return null;
        }

        return indexExpression;
    }
}