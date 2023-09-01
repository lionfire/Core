using ReactiveUI;
using System.Reactive.Linq;

namespace LionFire.Inspection.Nodes;

#if OLD // Extraneous
public abstract class CompositeChildrenNode : ReactiveObject, INode
{
    protected void InitChildren()
    {
        var groupNodes = Groups == null
            ? null
            : Groups
               .Connect()
               .Transform(g => (INode)new GroupNode(this, g));


        PopulateIntoChildren(groupNodes, "group:");
        PopulateIntoChildren(Nodes, "");


        //SourceCache<INode, string> children;
        //SourceCache<INode, string> initChildren(SourceCache<INode, string> c)
        //=> c ?? new SourceCache<INode, string>(n => n.Key);

        //if (groupNodes != null)
        //{
        //    children = initChildren(children);
        //    groupNodes.PopulateInto(children);
        //}
        //if (Nodes != null)
        //{
        //    children = initChildren(children);
        //    Nodes.Connect().PopulateInto(children);
        //}

        if (children != null)
        {
            Children = children.AsObservableCache();
            hasChildren = Children.CountChanged.Select(c => c > 0 ? (bool?)true : false);

            //hasChildren = this.WhenAnyValue(x => x.Groups)
            //.Select(_ =>
            //{
            //    //if (Children.Count > 0) return (bool?)true;
            //    if (Groups.Count == 0) return (bool?)false;

            //    // else Groups > 0 && Children == 0
            //    return Groups.Items.Any(g => !g.HasValue)
            //        ? (bool?)null
            //        : false;
            //});
        }
        else
        {
            hasChildren = Observable.Return((bool?)false);
        }
    }

    public IDisposable PopulateIntoChildren(IObservableCache<INode, string> observableCache, string idPrefix = "")
    {
        children ??= new SourceCache<INode, string>(n => idPrefix + n.Key);
        return observableCache.Connect().PopulateInto(children);
    }

    #region Children

    public IObservableCache<INode, string>? Children => children.AsObservableCache();
    protected SourceCache<INode, string>? children { get; set; }


    public IObservable<bool?> HasChildren => hasChildren;
    private IObservable<bool?> hasChildren;

    #region Child sources

    public virtual IObservableCache<InspectorGroup, string>? Groups => null;
    public virtual IObservableCache<INode, string>? Nodes => null;

    #endregion

    #endregion

    #region IKeyProvider

    public virtual string GetKey(INode? node) => Interlocked.Increment(ref nextKey).ToString();
    protected static ulong nextKey = 0;

    #endregion
}
#endif
