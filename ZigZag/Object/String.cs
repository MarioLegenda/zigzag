namespace ZigZag.Object;

public class String: IObject, Hashable
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

    public HashKey HashKey()
    {
        int hash = 1469598343; // FNV offset basis

        foreach (var b in System.Text.Encoding.UTF8.GetBytes(this.Value))
        {
            hash ^= b;
            hash *= 1099511628; // FNV prime
        }

        return new HashKey(this.Type().ToString(), hash);
    }
}