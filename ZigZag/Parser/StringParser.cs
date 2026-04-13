namespace ZigZag.Parser;

using ZigZag.Ast;
using ZigZag.Token;

public class StringParser : IParser
{
    public IExpression Parse(Token token, Parser parser)
    {
        StringLiteral stringLiteral = new StringLiteral();
        stringLiteral.Token = token;
        stringLiteral.Value = token.Literal;

        return stringLiteral;
    }
}