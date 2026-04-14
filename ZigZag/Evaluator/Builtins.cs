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

            if (arg is Array b)
            {
                return new Integer(b.Elements.Count);
            }

            return new Error("argument to `len` not supported, got {0}", args[0].Type());
        });
        
        _dictionary["first"] = new Builtin((args) =>
        {
            if (args.Length != 1)
            {
                return new Error("wrong number of arguments. got={0}, want=1", args.Length);
            }

            IObject arg = args[0];
            if (arg.Type() != ObjectTypeEnum.ARRAY_OBJ)
            {
                return new Error("argument to `first` must be ARRAY, got {0}", arg.Type());
            }

            Array array = (Array)arg;
            if (array.Elements.Count > 0)
            {
                return array.Elements[0];
            }

            return new Error("argument to `len` not supported, got {0}", args[0].Type());
        });
        
        _dictionary["last"] = new Builtin((args) =>
        {
            if (args.Length != 1)
            {
                return new Error("wrong number of arguments. got={0}, want=1", args.Length);
            }

            IObject arg = args[0];
            if (arg.Type() != ObjectTypeEnum.ARRAY_OBJ)
            {
                return new Error("argument to `first` must be ARRAY, got {0}", arg.Type());
            }

            Array array = (Array)arg;
            int length = array.Elements.Count;
            if (length > 0)
            {
                return array.Elements[length - 1];
            }

            return new Error("argument to `len` not supported, got {0}", args[0].Type());
        });
        
        _dictionary["rest"] = new Builtin((args) =>
        {
            if (args.Length != 1)
            {
                return new Error("wrong number of arguments. got={0}, want=1", args.Length);
            }

            IObject arg = args[0];
            if (arg.Type() != ObjectTypeEnum.ARRAY_OBJ)
            {
                return new Error("argument to `first` must be ARRAY, got {0}", arg.Type());
            }

            Array array = (Array)arg;
            int length = array.Elements.Count;
            if (length > 0)
            {
                var newElements = array.Elements.Skip(1).ToList();
                return new Array(newElements);
            }

            return new Null();
        });
        
        _dictionary["push"] = new Builtin((args) =>
        {
            if (args.Length != 2)
            {
                return new Error("wrong number of arguments. got={0}, want=1", args.Length);
            }

            IObject arg = args[0];
            if (arg.Type() != ObjectTypeEnum.ARRAY_OBJ)
            {
                return new Error("argument to `first` must be ARRAY, got {0}", arg.Type());
            }

            Array array = (Array)arg;
            int length = array.Elements.Count;

            var newElements = array.Elements.Append(args[1]).ToList();
            Array newArray = new Array(newElements);

            return newArray;
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