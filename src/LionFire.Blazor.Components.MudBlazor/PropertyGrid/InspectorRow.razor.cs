using LionFire.Inspection.ViewModels;
using Microsoft.AspNetCore.Components;

namespace LionFire.Blazor.Components.MudBlazor_.PropertyGrid;

// As much ViewModel logic as possible should be in NodeVM

public partial class InspectorRow
{
    #region Parameters

    [CascadingParameter(Name = "InspectorVM")]
    public InspectorVM? InspectorVM { get; set; }

    [Parameter]
    public NodeVM? NodeVM { get; set; }// { get => ViewModel.NodeVM; set => ViewModel.NodeVM = value; }

    #endregion

    #region Lifecycle

    protected override Task OnParametersSetAsync()
    {
        ViewModel!.NodeVM = NodeVM;
        ViewModel.InspectorVM = InspectorVM;

        this.WhenAnyValue(x => x.NodeVM!.AreChildrenVisible)
            .Subscribe(visible =>
            {
                UpdateChildContent();
            });

        return base.OnParametersSetAsync();
    }

    protected override Task OnInitializedAsync()
    {
        Debug.WriteLine($"Depth: {NodeVM.Depth}.  Target: {NodeVM.Node.Source?.GetType().Name}");

        ViewModel.Refresh();

        if (NodeVM.Depth == 0) ViewModel.NodeVM.AreChildrenVisible = true;

        UpdateChildContent(); // ENH - bind IsExpanded to UpdateChildContent

        if (NodeVM.Node.Info.CanRead() && NodeVM.Node.Info.Type is System.Collections.IEnumerable e)
        {
            try
            {
                //ChildNodeVMs2 = new GetterVM<IEnumerable<object>>() InspectorNode.Source; 
                //ViewModel.ChildNodeVMs = ReflectionNodeVM.GetFor(dataNodeVM.GetValue());
                //ViewModel.ChildNodeVMs = ReflectionNodeVM.GetFor(dataNodeVM.GetValue());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        SetChildGenerator(); // .razor

        return base.OnInitializedAsync();
    }

    #endregion

    #region Children

    private RenderFragment? ChildGenerator;
    private RenderFragment? CurrentChildGenerator;

    #region (private)

    private void UpdateChildContent()
    {
        CurrentChildGenerator =
            ViewModel?.NodeVM?.AreChildrenVisible == true
                && ViewModel?.NodeVM?.Children.Count > 0 == true
                && NodeVM.Depth < ViewModel.NodeVM.InheritedOptions.MaxDepth
            ? ChildGenerator
            : null;
    }

    #endregion

    #endregion
}

