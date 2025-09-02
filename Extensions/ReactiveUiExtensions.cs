using DynamicData;
using ReactiveUI;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Func.Extensions;

public static class ReactiveUiExtensions
{
    public static void ExecuteIfPossible<TParam, T, TResult>(this ReactiveCommand<TParam, TResult> cmd, T t)
    {
        if (cmd is ICommand c && c.CanExecute(t))
            c.Execute(t);
    }

    public static IObservable<TResult> WhenAnyValueNotNull<TSource, TResult>(
        this TSource source,
        Expression<Func<TSource, TResult>> propertySelector)
        where TSource : IReactiveObject
    {
        return source.WhenAnyValue(propertySelector)
            .Where(value => value != null)
            .Select(value => value!);
    }
}
