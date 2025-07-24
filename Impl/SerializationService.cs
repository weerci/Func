
namespace Func.Services;

/// <inheritdoc />
public class SerializationService : ISerializationService
{
    private readonly FileSystem _fileSystem = new();

    private static JsonSerializerOptions GetOptions(IJsonTypeInfoResolver? serializerContext) =>
        new()
        {
            TypeInfoResolver = serializerContext,
            IgnoreReadOnlyProperties = true,
            WriteIndented = true
        };

    /// <inheritdoc />
    public Ex<string> Serialize<T>(T dataToSerialize, IJsonTypeInfoResolver? serializerContext) =>
        ExE.Of(() => JsonSerializer.Serialize(dataToSerialize, typeof(T), GetOptions(serializerContext)));

    /// <inheritdoc />
    public Task SerializeAsync<T>(Stream utf8Json, T dataToSerialize, IJsonTypeInfoResolver? serializerContext) =>
        JsonSerializer.SerializeAsync(utf8Json, dataToSerialize, typeof(T), GetOptions(serializerContext));

    /// <inheritdoc />
    public Ex<T> Deserialize<T>(Ex<string> data, IJsonTypeInfoResolver? serializerContext) =>
        data.Map(d => (T)JsonSerializer.Deserialize(d, typeof(T), GetOptions(serializerContext)));

    /// <inheritdoc />
    public async ValueTask<T> DeserializeAsync<T>(Stream utf8Json, IJsonTypeInfoResolver? serializerContext)
        where T : class, new() =>
        (T)await JsonSerializer.DeserializeAsync(utf8Json, typeof(T), GetOptions(serializerContext)).ConfigureAwait(false)!;

    /// <inheritdoc />
    public Ex<bool> SerializeToFile<T>(T dataToSerialize, Ex<FileName> fileName, IJsonTypeInfoResolver? serializerContext)
    {
        return fileName.Map( fn =>
        {
            using var writer = _fileSystem.FileStream.New(fn.PathFile, FileMode.Create);
            JsonSerializer.Serialize(writer, dataToSerialize, typeof(T), GetOptions(serializerContext));
            writer.Flush();
            return true;
        });
    }

    /// <inheritdoc />
    public async Task SerializeToFileAsync<T>(T dataToSerialize, string path, IJsonTypeInfoResolver? serializerContext)
    {
        await using var writer = _fileSystem.FileStream.New(path, FileMode.Create);
        await JsonSerializer.SerializeAsync(writer, dataToSerialize, typeof(T), GetOptions(serializerContext));
        writer.Flush();
    }

    /// <inheritdoc />
    public Ex<T> DeserializeFromFile<T>(Ex<FileName> fileName, IJsonTypeInfoResolver? serializerContext)
    {
        return fileName.Map(fn => {
            using var stream = _fileSystem.File.OpenRead(fn.PathFile);
            return (T)JsonSerializer.Deserialize(stream, typeof(T), GetOptions(serializerContext))!;
        });

    }

    /// <inheritdoc />
    public async ValueTask<T> DeserializeFromFileAsync<T>(string path, IJsonTypeInfoResolver? serializerContext)
    {
        var stream = _fileSystem.File.OpenRead(path);
        return (T)(await JsonSerializer.DeserializeAsync(stream, typeof(T), GetOptions(serializerContext)))!;
    }
}
