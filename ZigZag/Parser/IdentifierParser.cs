namespace ZigZag.Parser;

using ZigZag.Ast;
using Token;

public class IdentifierParser: IParser
{
    public IExpression Parse(Token token, Parser parser)
    {
        return new Identifier(token, token.Literal);
    }
}