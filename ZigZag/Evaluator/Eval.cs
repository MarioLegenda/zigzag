using System.Runtime.InteropServices.JavaScript;
using System.Xml.Xsl;
using ZigZag.Parser;

namespace ZigZag.Evaluator;

using Object;
using Ast;

public class Eval
{
    public IObject? Evaluate(INode node)
    {
        if (node is Program p)
        {
            return evalProgram(p.Statements);
        }

        if (node is ReturnStatement rt)
        {
            IObject val = new Eval().Evaluate(rt.ReturnValue);
            return new Object.ReturnValue(val);
        }

        if (node is Ast.Boolean b)
        {
            return new Object.Boolean(b.Value);
        }

        if (node is Ast.BlockStatement bl)
        {
            return evalBlockStatement(bl.Statements);
        }

        if (node is Ast.IfExpression ifexp)
        {
            return evalIfExpression(ifexp);
        }

        if (node is PrefixExpression pe)
        {
            IObject? right = new Eval().Evaluate(pe.Right);
            if (right != null)
            {
                return evalPrefixExpression(pe.Operator, right);
            }
        }

        if (node is InfixExpression ife)
        {
            IObject? left = new Eval().Evaluate(ife.Left);
            IObject? right = new Eval().Evaluate(ife.Right);

            if (left != null && right != null)
            {            
                return evalInfixExpression(ife.Operator, left, right);
            }
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

    private IObject evalIfExpression(Ast.IfExpression ifexp)
    {
        IObject? condition = new Eval().Evaluate(ifexp.Condition);

        if (isTruthy(condition))
        {
            return new Eval().Evaluate(ifexp.Consequence);
        }
        else if (ifexp.Alternative != null)
        {
            return new Eval().Evaluate(ifexp.Alternative);
        }
        else
        {
            return new Null();
        }
    }

    private bool isTruthy(IObject? obj)
    {
        if (obj is null)
        {
            return false;
        }
        
        if (obj.Type() == ObjectTypeEnum.BOOLEAN_OBJ)
        {
            Object.Boolean b = (Object.Boolean)obj;
            if (b.Value)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        if (obj.Type() == ObjectTypeEnum.NULL_OBJ)
        {
            return false;
        }

        return true;
    }

    private IObject evalPrefixExpression(string op, IObject right)
    {
        switch (op)
        {
            case "!":
                return this.evalBangOperatorExpression(right);
            case "-":
                return this.evalMinusPrefixOperatorExpression(right);
            default:
                return new Null();
        }
    }

    private IObject evalMinusPrefixOperatorExpression(IObject right)
    {
        if (right.Type() != ObjectTypeEnum.INTEGER_OBJ)
        {
            return new Null();
        }

        Integer value = (Integer)right;
        return new Integer(-value.Value);
    }

    private IObject evalBangOperatorExpression(IObject right)
    {
        switch (right.Inspect())
        {
            case "true":
                return new Object.Boolean(false);
            case "false":
                return new Object.Boolean(true);
            case "null":
                return new Object.Boolean(true);
            default:
                return new Object.Boolean(false);
        }
    }

    private IObject evalInfixExpression(string op, IObject left, IObject right)
    {
        if (left.Type() == ObjectTypeEnum.INTEGER_OBJ && right.Type() == ObjectTypeEnum.INTEGER_OBJ)
        {
            return evalIntegerInfixExpression(op, left, right);
        }

        if (op == "==" && left is Object.Boolean)
        {
            Object.Boolean l = (Object.Boolean)left;
            Object.Boolean r = (Object.Boolean)right;
            return nativeBoolToBooleanObject(l.Value == r.Value);
        }

        if (op == "!=")
        {
            Object.Boolean l = (Object.Boolean)left;
            Object.Boolean r = (Object.Boolean)right;
            return nativeBoolToBooleanObject(l.Value != r.Value);
        }
        
        if (op == "==" && left is Object.Integer)
        {
            Object.Integer l = (Object.Integer)left;
            Object.Integer r = (Object.Integer)right;
            return nativeBoolToBooleanObject(l.Value == r.Value);
        }

        if (op == "!=")
        {
            Object.Integer l = (Object.Integer)left;
            Object.Integer r = (Object.Integer)right;
            return nativeBoolToBooleanObject(l.Value != r.Value);
        }

        return new Null();
    }

    private IObject evalIntegerInfixExpression(string op, IObject left, IObject right)
    {
        Integer l = (Integer)left;
        Integer r = (Integer)right;

        switch (op)
        {
            case "<":
                return nativeBoolToBooleanObject(l.Value < r.Value);
            case ">":
                return nativeBoolToBooleanObject(l.Value > r.Value);
            case "==":
                return nativeBoolToBooleanObject(l.Value == r.Value);
            case "!=":
                return nativeBoolToBooleanObject(l.Value != r.Value);
            case "+":
                return new Integer(l.Value + r.Value);
            case "-":
                return new Integer(l.Value - r.Value);
            case "*":
                return new Integer(l.Value * r.Value);
            case "/":
                return new Integer(l.Value / r.Value);
            default:
                return new Null();
        }
    }

    private IObject nativeBoolToBooleanObject(bool input)
    {
        if (input)
        {
            return new Object.Boolean(true);
        }

        return new Object.Boolean(false);
    }

    private IObject evalProgram(List<IStatement> stmts)
    {
        IObject result = null;
        foreach (var stmt in stmts)
        {
            result = new Eval().Evaluate(stmt);

            if (result is Object.ReturnValue rt)
            {
                return rt.Value;
            }
        }

        return result;
    }

    public IObject evalBlockStatement(List<IStatement> stmts)
    {
        IObject result = null;
        foreach (var stmt in stmts)
        {
            result = new Eval().Evaluate(stmt);

            if (result != null && result.Type() == ObjectTypeEnum.RETURN_VALUE_OBJ)
            {
                return result;
            }
        }

        return result;
    }
}