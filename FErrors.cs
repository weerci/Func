namespace Func;

public static class CoreErrors
{
    public static DeserializeError DeserializeError(string className, string path) => new(className, path);
    public static EmptyListError EmptyListError() => new();
    public static EmptyValueError EmptyValueError() => new();
}

public sealed class DeserializeError(string className, string path) : Exception($"Не удалось восстановить объект класса {className} из файла '{path}'");
public sealed class EmptyListError() : Exception("The list is empty.");
public sealed class EmptyValueError() : Exception("Значение не определено.");
