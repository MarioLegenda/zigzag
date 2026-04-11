namespace ZigZag.Object;

public class Null: IObject
{
    public string Inspect()
    {
        return "null";
    }

    public ObjectTypeEnum Type()
    {
        return ObjectTypeEnum.NULL_OBJ;
    }
}