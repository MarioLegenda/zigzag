namespace ZigZag.Object;

public class Error: IObject
{
    public string Message { set; get; }

    public Error(string format, params object[] args)
    {
        this.Message = string.Format(format, args);
    }

    public string Inspect()
    {
        return "Error: " + this.Message;
    }

    public ObjectTypeEnum Type()
    {
        return ObjectTypeEnum.ERROR_OBJ;
    }
}