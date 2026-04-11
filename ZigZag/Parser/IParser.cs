namespace ZigZag.Parser;

using ZigZag.Ast;
using Token;

public interface IParser
{
    public IExpression Parse(Token token, Parser parser);
}