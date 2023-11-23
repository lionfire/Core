#if ENH
using ReactiveUI;
using System.Reactive.Linq;

namespace LionFire.Inspection.Nodes;

public abstract class DynamicSourceNode : Node
{
    // FUTURE: dynamic Source via SourceNode?
    //get => SourceNode == null ? source : SourceNode.Source;
    //set => this.RaiseAndSetIfChanged(ref source, value);
    //private object? source;

    ///// <summary>
    ///// Points to the Node that contains the actual source
    ///// </summary>
    //public INode SourceNode { get; init; }

    //#region Derived

    //public bool HasSourceNode => !ReferenceEquals(SourceNode, this);

    //#endregion

    protected DynamicSourceNode(INode? parent, object? source, InspectorContext? context = null) 
        : base(parent, source, context)
    {
    }

    protected override void InitSource()
    {
        // ENH: SourceNode
        this.WhenAnyValue(x => x.Source).Subscribe(OnSourceChanged);
    }
}
#endif