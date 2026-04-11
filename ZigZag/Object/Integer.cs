namespace ZigZag.Object;

public class Integer: IObject
{
    public int Value { get; set; }

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
}