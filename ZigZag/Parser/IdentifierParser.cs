namespace ZigZag.Parser;

using ZigZag.Ast;
using Token;

public class IdentifierParser: IPrefixParser
{
    public IExpression Parse(Token token)
    {
        return new Identifier(token, token.Literal);
    }
}