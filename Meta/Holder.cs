namespace Func.Meta;

[AttributeUsage(AttributeTargets.All)]
internal class Holder(object obj) : Attribute, IDisposable
{
    internal readonly Dictionary<FuncEnum, MetaProperty> MetaDic = [];
    internal WeakReference? _HostReference = new(obj);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        foreach (var kvp in MetaDic)
            kvp.Value.Dispose();

        MetaDic.Clear();
        _HostReference = null;

    }

    ~Holder()
    {
        Dispose(false);
    }
}

