using ZigZag.Ast;

namespace ZigZag.Parser;

public interface IInfixParser
{
    public IExpression Parse(IExpression expression);
}