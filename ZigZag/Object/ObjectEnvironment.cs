namespace ZigZag.Object;

public class ObjectEnvironment
{
    private Dictionary<string, IObject?> store = new();

    public IObject? Get(string name)
    {
        if (this.store.TryGetValue(name, out var value))
        {
            return value;
        }

        return null;
    }

    public void Set(string name, IObject? obj)
    {
        this.store[name] = obj;
    }
}