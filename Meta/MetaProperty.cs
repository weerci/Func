namespace Func.Meta;

internal class MetaProperty(FuncEnum name, object value, bool canDisposed) : IDisposable
{
    public FuncEnum PropertyName => name;
    public object? PropertyValue { get; set; } = value;
    public bool Disposed { get; set; } = canDisposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (PropertyValue != null && PropertyValue is IDisposable disposable && Disposed)
            disposable.Dispose();

        PropertyValue = default;
    }

    ~MetaProperty()
    {
        Dispose(false);
    }
}
