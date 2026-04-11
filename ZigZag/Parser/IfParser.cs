using ZigZag.Token;

namespace ZigZag.Parser;

using ZigZag.Ast;

public class IfParser: IParser
{
    public IExpression Parse(Token.Token token, Parser parser)
    {
        IfExpression exp = new IfExpression();
        exp.Token = token;

        if (!parser.expectPeek(Tokens.LPAREN))
        {
            return null;
        }
        
        parser.NextToken();
        exp.Condition = parser.ParseExpression(ParsingTokens.LOWEST);

        if (!parser.expectPeek(Tokens.RPAREN))
        {
            return null;
        }
        
        if (!parser.expectPeek(Tokens.LBRACE))
        {
            return null;
        }

        exp.Consequence = parser.ParseBlockStatement();

        return exp;
    }
}