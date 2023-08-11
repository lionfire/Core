namespace LionFire.Mvvm.ObjectInspection;

public abstract class InspectorNode<TInfo> : IInspectorNode
    where TInfo : IInspectorNodeInfo
{
    public TInfo Info { get; }
    IInspectorMemberInfo IInspectorNode.Info => Info;

    public InspectorNode(TInfo info)
    {
        Info = info;
    }

    public abstract object Source { get; }
    public abstract IEnumerable<IInspectorNode> DerivedFrom { get; }
    public abstract IReadOnlyDictionary<string, IInspectorNode> DerivedInspectors { get; }
    public abstract IObservableList<IInspectorNode> Children { get; }
    public abstract IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changing { get; }
    public abstract IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changed { get; }
    public abstract IObservable<Exception> ThrownExceptions { get; }


    public abstract event PropertyChangedEventHandler? PropertyChanged;
    public abstract event PropertyChangingEventHandler? PropertyChanging;

    public abstract IDisposable SuppressChangeNotifications();
    public abstract void RaisePropertyChanging(PropertyChangingEventArgs args);
    public abstract void RaisePropertyChanged(PropertyChangedEventArgs args);
}

public abstract class MemberVM : InspectorNode<IInspectorMemberInfo>
{
    protected MemberVM(IInspectorMemberInfo info) : base(info)
    {
    }

    public abstract object Source { get; }
}
