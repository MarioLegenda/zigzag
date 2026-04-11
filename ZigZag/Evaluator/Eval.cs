using System.Runtime.InteropServices.JavaScript;

namespace ZigZag.Evaluator;

using Object;
using Ast;

public class Eval
{
    public IObject Evaluate(INode node)
    {
        if (node is Program p)
        {
            return evalStatements(p.Statements.ToArray());
        }

        if (node is Ast.Boolean b)
        {
            return new Object.Boolean(b.Value);
        } 

        if (node is ExpressionStatement exp && exp.Expression is not null)
        {
            return new Eval().Evaluate(exp.Expression);
        }
        
        if (node is IntegerLiteral it)
        {
            return new Integer(it.Value);
        }

        return null;
    }

    private IObject evalStatements(IStatement[] stmts)
    {
        IObject result = null;
        foreach (var stmt in stmts)
        {
            result = new Eval().Evaluate(stmt);
        }

        return result;
    }
}