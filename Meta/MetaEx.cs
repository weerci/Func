namespace Func.Meta;

public static class MetaEx
{
    public static R SetMeta<T, R>(this R @this, FuncEnum name, T value) //where T : IComparable<T>
    {
        @this!.SetMetaProperty(name, value!);
        return @this;
    }
    public static T? GetMeta<T>(this object @this, FuncEnum name) //where T: IComparable<T>
    {
        var res = @this.GetMetaProperty(name);
        return res is T r ? r : default(T);
    }

    static void SetMetaProperty(this object obj, FuncEnum name, object value)
    {
        SetMetaProperty(obj, name, value, true); //autodispose:true
    }
    static Ex<bool> SetMetaProperty(this object obj, FuncEnum name, object value, bool canDisposed)
    {
        lock (obj)
        {
            var holder = GetHolder(obj); // Holder creation requested

            return holder.TryBool(h =>
            {
                // If exists...
                if (h.MetaDic.TryGetValue(name, out MetaProperty? meta))
                {
                    // Dispose the old value if needed...
                    if (meta.PropertyValue != null && meta.PropertyValue is IDisposable disposable && meta.Disposed)
                        disposable.Dispose();

                    // And storing the new one...
                    meta.PropertyValue = value;
                    meta.Disposed = canDisposed;
                }
                // If not, create the meta-property...
                else
                {
                    meta = new MetaProperty(name, value, canDisposed);
                    h.MetaDic.Add(name, meta);
                }
            });
        }
    }
    static object GetMetaProperty(this object obj, FuncEnum name)
    {
        lock (obj)
        {
            var holder = GetHolder(obj);
            return holder.Match(
                OnSuccess: h =>
                {
                    if (h.MetaDic.TryGetValue(name, out MetaProperty? meta))
                        return meta.PropertyValue;
                    return default;
                },
                OnError: e => default
                )!;
        }
    }

    static Ex<Holder> GetHolder(object obj)
    {
        Holder? res = null;
        lock (_WeakHolders)
        {

            // Trying to find the holder carried in the form of an attribute by this host...
            AttributeCollection list = TypeDescriptor.GetAttributes(obj);
            if (list != null)
            {
                foreach (var item in list)
                {
                    if (item is Holder holder)
                    {
                        res = holder;
                        break;
                    }
                }
            }

            // If not found but creation requested...
            if (res == null)
            {
                res = new Holder(obj);
                var descriptor = TypeDescriptor.AddAttributes(obj, res);

                // Adding the descriptor as the first meta-property...
                var meta = new MetaProperty(FuncEnum.TypeDescriptionProvider, descriptor, true);
                res.MetaDic.Add(meta.PropertyName, meta);

                // And tracking the holder (as a weak reference to not interfere with GC)...
                if (_TrackHolders) _WeakHolders.Add(new WeakReference(res));
            }
        }
        // Returning found, created, or null...
        return res;
    }
    readonly static List<WeakReference> _WeakHolders = [];
    readonly static bool _TrackHolders = true;
}
