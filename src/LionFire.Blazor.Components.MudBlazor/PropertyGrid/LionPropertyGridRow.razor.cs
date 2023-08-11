using LionFire.Data.Mvvm;
using LionFire.Mvvm.ObjectInspection;
using LionFire.UI.Components;
using Microsoft.AspNetCore.Components;

namespace LionFire.Blazor.Components.MudBlazor_.PropertyGrid;

public partial class LionPropertyGridRow
{
    #region Parameters

    [CascadingParameter(Name = "PropertyGridVM")]
    public PropertyGridVM? PropertyGridVM { get; set; }// { set => ViewModel.PropertyGridVM = value; }

    [Parameter]
    public int Depth { get; set; }

    [Parameter]
    public IInspectorNode? MemberVM { get; set; }// { get => ViewModel.MemberVM; set => ViewModel.MemberVM = value; }

    #endregion

    #region Lifecycle

    protected override Task OnParametersSetAsync()
    {
        ViewModel.MemberVM = MemberVM;
        ViewModel.PropertyGridVM = PropertyGridVM;

        return base.OnParametersSetAsync();
    }

    public bool ShowValueType => ViewModel.ValueTypeDiffersFromMemberType;

    protected override Task OnInitializedAsync()
    {
        Debug.WriteLine($"Depth: {Depth}.  Target: {MemberVM.Source?.GetType().Name}");

        ViewModel.Refresh();

        if (Depth == 0) ViewModel.IsExpanded = true;
        UpdateChildContent(); // ENH - bind IsExpanded to UpdateChildContent

        if(MemberVM.Info.CanRead() && MemberVM.Info.Type is System.Collections.IEnumerable e)
        {
            try
            {
                ChildMemberVMs2 = new GetterVM<IEnumerable<object>>() InspectorNode.Source 
                ViewModel.ChildMemberVMs = ReflectionMemberVM.GetFor(dataMemberVM.GetValue());
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

    private RenderFragment ChildGenerator;
    private RenderFragment childrenContent;

    public Task ExpandedChanged(bool newVal)
    {
        ViewModel.IsExpanded = newVal;
        UpdateChildContent();
        return Task.CompletedTask;
    }

    #region (private)

    private void UpdateChildContent()
    {
        childrenContent = ViewModel.IsExpanded ? ChildGenerator : null;
    }

    #endregion

    #endregion
}
