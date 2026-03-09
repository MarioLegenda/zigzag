namespace ZigZag.Ast;

public interface INode
{
    public string TokenLiteral();
    public void String();
}

public interface IStatement : INode
{ }

public interface IExpression : IStatement
{ }