namespace ZigZag.Object;

public class Integer: IObject
{
    private int _value;

    public Integer(int value)
    {
        this._value = value;
    }
    
    public string Inspect()
    {
        return "" + this._value;
    }

    public ObjectTypeEnum Type()
    {
        return ObjectTypeEnum.INTEGER_OBJ;
    }
}