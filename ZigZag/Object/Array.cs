using ZigZag.Ast;

namespace ZigZag.Object;

public class Array : IObject
{
    public List<IObject> Elements = new();

    public Array(List<IObject> elems)
    {
        this.Elements = elems;
    }
    
    public string Inspect()
    {
        string str = "";

        List<string> elems = new();
        foreach (var elem in this.Elements)
        {
            elems.Add(elem.Inspect());
        }

        str += "[";
        str += string.Join(", ", elems);
        str += "]";

        return str;
    }

    public ObjectTypeEnum Type()
    {
        return ObjectTypeEnum.ARRAY_OBJ;
    }
}