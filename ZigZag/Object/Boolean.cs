namespace ZigZag.Object;

public class Boolean: IObject
{
    private bool _value;

    public Boolean(bool value)
    {
        this._value = value;
    }
    
    public string Inspect()
    {
        throw new NotImplementedException();
    }

    public ObjectTypeEnum Type()
    {
        return ObjectTypeEnum.BOOLEAN_OBJ;
    }
}