namespace ZigZag.Ast;

public interface INode
{
    public string TokenLiteral();
}

public interface IStatement : INode
{
    
}

public interface IExpression : INode
{
    
}