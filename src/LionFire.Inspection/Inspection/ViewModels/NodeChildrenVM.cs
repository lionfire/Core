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


public class NodeChildrenVM : ReactiveObject
{
    //public InspectorService InspectorService { get; }

    private INode Node => NodeVM.Node;

    public NodeChildrenVM(NodeVM nodeVM /*InspectorService inspectorService*/)
    {
        //ObjectService = inspectorService;
        NodeVM = nodeVM;

        hasChildren = Node.HasChildren.ToProperty(this, x => x.HasChildren);

        children = Node.Groups.Connect()
            .TransformMany(n => new NodeVM(NodeVM, n))
            .AsObservableCache();


        // --------------------- TOTRIAGE:

        //NodeVM.InheritedOptions.FlattenedGroups

        //.BindToObservableList(out children)
        //.Subscribe();

        //Children.Connect()
        //    .Filter(c => IsVisible(c))
        //    .AsObservableCache();
    }

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
                    await Task.WhenAll(_.Item2.Groups.Items.Select(g => g.Get().AsTask())).ConfigureAwait(false);
                    IsGettingChildren = false;
                });
            }
        }
    }

    #endregion

    public bool? HasChildren => hasChildren.Value;
    readonly ObservableAsPropertyHelper<bool?> hasChildren;

    // --------------------- TOTRIAGE:

    public IObservableCache<NodeVM, string> Children => children;
    private IObservableCache<NodeVM, string> children;

    //public bool IsGroupVisible(NodeVM node) => true;
    public bool IsVisible(NodeVM nodeVM)
    {
        var o = NodeVM.InheritedOptions;
        if (nodeVM.Node.Source is InspectorGroup g)
        {
            if (o.GroupBlacklist?.Contains(g.Info.Key) == true)
            {
                return false;
            }
            if(o.GroupWhitelist != null && !o.GroupWhitelist.Contains(g.Info.Key))
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


    public InspectorVM InspectorVM { get; set; }
    public NodeVM NodeVM { get; set; }

    public IGetterRxO<IEnumerable<object>> MembersGetter { get; set; }

    public IObservableCache<NodeVM, string> NodeVMs => NodeVM.Children;
    public IObservableCache<NodeVM, string> Children => children;
    private IObservableCache<NodeVM, string> children;

    public IObservableCollection<NodeVM> VisibleChildren { get; init; }

    public bool IsVisible(NodeVM nodeVM)
    {
        if (InspectorVM.ShowDataMembers && nodeVM.Node.Info.NodeKind.HasFlag(InspectorNodeKind.Data))
        {
            return true;
        }
        if (InspectorVM.ShowEvents && nodeVM.Node.Info.NodeKind.HasFlag(InspectorNodeKind.Event))
        {
            return true;
        }
        if (InspectorVM.ShowMethods && nodeVM.Node.Info.NodeKind.HasFlag(InspectorNodeKind.Method))
        {
            return true;
        }
        return false;
    }

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

    #endregion
}

