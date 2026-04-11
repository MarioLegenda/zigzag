using ZigZag.Ast;
using ZigZag.Token;

namespace ZigZag.Parser;

public class FunctionParser: IParser
{
    public IExpression? Parse(Token.Token token, Parser parser)
    {
        FunctionLiteral functionLiteral = new FunctionLiteral();
        functionLiteral.Token = token;

        if (!parser.expectPeek(Tokens.LPAREN))
        {
            return null;
        }

        functionLiteral.Parameters = parseFunctionParameters(parser);

        if (!parser.expectPeek(Tokens.LBRACE))
        {
            return null;
        }

        functionLiteral.Body = parser.ParseBlockStatement();

        return functionLiteral;
    }

    private List<Identifier> parseFunctionParameters(Parser parser)
    {
        List<Identifier> identifiers = new();

        if (parser.peekTokenIs(Tokens.RPAREN))
        {
            parser.NextToken();
            return identifiers;
        }
        
        parser.NextToken();

        Identifier identifier = new Identifier(parser.CurrentToken(), parser.CurrentToken().Literal);
        identifiers.Add(identifier);

        while (parser.peekTokenIs(Tokens.COMMA))
        {
            parser.NextToken();
            parser.NextToken();
            
            Identifier ident = new Identifier(parser.CurrentToken(), parser.CurrentToken().Literal);
            identifiers.Add(ident);
        }

        if (!parser.expectPeek(Tokens.RPAREN))
        {
            return identifiers;
        }
        
        return identifiers;
    }
}