using LionFire.Inspection.Nodes;
using LionFire.Inspection.ViewModels;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace LionFire.Blazor.Components.MudBlazor_.PropertyGrid;

// As much ViewModel logic as possible should be in NodeVM

public partial class InspectorRow
{
    #region Parameters

    [CascadingParameter(Name = "InspectorVM")]
    public InspectorVM? InspectorVM { get; set; }

    [Parameter]
    [Required]
    public NodeVM? NodeVM { get; set; }

    #endregion

    #region Lifecycle

    protected override Task OnParametersSetAsync()
    {
        ViewModel!.NodeVM = NodeVM;
        ViewModel.InspectorVM = InspectorVM;

        Debug.WriteLine($"{NodeVM?.Depth} {NodeVM?.Node.Path}  Source: {NodeVM?.Node.Source?.GetType().Name}, Value: {NodeVM?.Node.Value?.GetType().Name}");

        ViewModel.Refresh();

        if (NodeVM != null && NodeVM.Depth == 0) NodeVM.ShowChildren = true; // View logic

        return base.OnParametersSetAsync();
    }

    protected override Task OnInitializedAsync()
    {
        NodeVM.ChildrenVM?.OnExpand();

        this.WhenAnyValue(x => x.NodeVM!.ShowChildren)
            .Subscribe(visible =>
            {
                UpdateChildContent();
            });
        UpdateChildContent(); // ENH - bind IsExpanded to UpdateChildContent

        //if (NodeVM.Node.Info.CanRead() && NodeVM.Node.Info.Type is System.Collections.IEnumerable e)
        //{
        //    try
        //    {
        //        //ChildNodeVMs2 = new GetterVM<IEnumerable<object>>() InspectorNode.Source; 
        //        //ViewModel.ChildNodeVMs = ReflectionNodeVM.GetFor(dataNodeVM.GetValue());
        //        //ViewModel.ChildNodeVMs = ReflectionNodeVM.GetFor(dataNodeVM.GetValue());
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex);
        //    }
        //}

        //SetChildGenerator(); // .razor

        return base.OnInitializedAsync();
    }

    #endregion

    #region Children

    //private RenderFragment? ChildGenerator;
    //private RenderFragment? CurrentChildGenerator;

    #region (private)

    private void UpdateChildContent()
    {
        //CurrentChildGenerator =
        //    ViewModel?.NodeVM?.ShowChildren == true
        //        && ViewModel?.NodeVM?.Children.Count > 0 == true
        //        && NodeVM.Depth < ViewModel.NodeVM.InheritedOptions.MaxDepth
        //    ? ChildGenerator
        //    : null;
    }

    #endregion

    #endregion


    public bool ShowInspectedNode { get; set; } = false;
    public bool ShowGroups { get; set; } = false;
    public IEnumerable<string> GetRowClasses2(NodeVM nodeVM)
    {
        //yield return nodeVM.Node switch
        //{
        //    InspectedNode => "inspected-node",
        //    _ => "",
        //};
        bool hidden = false;
        if (!ShowInspectedNode && nodeVM.Node is InspectedNode) hidden=true;
        if (!ShowGroups && nodeVM.Node is IGroupNode) hidden = true;
        if (NodeVM.IsFlattened)
        {
            hidden = true; 
            yield return "flattened";
        }
        yield return NodeVM.Node.Info.NodeKind.ToString();
        if (hidden && !NodeVM.Options.ShowAll) yield return "hidden";
        yield return nodeVM.Node.GetType().Name;
    }
    public string GetRowClasses(NodeVM nodeVM)
    {
        return string.Join(" ", GetRowClasses2(nodeVM));
    }
}

