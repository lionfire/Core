using DynamicData.Binding;
using LionFire.Data;
using LionFire.Data.Async.Gets;
using LionFire.IO;
using LionFire.Threading;
using MorseCode.ITask;
using ReactiveUI;
using System.ComponentModel;
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
    }

    public PropertyInfo PropertyInfo { get; set; }

    public override InspectorNodeKind NodeKind => InspectorNodeKind.Data;
}

public class PropertyNode : Node<PropertyNodeInfo>
    , IHasGetterRxO<object> // TODO: upgrade Getter to AsyncValue
    , INodeInfo
{

    #region Lifecycle

    public PropertyNode(INode parent, object source, PropertyInfo propertyInfo) : base(parent, source: source, new PropertyNodeInfo(propertyInfo))
    {
        getter = new PropertyNodeGetter(source, propertyInfo);
        //PropertyGroupInfo = propertyGroupInfo;
    }

    #endregion

    #region Getter

    public IGetterRxO<object> Getter => getter;
    protected PropertyNodeGetter getter;

    #endregion

    //public GroupInfo PropertyGroupInfo { get; }

    //public INode? Parent => throw new NotImplementedException();
    //public INodeInfo Info => throw new NotImplementedException();

    //public override SourceCache<IInspectorGroup, string> Groups => throw new NotImplementedException();

    #region INodeInfo

    public string? Name => getter.PropertyInfo.Name;

    public string? Order { get; set; } = null;

    public InspectorNodeKind NodeKind => InspectorNodeKind.Data;

    public IEnumerable<string> Flags => Enumerable.Empty<string>();

    //public override INodeInfo Info => this;

    public IODirection IODirection => getter.PropertyInfo.GetIODirection();

    public Type? Type => getter.PropertyInfo.PropertyType;
    public IEnumerable<Type>? InputTypes => !getter.PropertyInfo.CanWrite
        ? null
        : (getter.PropertyInfo.GetIndexParameters().Length == 0
            ? new Type[] { getter.PropertyInfo.PropertyType }
            : Enumerable.Concat(new Type[] { getter.PropertyInfo.PropertyType }, getter.PropertyInfo.GetIndexParameters().Select(pi => pi.ParameterType))
        );

    #endregion

    #region Internal Classes

    public class PropertyNodeGetter : GetterRxO<object?>//, IObserver<object?>
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
            return Task.FromResult<IGetResult<object?>>(new GetResult<object?>(result) { Flags = TransferResultFlags.RanSynchronously | TransferResultFlags.Success }).AsITask();
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

