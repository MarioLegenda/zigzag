namespace ZigZag.Object;

public class Integer: IObject, Hashable
{
    public int Value { get; }

    public Integer(int value)
    {
        this.Value = value;
    }

    public string Inspect()
    {
        return "" + this.Value;
    }

    public ObjectTypeEnum Type()
    {
        return ObjectTypeEnum.INTEGER_OBJ;
    }

    public HashKey HashKey()
    {
        return new HashKey(this.Type().ToString(), this.Value);
    }
}