namespace LionFire.Mvvm.ObjectInspection;

public interface IMemberVM<TState>: IMemberVM // TODO, maybe
{
    TState State { get; }
}

public interface IMemberVM 
    : IReactiveNotifyPropertyChanged<IReactiveObject>
    , IHandleObservableErrors
    , IReactiveObject
{
    IInspectorMemberInfo Info { get; }
    //object Source { get; set; }
}
