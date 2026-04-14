namespace ZigZag.Object;

public class HashPair
{
    public IObject Key { get; set; }
    public IObject Value { get; set; }

    public HashPair(IObject key, IObject value)
    {
        this.Key = key;
        this.Value = value;
    }
}