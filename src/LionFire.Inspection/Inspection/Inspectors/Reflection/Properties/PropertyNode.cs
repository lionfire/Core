using DynamicData.Binding;
using LionFire.Data;
using LionFire.Data.Async;
using LionFire.Data.Async.Gets;
using LionFire.Data.Async.Sets;
using LionFire.IO;
using LionFire.Threading;
using MorseCode.ITask;
using ReactiveUI;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Reflection;

namespace LionFire.Inspection.Nodes;

public interface IHasGetterRxO<T>
{
    IGetterRxO<T> Getter { get; }
}

public class PropertyNodeInfo : NodeInfoBase
{
    public PropertyNodeInfo(PropertyInfo propertyInfo)
    {
        PropertyInfo = propertyInfo;
        Name = propertyInfo.Name;
        Type = propertyInfo.PropertyType;
    }

    public PropertyInfo PropertyInfo { get; set; }

    public override InspectorNodeKind NodeKind => InspectorNodeKind.Data;
}

public class PropertyNode : Node<PropertyNodeInfo>
    //, IHasGetterRxO<object?> // TODO: upgrade Getter to AsyncValue
    , IHas<IGetterRxO<object?>>
    , IHas<IAsyncValue<object?>>
//, INodeInfo
{

    #region Lifecycle

    public PropertyNode(INode parent, object source, PropertyInfo propertyInfo) : base(parent, source: source, new PropertyNodeInfo(propertyInfo))
    {
        Key = propertyInfo.Name ?? throw new ArgumentNullException(nameof(propertyInfo) + "." + nameof(propertyInfo.Name));

        parent.Context?.InitNode(this);

        if (propertyInfo.CanRead)
        {
            if (propertyInfo.CanWrite)
            {
                asyncValue = new PropertyNodeValue(source, propertyInfo);
            }
            else
            {
                getter = new PropertyNodeGetter(source, propertyInfo);
            }
            ((IHas<IGetterRxO<object?>>)this).Object!.GetResults.Subscribe(OnGetResult);
            ((IHas<IAsyncValue<object?>>)this)?.Object?.SetResults.Subscribe(OnSetResult);
        }
        //else if (propertyInfo.CanWrite) // Probably different enough to be a class on its own, unless perhaps the get part is StagedValue
        //{
        //    // ENH: Setter only - probably do a separate class since it is so different, unless the Get is just the staged value
        //} 
        //else { /* can neither read nor write */ }
        //((IHas<IGetterRxO<object?>>)this).Object.GetOperations.Subscribe(OnGetResult);
    }

    private void OnGetResult(IGetResult<object?> result)
    {
        if (result.IsSuccess == true)
        {
            Parent?.RaisePropertyChanged(this.Name);
        }
    }
    private void OnSetResult(ISetResult<object?> result)
    {
        if (result.IsSuccess == true && Parent?.Parent is INode grandparent)
        {
            grandparent.RaiseSourceChangeEvents.RaisePropertyChanged(this, Name!);
            //Parent?.RaisePropertyChanged(this.Name);
        }
    }

    #endregion

    PropertyInfo PropertyInfo => (asyncValue?.PropertyInfo ?? getter?.PropertyInfo)!;

    #region Getter

    public IGetterRxO<object?>? Getter => asyncValue ?? (IGetterRxO<object?>?)getter;
    protected PropertyNodeGetter? getter;
    IGetterRxO<object?>? IHas<IGetterRxO<object?>>.Object => Getter;

    public IAsyncValue<object?>? AsyncValue => asyncValue;
    protected PropertyNodeValue? asyncValue;
    IAsyncValue<object?>? IHas<IAsyncValue<object?>>.Object => AsyncValue;

#if ENH
    public ISetterRxO<object?>? Setter => asyncValue ?? (ISetterRxO<object?>?)getter;
    protected PropertyNodeSetter? setter;
    ISetterRxO<object?>? IHas<ISetterRxO<object?>>.Object => Setter;
#endif

    #endregion

    //public GroupInfo PropertyGroupInfo { get; }

    //public INode? Parent => throw new NotImplementedException();
    //public INodeInfo Info => throw new NotImplementedException();

    //public override SourceCache<IInspectorGroup, string> Groups => throw new NotImplementedException();

    #region INodeInfo

    public string? Name => PropertyInfo.Name;

    public string? OrderString { get; set; } = null;

    public InspectorNodeKind NodeKind => InspectorNodeKind.Data;
    


    public IEnumerable<string> Flags => Enumerable.Empty<string>();

    //public override INodeInfo Info => this;

    public IODirection IODirection => PropertyInfo.GetIODirection();

    public Type? Type => PropertyInfo.PropertyType;
    public IEnumerable<Type>? InputTypes => !PropertyInfo.CanWrite
        ? null
        : (PropertyInfo.GetIndexParameters().Length == 0
            ? new Type[] { PropertyInfo.PropertyType }
            : Enumerable.Concat(new Type[] { PropertyInfo.PropertyType }, PropertyInfo.GetIndexParameters().Select(pi => pi.ParameterType))
        );

    #endregion

    #region Internal Classes

    public class PropertyNodeGetter : GetterRxO<object?>
    {
        public object? Source { get; }

        public PropertyInfo PropertyInfo { get; }

        public PropertyNodeGetter(object source, PropertyInfo propertyInfo)
        {
            Source = source;

            PropertyInfo = propertyInfo;

            //this.SourceNode.WhenAnyValue(n => n.Source) // TODO: skip immediate evaluation
            //    .Subscribe(OnObjectChanged);

            //SourceNode.Values.Subscribe(this);

            // TODO: Subscribe to INotifyPropertyChanged
            if (source is INotifyPropertyChanged inpc)
            {
                inpc.PropertyChanged += OnPropertyChanged;
            }
        }

        public bool SubscribeToSourceChanges { get; set; } = true; // TODO MOVE: to GetterOptions?
        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropertyInfo.Name && SubscribeToSourceChanges)
            {
                this.Get().AsTask().FireAndForget();
            }
        }

        protected override ITask<IGetResult<object?>> GetImpl(CancellationToken cancellationToken = default)
        {
            var result = PropertyInfo.GetValue(Source);
            return Task.FromResult<IGetResult<object?>>(GetResult<object?>.SyncSuccess(result)).AsITask();
        }

        #region IObserver: Values

        //public void OnCompleted()
        //{
        //    throw new NotImplementedException(); // TODO: Noop, just want to see this get hit
        //}

        //public void OnError(Exception error)
        //{
        //    throw new NotImplementedException();
        //}

        //public void OnNext(object? obj)
        //{
        //    DiscardValue();

        //    if (GetOptions.AutoGet)
        //    {
        //        Get().AsTask().FireAndForget();
        //    }
        //}

        #endregion

    }

    public class PropertyNodeValue : AsyncValue<object?>
    {
        public object? Source { get; }

        public PropertyInfo PropertyInfo { get; }

        public PropertyNodeValue(object source, PropertyInfo propertyInfo)
        {
            Source = source;

            PropertyInfo = propertyInfo;

            //this.SourceNode.WhenAnyValue(n => n.Source) // TODO: skip immediate evaluation
            //    .Subscribe(OnObjectChanged);

            //SourceNode.Values.Subscribe(this);

            // TODO: Subscribe to INotifyPropertyChanged
            if (source is INotifyPropertyChanged inpc)
            {
                inpc.PropertyChanged += OnPropertyChanged;
            }
        }

        public bool SubscribeToSourceChanges { get; set; } = true; // TODO MOVE: to GetterOptions?
        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropertyInfo.Name && SubscribeToSourceChanges)
            {
                this.Get().AsTask().FireAndForget();
            }
        }

        protected override ITask<IGetResult<object?>> GetImpl(CancellationToken cancellationToken = default)
        {
            var result = PropertyInfo.GetValue(Source);
            Debug.WriteLine($"[PropertyNode] {PropertyInfo.Name} = {result}");
            return Task.FromResult<IGetResult<object?>>(GetResult<object?>.SyncSuccess(result)).AsITask();
        }

        public override Task<ISetResult<object?>> SetImpl(object? value, CancellationToken cancellationToken = default)
        {
            PropertyInfo.SetValue(Source, value);
            return Task.FromResult<ISetResult<object?>>(SetResult<object>.SyncSuccess(value));
        }

        #region IObserver: Values

        //public void OnCompleted()
        //{
        //    throw new NotImplementedException(); // TODO: Noop, just want to see this get hit
        //}

        //public void OnError(Exception error)
        //{
        //    throw new NotImplementedException();
        //}

        //public void OnNext(object? obj)
        //{
        //    DiscardValue();

        //    if (GetOptions.AutoGet)
        //    {
        //        Get().AsTask().FireAndForget();
        //    }
        //}

        #endregion

    }
    #endregion
}

