namespace Func;

public record Ex<T>
{
    private Ex() { }
    public Exception? Error { get; }
    public T? Value { get; }

    public bool IsSuccess { get; }
    public bool IsException => !IsSuccess;
    public T? Right => IsSuccess ? Value : default;


    public Ex(Exception ex)
    {
        IsSuccess = false;
        Error = ex ?? throw new ArgumentNullException(nameof(ex));
        Value = default;
    }

    public Ex(T value)
    {
        
        if (value != null)
        {
            IsSuccess = true;
            Value = value;
        }
        else
        {
            IsSuccess = false;
            Error = new Exception(string.Format(Lang.Resources.ArgumentNullOrEmpty, value));
        }
    }

    public static implicit operator Ex<T>(Exception ex) => new(ex);
    public static implicit operator Ex<T>(T t) => new(t);

}

public static class ExE
{
    public static Ex<R> Of<R>(Func<R> func)
    {
        try
        {
            return func();
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public static Ex<R> Of<T, R>(T t, Func<T, R> func)
    {
        try
        {
            return func(t);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    // functor
    public static Ex<R> Map<A, R>(this Ex<A> @this, Func<A, R> f)
    {
        try
        {
            if (@this.IsSuccess)
                return f(@this.Value!);
            else
                return @this.Error!;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public static Ex<RR> Bind<R, RR>(this Ex<R> @this, Func<R, Ex<RR>> f)
    {
        try
        {
            if (@this.IsSuccess)
                return f(@this.Value!);
            else
                return @this.Error!;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public static Ex<R> Try<T, R>(this T t, Func<T, R> f)
    {
        try
        {
            return f(t);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    public static Ex<R> Try<T, R>(this Ex<T> @this, Func<T, R> f)
    {
        try
        {
            if (@this.IsSuccess)
                return f(@this.Value!);
            else
                return @this.Error!;
        }
        catch (Exception e)
        {
            return new(e);
        }
    }
    public static Ex<R> TryBind<T, R>(this Ex<T> @this, Func<T, Ex<R>> f)
    {
        try
        {
            if (@this.IsSuccess)
                return f(@this.Value!);
            else
                return @this.Error!;
        }
        catch (Exception e)
        {
            return new(e);
        }
    }
    public static Ex<bool> TryBool<T>(this Ex<T> @this, Action<T> f)
    {
        try
        {
            if (@this.IsSuccess)
            {
                f(@this.Value!);
                return true;
            }
            else
                return @this.Error!;
        }
        catch (Exception e)
        {
            return new(e);
        }
    }
    public static Ex<bool> TryBool<T>(this T t, Action<T> f)
    {
        try
        {
            f(t);
            return true;
        }
        catch (Exception e)
        {
            return e;
        }
    }
    public static TR Match<T, TR>(this Ex<T> @this, Func<T, TR>? OnSuccess = default, Func<Exception, TR>? OnError = default)
    {
        if (@this.IsSuccess && OnSuccess != null)
            return OnSuccess(@this.Value!);

        if (@this.IsException && OnError != null)
            return OnError(@this.Error!);

        return default!;
    }

    public static void Match<T>(this Ex<T> @this, Action<T>? OnSuccess = default, Action<Exception>? OnError = default) =>
        @this.Match(OnSuccess?.ToFunc(), OnError?.ToFunc());


    // function
    public static T GetOrElse<T>(this Ex<T> @this, T def) => @this.IsSuccess ? @this.Value! : def;


    public static Ex<A> ToEx<A>(this A a)
    {
        return a;
    }

    public static bool IsTrue<A>(this Ex<A> @this, Func<A, bool> f)
    {
        if (@this.IsSuccess)
            try
            {
                return f(@this.Value!);
            }
            catch
            {
                return false;
            }
        else
            return false;
    }

    public static Ex<T> Apply<T>(this Ex<T> @this, Func<T, Ex<T>> f)
    {
        try
        {
            if (@this.IsSuccess)
                return f(@this.Value!);
        }
        catch (Exception)
        {
            return @this;
        }

        return @this;
    }

    public static Ex<T> Apply<A, T>(this Ex<T> @this, A a, Func<A, T, Ex<T>> f)
    {
        try
        {
            if (@this.IsSuccess)
                return f(a, @this.Value!);
        }
        catch (Exception)
        {
            return @this;
        }

        return @this;
    }

    public static Ex<T> Apply<T>(this Ex<T> @this, Action<T> act) => @this.Map(it => { act(it); return it; });



    // LINQ
    public static Ex<R> Select<T, R>(this Ex<T> @this, Func<T, R> map) => @this.Map(map);

    public static Ex<RR> SelectMany<T, R, RR>
    (
       this Ex<T> @this,
       Func<T, Ex<R>> bind,
       Func<T, R, RR> project
    ) => @this.Match
    (
       OnError: ex => new Ex<RR>(ex),
       OnSuccess: t => bind(t).Match
       (
          OnError: ex => new Ex<RR>(ex),
          OnSuccess: r => project(t, r)
       )
    );

    public static IEnumerable<Ex<R>> SelectEx<T, R>(this IEnumerable<Ex<T>> list, Func<T, R> f)
    {
        foreach (var item in list)
            yield return item.Try(t => f(t));
    }

    public static IEnumerable<Ex<R>> SelectManyEx<A, R>(this IEnumerable<Ex<A>> source, Func<A, IEnumerable<R>> f)
    {
        foreach (var element in source)
        {
            if (element.IsException)
                yield return element.Error!;

            foreach (var subElement in f(element.Value!))
                yield return subElement;
        }
    }

    public static IEnumerable<Ex<A>> WhereEx<A>(this IEnumerable<Ex<A>> rs, Func<A, bool> f)
    {
        foreach (var item in rs)
            if (item.IsException)
                yield return item;
            else if (item.Value != null && f(item.Value))
                yield return item;
    }

    public static Ex<A> MinEx<A, B>(this IEnumerable<Ex<A>> list, Func<A, B> f) =>
        list.GetSuccess().Min(f) is Ex<A> ea ? ea : CoreErrors.EmptyListError();

    public static Ex<A> FirstOrDefaultEx<A>(this IEnumerable<Ex<A>> list, Func<A, bool> f, A def) =>
        list.GetSuccess().FirstOrDefault(def);

    public static Ex<A> FirstEx<A>(this IEnumerable<Ex<A>> list, Func<A, bool> f) => list.GetSuccess().First();

    public static Ex<A> AggregateEx<A>(this IEnumerable<Ex<A>> source, Func<A, A, A> func)
    {
        Ex<A> res;
        using IEnumerator<Ex<A>>? e = source.GetEnumerator();

        try
        {
            if (!e.MoveNext())
                return CoreErrors.EmptyValueError();

            res = e.Current;
            if (res.IsException)
                return res.Error!;

            while (e.MoveNext())
            {
                if (e.Current.IsException)
                    res = e.Current.Error!;
                else
                    res = func(res.Value!, e.Current.Value!);
            }
            ;

            return res;

        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /*  public static IOrderedEnumerable<Ex<A>> OrderByEx<A, K>(this IEnumerable<Ex<A>> source, Func<A, K> keySelector)
      {

      }*/


    public static string ToStringEx<A>(this IEnumerable<Ex<A>> source)
    {
        StringBuilder sb = new();
        foreach (var item in source)
            if (item.IsException)
                sb.Append($"{item.Error!.Message}\n");
            else
                sb.Append($"{item.Value}\n");
        return sb.ToString();
    }

    public static IEnumerable<Ex<TResult>> JoinEx<TOuter, TInner, TKey, TResult>(
           this IEnumerable<Ex<TOuter>> outer,
           IEnumerable<Ex<TInner>> inner,
           Func<TOuter, TKey> outerKeySelector,
           Func<TInner, TKey> innerKeySelector,
           Func<TOuter, TInner, TResult> resultSelector)
    {
        foreach (var o in outer)
            foreach (var i in inner)
            {
                if (o.IsSuccess && i.IsSuccess && o.Value != null && outerKeySelector(o.Value)!.Equals(innerKeySelector(i.Value!)))
                    yield return resultSelector(o.Value, i.Value!);
            }
    }

    public static IEnumerable<A> GetSuccess<A>(this IEnumerable<Ex<A>> rs)
    {
        foreach (var item in rs)
            if (item != null && item.IsSuccess)
                yield return item.Value!;
    }

    public static IEnumerable<string> GetFailure<A>(this IEnumerable<Ex<A>> rs)
    {
        foreach (var item in rs)
            if (item != null && item.IsException)
                yield return item.Error!.Message;
    }
}