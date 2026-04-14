using ZigZag.Ast;
using ZigZag.Token;

namespace ZigZag.Parser;

public class ArrayParser: IParser
{
    public IExpression Parse(Token.Token token, Parser parser)
    {
        ArrayLiteral arrayLiteral = new ArrayLiteral();
        arrayLiteral.Token = token;

        List<IExpression> list = new();
        arrayLiteral.Elements = list;

        if (parser.peekTokenIs(Tokens.RBRACKET))
        {
            parser.NextToken();
            return arrayLiteral;
        }
        
        parser.NextToken();
        arrayLiteral.Elements.Add(parser.ParseExpression(ParsingTokens.LOWEST));

        while (parser.peekTokenIs(Tokens.COMMA))
        {
            parser.NextToken();
            parser.NextToken();
            arrayLiteral.Elements.Add(parser.ParseExpression(ParsingTokens.LOWEST));
        }

        if (!parser.expectPeek(Tokens.RBRACKET))
        {
            return null;
        }

        return arrayLiteral;
    }
}