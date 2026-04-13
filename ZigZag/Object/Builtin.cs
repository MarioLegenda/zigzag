namespace ZigZag.Object;

public delegate IObject BuiltinFunction(params IObject[] args);

public class Builtin: IObject
{
    public BuiltinFunction Fn { get; set; }
    
    public Builtin(BuiltinFunction fn)
    {
        Fn = fn;
    }

    public string Inspect()
    {
        return "builtin function";
    }

    public ObjectTypeEnum Type()
    {
        return ObjectTypeEnum.BUILTIN_OBJ;
    }
}