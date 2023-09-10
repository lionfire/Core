using ReactiveUI;
using LionFire.Data.Async.Gets;
using DynamicData.Binding;
using LionFire.Inspection.ViewModels;
using System.Diagnostics;

namespace LionFire.Inspection.Nodes;

//public class NodesOptions // ENH ?
//{
//    [Reactive]
//    public InspectorNodeKind VisibleItemTypes { get; set; } = InspectorNodeKind.Data;
//}


/// <summary>
/// Decorates NodeVM with children and related properties
/// </summary>
public class NodeChildrenVM : ReactiveObject
{
    #region Relationships

    // Cascading parameter
    public InspectorVM InspectorVM { get; set; }


    public NodeVM NodeVM
    {
        get => nodeVM;
        set
        {
            if (nodeVM == value) return;

            nodeVM = value;

            viewableChildrenSubscription?.Dispose();

            if (Node is IHierarchicalNode h)
            {
                children = h.Children.ObservableCache.Connect()
                    .Transform(n => new NodeVM(NodeVM, n))
                    .AsObservableCache();
                hasChildren = h.HasChildren.ToProperty(this, x => x.HasChildren);

                viewableChildrenSubscription = children
                    .Connect()
                    .Filter(c => IsVisible(c))
                    .ToSortedCollection(n => n.Node.Key)
                    .BindTo(viewableChildren, c => c);
            }
            else
            {
                children = null;
                hasChildren = null;
                viewableChildren.Clear();
            }
        }
    }

    #region Derived

    private INode Node => NodeVM.Node;

    #endregion

    #endregion

    #region Lifecycle

    public NodeChildrenVM(NodeVM nodeVM /*InspectorService inspectorService*/)
    {
        NodeVM = nodeVM;

        // --------------------- TOTRIAGE:

        //NodeVM.InheritedOptions.FlattenedGroups
        //.BindToObservableList(out children)

        //Children.Connect()
        //    .Filter(c => IsVisible(c))
        //    .AsObservableCache();
    }

    #endregion

    #region Children

    public IObservableCache<NodeVM, string>? Children => children;
    private IObservableCache<NodeVM, string>? children;

    public bool? HasChildren => hasChildren?.Value ?? false;
    private ObservableAsPropertyHelper<bool?>? hasChildren;

    public IObservableCollection<NodeVM> ViewableChildren => viewableChildren;
    private readonly IObservableCollection<NodeVM> viewableChildren = new ObservableCollectionExtended<NodeVM>();
    IDisposable? viewableChildrenSubscription;

    #endregion

    #region Event Handler

    public void OnExpand()
    {
        // ENH - more sophisticated approach to this
        var delay = NodeVM.InheritedOptions.GetChildrenOnExpandRetryDelay;
        if (delay != TimeSpan.MaxValue
          && DateTimeOffset.UtcNow - MostRecentGetChildrenTime > delay)
        {
            MostRecentGetChildrenTime = DateTimeOffset.UtcNow;

            var existingTask = getChildrenTask;
            // TODO TOTHREADSAFETY
            if (existingTask == null || existingTask.IsCompleted)
            {
                Debug.WriteLine("UNTESTED - Groups.Get on expand");

                IsGettingChildren = true;
                getChildrenTask = Task.Run(async () =>
                {
                    if (Node is IInspectedNode i)
                    {
                        await Task.WhenAll(i.Groups.Items.Select(g => g.Children.Get().AsTask())).ConfigureAwait(false);
                    }

                    IsGettingChildren = false;
                });
            }
        }
    }

    #endregion

    public bool IsVisible(NodeVM nodeVM)
    {
        var o = NodeVM.InheritedOptions;

        if (!InspectorVM.ShowDataMembers && nodeVM.Node.Info.NodeKind.HasFlag(InspectorNodeKind.Data))
        {
            return false;
        }
        if (!InspectorVM.ShowEvents && nodeVM.Node.Info.NodeKind.HasFlag(InspectorNodeKind.Event))
        {
            return false;
        }
        if (!InspectorVM.ShowMethods && nodeVM.Node.Info.NodeKind.HasFlag(InspectorNodeKind.Method))
        {
            return false;
        }

        if (nodeVM.Node is IInspectorGroup g)
        {
            if (o.GroupBlacklist?.Contains(g.Info.Key) == true)
            {
                return false;
            }
            if (o.GroupWhitelist != null && !o.GroupWhitelist.Contains(g.Info.Key))
            {
                return false;
            }
        }
        else
        {
            //if(o.FlattenedGroups != null )
        }
        return true;
    }

    // --------------------- TOTRIAGE:


    //public IGetterRxO<IEnumerable<object>> MembersGetter { get; set; }

    //public IObservableCache<NodeVM, string> NodeVMs => NodeVM.Children;


    //public IEnumerable<NodeVM> VisibleChildren
    //{
    //    get
    //    {
    //        if (InspectorVM.ShowDataMembers)
    //        {
    //            foreach (var m in NodeVMs.Items.Where(m => m.Node.Info.NodeKind.HasFlag(InspectorNodeKind.Data)))
    //            {
    //                yield return m;
    //            }
    //        }
    //        if (InspectorVM.ShowEvents)
    //        {
    //            foreach (var m in NodeVMs.Items.Where(m => m.Node.Info.NodeKind.HasFlag(InspectorNodeKind.Event)))
    //            {
    //                yield return m;
    //            }
    //        }
    //        if (InspectorVM.ShowMethods)
    //        {
    //            foreach (var m in NodeVMs.Items.Where(m => m.Node.Info.NodeKind.HasFlag(InspectorNodeKind.Method)))
    //            {
    //                yield return m;
    //            }
    //        }
    //    }
    //}

    #region State: Get Children

    public DateTimeOffset MostRecentGetChildrenTime { get; set; } = DateTimeOffset.MinValue; // REVIEW - should this exist? Should it be a decorator on Node itself, if this tracking is requried?

    [Reactive]
    public bool IsGettingChildren { get; set; }

    private Task? getChildrenTask;
    private NodeVM nodeVM;

    #endregion
}

