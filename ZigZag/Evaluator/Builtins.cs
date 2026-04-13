namespace ZigZag.Evaluator;

using Object;

public class Builtins
{
    private Dictionary<string, Builtin> _dictionary = new ();

    public Builtins()
    {
        _dictionary["len"] = new Builtin((args) =>
        {
            if (args.Length != 1)
            {
                return new Error("wrong number of arguments. got={0}, want=1", args.Length);
            }

            IObject arg = args[0];
            if (arg is String a)
            {
                return new Integer(a.Value.Length);
            }

            return new Error("argument to `len` not supported, got {0}", args[0].Type());
        });
    }

    public bool Exists(string builtinName)
    {
        return this._dictionary.ContainsKey(builtinName);
    }

    public Builtin Get(string builtinName)
    {
        return this._dictionary[builtinName];
    }
}