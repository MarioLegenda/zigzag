namespace ZigZag.Object;

public class ObjectEnvironment
{
    private Dictionary<string, IObject?> store = new();
    private ObjectEnvironment? outer;

    public ObjectEnvironment() {}
    public ObjectEnvironment(ObjectEnvironment? closedEnv)
    {
        this.outer = closedEnv;
    }

    public IObject? Get(string name)
    {
        if (this.store.ContainsKey(name))
        {
            return this.store[name];
        }
        else if (this.outer is not null)
        {
            return this.outer.Get(name);
        }

        return null;
    }

    public void Set(string name, IObject? obj)
    {
        this.store[name] = obj;
    }
}