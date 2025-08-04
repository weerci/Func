using DynamicData;
using ReactiveUI;
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

}
