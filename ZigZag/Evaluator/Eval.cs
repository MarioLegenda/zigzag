using System.Runtime.InteropServices;

namespace ZigZag.Evaluator;

using Object;
using Ast;
using ZigZag.Parser;

public class Eval
{
    public IObject? Evaluate(INode node, ObjectEnvironment env)
    {
        if (node is Program p)
        {
            return evalProgram(p.Statements, env);
        }

        if (node is IndexExpression ie)
        {
            IObject? left = new Eval().Evaluate(ie.Left, env);
            ArgumentNullException.ThrowIfNull(left);
            if (isError(left))
            {
                return left;
            }

            IObject? index = new Eval().Evaluate(ie.Index, env);
            ArgumentNullException.ThrowIfNull(index);

            return evalIndexExpression(left, index);
        }

        if (node is ArrayLiteral al)
        {
            List<IObject> list = evalExpressions(al.Elements, env);
            if (list.Count == 1 && isError(list[0]))
            {
                return list[0];
            }

            return new Array(list);
        }

        if (node is StringLiteral stl)
        {
            return new Object.String(stl.Value);
        }

        if (node is CallExpression ce)
        {
            IObject? function = new Eval().Evaluate(ce.Function, env);
            if (function is null)
            {
                throw new ArgumentNullException();
            }

            if (isError(function))
            {
                return function;
            }

            List<IObject> args = evalExpressions(ce.Arguments, env);
            if (args.Count == 1 && isError(args[0]))
            {
                return args[0];
            }

            return applyFunction(function, args);
        }

        if (node is FunctionLiteral fn)
        {
            List<Identifier> parameters = fn.Parameters;
            BlockStatement body = fn.Body;

            return new Function(parameters, body, env);
        }

        if (node is Identifier id)
        {
            return evalIdentifier(id, env);
        }

        if (node is LetStatement lt)
        {
            if (lt.Value is null)
            {
                throw new ArgumentNullException();
            }
            
            IObject? val = new Eval().Evaluate(lt.Value, env);
            if (val is not null && isError(val))
            {
                return val;
            }
            
            env.Set(lt.Name.Value, val);

            return val;
        }

        if (node is ReturnStatement rt)
        {
            IObject val = new Eval().Evaluate(rt.ReturnValue, env);
            if (isError(val))
            {
                return val;
            }
            
            return new Object.ReturnValue(val);
        }

        if (node is Ast.Boolean b)
        {
            return new Object.Boolean(b.Value);
        }

        if (node is Ast.BlockStatement bl)
        {
            return evalBlockStatement(bl.Statements ,env);
        }

        if (node is Ast.IfExpression ifexp)
        {
            return evalIfExpression(ifexp, env);
        }

        if (node is PrefixExpression pe)
        {
            IObject? right = new Eval().Evaluate(pe.Right, env);
            if (isError(right))
            {
                return right;
            }
            
            if (right != null)
            {
                return evalPrefixExpression(pe.Operator, right);
            }
        }

        if (node is InfixExpression ife)
        {
            IObject? left = new Eval().Evaluate(ife.Left, env);
            if (isError(left))
            {
                return left;
            }
            
            IObject? right = new Eval().Evaluate(ife.Right, env);
            if (isError(right))
            {
                return right;
            }

            if (left != null && right != null)
            {            
                return evalInfixExpression(ife.Operator, left, right);
            }
        }

        if (node is ExpressionStatement exp && exp.Expression is not null)
        {
            return new Eval().Evaluate(exp.Expression, env);
        }
        
        if (node is IntegerLiteral it)
        {
            return new Integer(it.Value);
        }

        return null;
    }

    private IObject evalIndexExpression(IObject left, IObject index)
    {
        if (left.Type() == ObjectTypeEnum.ARRAY_OBJ && index.Type() == ObjectTypeEnum.INTEGER_OBJ)
        {
            return evalArrayIndexExpression(left, index);
        }

        return newError("index operator not supported: {0}", left.Type());
    }

    private IObject evalArrayIndexExpression(IObject array, IObject index)
    {
        Array arrayObject = (Array)array;
        Integer indexInteger = (Integer)index;

        int idx = indexInteger.Value;
        int max = arrayObject.Elements.Count - 1;

        if (idx < 0 || idx > max)
        {
            return new Null();
        }

        return arrayObject.Elements[idx];
    }

    private IObject evalIfExpression(Ast.IfExpression ifexp, ObjectEnvironment env)
    {
        IObject? condition = new Eval().Evaluate(ifexp.Condition, env);
        if (condition is not null && isError(condition))
        {
            return condition;
        }

        if (isTruthy(condition))
        {
            return new Eval().Evaluate(ifexp.Consequence, env);
        }
        else if (ifexp.Alternative != null)
        {
            return new Eval().Evaluate(ifexp.Alternative, env);
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
                return newError("unknown operator: {0}{1}", op, right.Type());
            
        }
    }

    private IObject evalMinusPrefixOperatorExpression(IObject right)
    {
        if (right.Type() != ObjectTypeEnum.INTEGER_OBJ)
        {
            return newError("unknown operator: -{0}", right.Type());
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
        if (left.Type() != right.Type())
        {
            return newError("type mismatch: {0} {1} {2}", left.Type(), op, right.Type());
        }
        
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

        if (left.Type() == ObjectTypeEnum.STRING_OBJ && right.Type() == ObjectTypeEnum.STRING_OBJ)
        {
            return evalStringInfixExpression(op, left, right);
        }

        return newError("unknown operator: {0} {1} {2}", left.Type(), op, right.Type());
    }

    private IObject evalStringInfixExpression(string op, IObject left, IObject right)
    {
        Console.WriteLine("Operator: {0}, left: {1}, right: {2}", op, left.Type(), right.Type());
        if (op != "+")
        {
            return new Error("unknown operator: {0} {1} {2}", left.Type(), op, right.Type());
        }
        
        

        String leftVal = (String)left;
        String rightVal = (String)right;
        return new String(leftVal.Value + rightVal.Value);
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
                return newError("unknown operator: {0} {1} {2}", left.Type(), op, right.Type());
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

    private IObject evalProgram(List<IStatement> stmts, ObjectEnvironment env)
    {
        IObject result = null;
        foreach (var stmt in stmts)
        {
            result = new Eval().Evaluate(stmt, env);

            if (result is Object.ReturnValue rt)
            {
                return rt.Value;
            }

            if (result is Object.Error e)
            {
                return result;
            }
        }

        return result;
    }

    public IObject evalBlockStatement(List<IStatement> stmts, ObjectEnvironment env)
    {
        IObject result = null;
        foreach (var stmt in stmts)
        {
            result = new Eval().Evaluate(stmt, env);

            if (result != null && result.Type() == ObjectTypeEnum.RETURN_VALUE_OBJ || result.Type() == ObjectTypeEnum.ERROR_OBJ)
            {
                return result;
            }
        }

        return result;
    }

    private IObject evalIdentifier(Identifier ident, ObjectEnvironment env)
    {
        IObject? val = env.Get(ident.Value);
        if (val is not null)
        {
            return val;
        }

        Builtins builtins = new Builtins();
        if (builtins.Exists(ident.Value))
        {
            return builtins.Get(ident.Value);
        }

        return newError("identifier not found: " + ident.Value);
    }

    private Error newError(string format, params object[] args)
    {
        return new Error(format, args);
    }

    private bool isError(IObject? obj)
    {
        if (obj != null)
        {
            return obj.Type() == ObjectTypeEnum.ERROR_OBJ;
        }

        return false;
    }

    private IObject applyFunction(IObject fn, List<IObject> args)
    {
        if (fn is Function)
        {
            Function function = (Function)fn;

            ObjectEnvironment extendedEnv = extendFunctionEnv(function, args);
            IObject? evaluated = new Eval().Evaluate(function.Body, extendedEnv);
            ArgumentNullException.ThrowIfNull(evaluated);

            return unwrapReturnValue(evaluated);
        }

        if (fn is Builtin builtin)
        {
            return builtin.Fn(args.ToArray());
        }

        return newError("not a function: {0}", fn.Type());
    }

    private IObject unwrapReturnValue(IObject obj)
    {
        if (obj is ReturnValue rt)
        {
            return rt.Value;
        }

        return obj;
    }

    private ObjectEnvironment extendFunctionEnv(Function fn, List<IObject> args)
    {
        ObjectEnvironment env = new ObjectEnvironment(fn.Env);

        for (int idx = 0; idx < fn.Parameters.Count; idx++)
        {
            Identifier identifier = fn.Parameters[idx];
            env.Set(identifier.Value, args[idx]);
        }

        return env;
    }

    private List<IObject> evalExpressions(List<IExpression> expressions, ObjectEnvironment env)
    {
        List<IObject> result = new();

        foreach (var exp in expressions)
        {
            var evaluated = new Eval().Evaluate(exp, env);
            if (isError(evaluated))
            {
                return new List<IObject>() { evaluated };
            }

            if (evaluated is null)
            {
                throw new ArgumentNullException();
            }
            
            result.Add(evaluated);
        }

        return result;
    }
}