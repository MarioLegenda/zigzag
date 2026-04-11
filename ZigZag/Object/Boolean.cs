namespace ZigZag.Object;

public class Boolean: IObject
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
}