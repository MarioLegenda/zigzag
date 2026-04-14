namespace ZigZag.Object;

public struct HashKey
{
    public string Type { get; }
    public int Value { get; }

    public HashKey(string type, int value)
    {
        this.Type = type;
        this.Value = value;
    }
}