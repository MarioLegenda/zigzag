namespace ZigZag.Object;

public class ReturnValue : IObject
{
    public IObject Value { get; }

    public ReturnValue(IObject val)
    {
        this.Value = val;
    }

    public string Inspect()
    {
        return this.Value.Inspect();
    }

    public ObjectTypeEnum Type()
    {
        return ObjectTypeEnum.RETURN_VALUE_OBJ;
    }
}