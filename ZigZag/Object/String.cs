namespace ZigZag.Object;

public class String: IObject
{
    public string Value { get; }

    public String(string value)
    {
        this.Value = value;
    }

    public string Inspect()
    {
        return this.Value;
    }

    public ObjectTypeEnum Type()
    {
        return ObjectTypeEnum.STRING_OBJ;
    }
}