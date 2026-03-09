namespace ZigZag.Parser;

using ZigZag.Ast;
using ZigZag.Token;

public class IntegerParser : IPrefixParser
{
    public IExpression Parse(Token token)
    {
        IntegerLiteral integerLiteral = new IntegerLiteral();
        integerLiteral.Token = token;

        if (int.TryParse(token.Literal, out int value))
        {
            integerLiteral.Value = value;
        }
        else
        {
            throw new InvalidCastException($"Invalid cast for literal {token.Literal}");
        }

        return integerLiteral;
    }
}