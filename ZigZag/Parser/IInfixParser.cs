namespace ZigZag.Parser;

using ZigZag.Ast;
using ZigZag.Token;

public interface IInfixParser
{
    public IExpression Parse(Token token, IExpression left, Parser parser);
}