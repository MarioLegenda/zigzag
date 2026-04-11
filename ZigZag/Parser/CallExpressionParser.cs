using ZigZag.Ast;
using ZigZag.Token;

namespace ZigZag.Parser;

public class CallExpressionParser: IInfixParser
{
    public IExpression? Parse(Token.Token token, IExpression fn, Parser parser)
    {
        CallExpression callExpression = new CallExpression();
        callExpression.Token = token;
        callExpression.Function = fn;

        List<IExpression> args = new();
        if (parser.peekTokenIs(Tokens.RPAREN))
        {
            parser.NextToken();
            return callExpression;
        }
        
        parser.NextToken();
        
        args.Add(parser.ParseExpression(ParsingTokens.LOWEST));
        
        while (parser.peekTokenIs(Tokens.COMMA))
        {
            parser.NextToken();
            parser.NextToken();
            
            args.Add(parser.ParseExpression(ParsingTokens.LOWEST));
        }

        if (!parser.expectPeek(Tokens.RPAREN))
        {
            return null;
        }

        callExpression.Arguments = args;

        return callExpression;
    }
}