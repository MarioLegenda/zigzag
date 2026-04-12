using ZigZag.Ast;

namespace ZigZag.Object;

public class Function: IObject
{
    public List<Identifier> Parameters { get; set; }
    public BlockStatement Body { get; set; }
    
    public ObjectEnvironment Env { get; set; }

    public Function(List<Identifier> prms, BlockStatement body, ObjectEnvironment env)
    {
        this.Parameters = prms;
        this.Body = body;
        this.Env = env;
    }

    public string Inspect()
    {
        string str = "";

        List<string> prms = new();
        foreach (var param in this.Parameters)
        {
            prms.Add(param.String());
        }

        str += "fn";
        str += "(";
        str += string.Join(", ", prms);
        str += ") {\n";
        str += this.Body.String();
        str += "\n";

        return str;
    }

    public ObjectTypeEnum Type()
    {
        return ObjectTypeEnum.FUNCTION_OBJ;
    }
}