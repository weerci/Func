namespace Func.Impl;

public static class SelectedObj
{
    private readonly static Dictionary<object, bool> Dic = [];

    public static void Selected(this object obj, bool isSelected)
    {
        var res = Dic.TryGetValue(obj, out var la);
        if (res)
            Dic[obj] = isSelected;
        else
            Dic.Add(obj, isSelected);
    }

    public static bool IsSelected(this object obj)
    {
        _ = Dic.TryGetValue(obj, out var la);
        return la;
    }

    public static void RemoveSelected(this object obj)
    {
        var res = Dic.TryGetValue(obj, out var la);
        if (res)
            Dic.Remove(obj);
    }
}
