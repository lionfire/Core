﻿using ReactiveUI;
using LionFire.Data.Async.Gets;
using DynamicData.Binding;
using LionFire.Inspection.ViewModels;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Collections.ObjectModel;

namespace LionFire.Inspection.Nodes;

//public class NodesOptions // ENH ?
//{
//    [Reactive]
//    public InspectorNodeKind VisibleItemTypes { get; set; } = InspectorNodeKind.Data;
//}

/// <summary>
/// Decorates NodeVM with children and related properties
/// </summary>
public partial class NodeChildrenVM : ReactiveObject
{
    #region Relationships

    // Cascading parameter
    //public InspectorVM? InspectorVM { get; set; }

    [SetOnce]
    [ReactiveUI.SourceGenerators.Reactive]
    private NodeVM _nodeVM;

    //public NodeVM NodeVM
    //{
    //    get => nodeVM;
    //    set
    //    {
    //        if (nodeVM == value) return;

    //        nodeVM = value;

    //        viewableChildrenSubscription?.Dispose();

    //        if (Node is IHierarchicalNode h)
    //        {
    //            children = h.Children.ObservableCache.Connect()
    //                .Transform(n => new NodeVM(NodeVM, n))
    //                .AsObservableCache();
    //            hasChildren = h.HasChildren.ToProperty(this, x => x.HasChildren);

    //            //viewableChildrenSubscription = children
    //            viewableChildren = children
    //                .Connect()
    //                .Filter(c => IsVisible(c))
    //                .ToSortedCollection(n => n.Node.Key)
    //                .ToProperty(this, x => x.ViewableChildren);
    //                //.BindTo(viewableChildren, c => c);
    //        }
    //        else
    //        {
    //            children = null;
    //            hasChildren = null;
    //            viewableChildren.Clear();
    //        }
    //    }
    //}
    //private NodeVM nodeVM;


    #region Derived

    private INode Node => NodeVM.Node;

    #endregion

    #endregion

    #region Lifecycle

    public NodeChildrenVM(NodeVM nodeVM)
    {
        NodeVM = nodeVM;

        //viewableChildren = this.WhenAnyValue(x => x.NodeVM)
        //    .Select(nodeVM =>
        //    {
        //        //viewableChildrenSubscription?.Dispose();

        //        if (nodeVM.Node is IHierarchicalNode h)
        //        {
        //            children = h.Children.ObservableCache.Connect()
        //                .Transform(n => new NodeVM(nodeVM, n))
        //                .AsObservableCache();
        //            hasChildren = h.HasChildren.ToProperty(this, x => x.HasChildren);

        //            return children
        //                .Connect()
        //                .Filter(c => IsVisible(c))
        //                .ToSortedCollection(n => n.Node.Key);
        //        }
        //        else
        //        {
        //            children = null;
        //            hasChildren = null;
        //            //viewableChildren.Clear();

        //            return Observable.Return<IReadOnlyCollection<NodeVM>>(new ReadOnlyCollection<NodeVM>(new List<NodeVM> { }));
        //        }

        //    })
        //    .ToProperty(this, x => x.ViewableChildren)
        //    ;
        //viewableChildrenSubscription?.Dispose();

        if (nodeVM.Node is IHierarchicalNode h)
        {
            children = h.Children.ObservableCache.Connect()
                .Transform(n => new NodeVM(nodeVM, n))
                .AsObservableCache();
            hasChildren = h.HasChildren.ToProperty(this, x => x.HasChildren);

            viewableChildren = children
                .Connect()
                .AutoRefresh(x => x.LocalOptions)
                .AutoRefresh(x => x.Options.VisibilityFlags)
                .Filter(c => IsVisible(c))
                .ToSortedCollection(NodeVMComparer.Instance) // was (n => n.Node.Key)
                .ToProperty(this, x => x.ViewableChildren);
        }
        else
        {
            children = null;
            hasChildren = null;
            //viewableChildren.Clear();

            viewableChildren = Observable.Return<IReadOnlyCollection<NodeVM>>(new ReadOnlyCollection<NodeVM>(new List<NodeVM> { }))
                .ToProperty(this, x => x.ViewableChildren);
        }
    }

    #endregion

    #region Children

    public IObservableCache<NodeVM, string>? Children => children;
    private IObservableCache<NodeVM, string>? children;

    public bool? HasChildren => hasChildren?.Value ?? false;
    private ObservableAsPropertyHelper<bool?>? hasChildren;

    public IReadOnlyCollection<NodeVM> ViewableChildren => viewableChildren.Value ?? EmptyNodeVMs;
    private readonly ObservableAsPropertyHelper<IReadOnlyCollection<NodeVM>> viewableChildren;
    private readonly static ReadOnlyCollection<NodeVM> EmptyNodeVMs = new ReadOnlyCollection<NodeVM>(Enumerable.Empty<NodeVM>().ToList());

    #endregion

    #region Event Handler

    public void OnExpand()
    {
        // ENH - more sophisticated approach to this
        var delay = NodeVM.Options.GetChildrenOnExpandRetryDelay;
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
                    else if (Node is IHierarchicalNode h)// Node.Info.NodeKind | InspectorNodeKind.Group == InspectorNodeKind.Group)
                    {
                        // TEMP?
                        await Task.WhenAll(h.Children.Select(kvp => kvp.Value as IStatelessGetter<object>).Where(o => o != null).Select(g => g.Get().AsTask())).ConfigureAwait(false);
                    }
                    IsGettingChildren = false;
                });
            }
        }
    }

    #endregion

    public bool IsVisible(NodeVM nodeVM)
    {
        var o = NodeVM.Options;

        if (!NodeVM.ShowDataMembers && nodeVM.Node.Info.NodeKind.HasFlag(InspectorNodeKind.Data))
        {
            return false;
        }
        if (!NodeVM.ShowEvents && nodeVM.Node.Info.NodeKind.HasFlag(InspectorNodeKind.Event))
        {
            return false;
        }
        if (!NodeVM.ShowMethods && nodeVM.Node.Info.NodeKind.HasFlag(InspectorNodeKind.Method)) { return false; }
        if (!NodeVM.ShowDev && nodeVM.Node.Visibility.HasFlag(InspectorVisibility.Dev)) { return false; }
        if (!NodeVM.ShowHidden && nodeVM.Node.Visibility.HasFlag(InspectorVisibility.Hidden)) { return false; }

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

    [ReactiveUI.SourceGenerators.Reactive]
    private bool _isGettingChildren;
    private Task? getChildrenTask;

    #endregion
}
