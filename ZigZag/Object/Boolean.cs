namespace ZigZag.Object;

public class Boolean: IObject, Hashable
{
    public bool Value { set; get; }

    public Boolean(bool value)
    {
        this.Value = value;
    }
    
    public string Inspect()
    {
        return this.Value == true ? "true" : "false";
    }

    public ObjectTypeEnum Type()
    {
        return ObjectTypeEnum.BOOLEAN_OBJ;
    }

    public HashKey HashKey()
    {
        int value = 0;
        if (this.Value)
        {
            value = 1;
        }

        return new HashKey(this.Type().ToString(), value);
    }
}