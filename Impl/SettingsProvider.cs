using Func.Services;

namespace Func.Impl;

public class SettingsProvider<T>(ISerializationService serializationService, IJsonTypeInfoResolver? serializerContext)
    : ISettingsProvider<T> where T : class, new()
{

    private readonly ISerializationService _serialization = serializationService;
    private readonly IJsonTypeInfoResolver? _serializerContext = serializerContext;

    public T Value
    {
        get => _value;
        set
        {
            _value = value;
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
    private T _value = new();

    public event EventHandler? Changed;
    public event EventHandler? Saving;

    public Ex<bool> Load(Ex<FileName> fileName)
    {
        var v = fileName.Bind(fn => _serialization.DeserializeFromFile<T>(fn, _serializerContext));
        if (v.IsSuccess)
            return true;
        else
            return v.Error!;
    }

    public Ex<bool> Save(Ex<FileName> fileName)
    {
        Saving?.Invoke(this, EventArgs.Empty);

        return fileName.Bind(fn => _serialization.SerializeToFile(Value, fn, _serializerContext));
    }
}
