namespace Func;

public class FileName() : FileSystem
{
    private FileName(string path) : this()
    {
        if (!File.Exists(path))
            throw new FileNotFoundException();
        PathFile = path;
        Dir = Path.GetDirectoryName(path);
        Name = Path.GetFileNameWithoutExtension(path);
        Ext = Path.GetExtension(path);
        NameExt = Path.GetFileName(path);
        Dt = DateTime.Now;
    }

    #region Properties
    public string PathFile { get; }
    public string? Dir { get; }
    public string? Name { get; }
    public string? Ext { get; }
    public string? NameExt { get; }
    public DateTime Dt { get; }

    #endregion

    public static Ex<FileName> Open(string path) => Of(() => new FileName(path));
    public static Ex<FileName> OpenOrCreate(string path) => Of(() =>
    {
        FileSystem fs = new();
        if (!fs.File.Exists(path))
        {
            var dir = fs.Path.GetDirectoryName(path);
            if (!String.IsNullOrEmpty(dir) && !fs.Directory.Exists(dir))
                fs.Directory.CreateDirectory(dir!);
            using var v = fs.File.Create(path);
            v.Close();
        }

        return new FileName(path);
    });


    /// <summary>
    /// Файл разбирается на отдельные строки
    /// </summary>
    public Ex<string[]> FileNameToList() => File.ReadAllLines(PathFile, Encoding.Default);

    /// <summary>
    /// Читает файл в массив байтов
    /// </summary>
    public Ex<byte[]> FileNameToArray() => File.ReadAllBytes(PathFile);
}

public static class FileNameEx
{
    public static void Delete(this Ex<FileName> fileName) =>
        fileName.Match(fn =>
        {
            if (fn.File.Exists(fn.PathFile))
                fn.File.Delete(fn.PathFile); return Unit.Default;
        });

    public static bool Exists(this Ex<FileName> fileName) => fileName.Match(fn => fn.File.Exists(fn.PathFile));
}
