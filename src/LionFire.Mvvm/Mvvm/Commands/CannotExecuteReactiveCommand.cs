using LionFire.Subscribing;
using ReactiveUI;

namespace LionFire.ReactiveUI_;

public class CannotExecuteReactiveCommand<TParam, TResult>
{
    public static ReactiveCommand<TParam, TResult> Instance { get; }

    static CannotExecuteReactiveCommand()
    {
        Instance = ReactiveCommand.Create<TParam, TResult>(p => throw new UnreachableCodeException(), canExecute: ObservableEx2.Return(false));
    }
}
