namespace ZigZag.Parser;

using ZigZag.Ast;
using Token;

public interface IPrefixParser
{
    public IExpression Parse(Token token);
}