namespace ZigZag.Object;

public class Hash : IObject
{
    public Dictionary<HashKey, HashPair> Pairs = new ();

    public Hash(Dictionary<HashKey, HashPair> pairs)
    {
        this.Pairs = pairs;
    }
    
    public string Inspect()
    {
        string str = "";

        List<string> pairs = new();
        foreach (var value in this.Pairs.Values)
        {
            pairs.Add(string.Format("{0}: {1}", value.Key.Inspect(), value.Value.Inspect()));
        }

        str += "{";
        str += string.Join(", ", pairs);
        str += "}";
        
        return str;
    }

    public ObjectTypeEnum Type()
    {
        return ObjectTypeEnum.HASH_OBJ;
    }
}