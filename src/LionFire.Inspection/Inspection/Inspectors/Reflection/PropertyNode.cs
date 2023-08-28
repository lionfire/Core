using DynamicData.Binding;
using LionFire.Data;
using LionFire.Data.Async.Gets;
using LionFire.IO;
using LionFire.Threading;
using MorseCode.ITask;
using ReactiveUI;
using System.Reflection;

namespace LionFire.Inspection.Nodes;

public interface IHasGetterRxO<T>
{
    IGetterRxO<T> Getter { get; }
}

public class PropertyNode : Node
    , IHasGetterRxO<object> // TODO: upgrade Getter to AsyncValue
    , INodeInfo
{

    #region Lifecycle

    public PropertyNode(INode parent, INode sourceNode, PropertyInfo propertyInfo) : base(parent, sourceNode)
    {
        getter = new PropertyNodeGetter(sourceNode, propertyInfo);
        //PropertyGroupInfo = propertyGroupInfo;
    }

    #endregion

    #region Getter

    public IGetterRxO<object> Getter => getter;
    protected PropertyNodeGetter getter;

    #endregion

    //public InspectorGroupInfo PropertyGroupInfo { get; }

    //public INode? Parent => throw new NotImplementedException();
    //public INodeInfo Info => throw new NotImplementedException();

    public override SourceCache<InspectorGroup, string> Groups => throw new NotImplementedException();

    #region INodeInfo

    public string? Name => getter.PropertyInfo.Name;

    public string? Order { get; set; } = null;

    public InspectorNodeKind NodeKind => InspectorNodeKind.Data;

    public IEnumerable<string> Flags => Enumerable.Empty<string>();

    public override INodeInfo Info => this;

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

    public class PropertyNodeGetter : GetterRxO<object>
    {
        public INode SourceNode { get; }
        public object Source => SourceNode?.Source;

        public PropertyInfo PropertyInfo { get; }

        public PropertyNodeGetter(INode sourceNode, PropertyInfo propertyInfo)
        {
            SourceNode = sourceNode;
            PropertyInfo = propertyInfo;

            this.SourceNode.WhenAnyValue(n => n.Source) // TODO: skip immediate evaluation
                .Subscribe(OnSourceChanged);

        }

        private void OnSourceChanged(object? obj)
        {
            DiscardValue();

            if (GetOptions.AutoGet)
            {
                Get().AsTask().FireAndForget();
            }
        }

        protected override ITask<IGetResult<object>> GetImpl(CancellationToken cancellationToken = default)
        {
            var result = PropertyInfo.GetValue(Source);
            return Task.FromResult<IGetResult<object>>(new GetResult<object>(result)).AsITask();
        }
    }
    #endregion
}

