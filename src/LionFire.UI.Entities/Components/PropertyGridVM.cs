using LionFire.Mvvm;
using LionFire.UI.Metadata;
using ReactiveUI;
using LionFire.UI.Components.PropertyGrid;

namespace LionFire.UI.Components;

public class PropertyGridVM : ReactiveObject, IObjectEditorVM
{
    [Reactive]
    public object Object { get; set; }

    public bool ReadOnly { get; set; }

    public int MaxDepth { get; set; } = 4; // TEMP TODO: change to something like 15, maybe

    //public EditApproach EditApproach { get; set; } = EditApproach.InPlace;
    
    public RelevanceFlags ReadRelevance { get; set; } = RelevanceFlags.DefaultForUser;
    public RelevanceFlags WriteRelevance { get; set; } = RelevanceFlags.DefaultForUser;

    #region Derived

    public TypeInteractionModel? TypeModel => TypeInteractionModels.Get(Object?.GetType());

    #endregion

    #region Lifecycle

    public PropertyGridVM(IViewModelProvider viewModelProvider)
    {
        this.WhenAnyValue(t => t.Object)
            .Subscribe(change => Init(change));
        ViewModelProvider = viewModelProvider;
    }

    public void Init(object o)
    {
        MemberVMs = MemberVM.GetFor(o); // TypeModel?.Members.Select(m => MemberVM.Create(m, o)).ToList() ?? new();

        Title = o?.GetType().Name.ToDisplayString() ?? "???";
    }

    #endregion

    #region Title

    public string Title { get; set; } = "Properties";
    public bool ShowTitle { get; set; } = true;

    #endregion

    #region Item Filters

    [Reactive]
    public bool ShowFilterTypes { get; set; }

    [Reactive]
    public PropertyGridItemType VisibleItemTypes { get; set; } = PropertyGridItemType.Data;

    public bool ShowDataMembers
    {
        get => VisibleItemTypes.HasFlag(PropertyGridItemType.Data);
        set
        {
            if (value) VisibleItemTypes |= PropertyGridItemType.Data; else VisibleItemTypes &= ~PropertyGridItemType.Data;
        }
    }
    public bool ShowEvents
    {
        get => VisibleItemTypes.HasFlag(PropertyGridItemType.Events);
        set
        {
            if (value) VisibleItemTypes |= PropertyGridItemType.Events; else VisibleItemTypes &= ~PropertyGridItemType.Events;
        }
    }
    public bool ShowMethods
    {
        get => VisibleItemTypes.HasFlag(PropertyGridItemType.Methods);
        set
        {
            if (value) VisibleItemTypes |= PropertyGridItemType.Methods; else VisibleItemTypes &= ~PropertyGridItemType.Methods;
        }
    }

    #endregion

    #region Items

    public IEnumerable<MemberVM> MemberVMs { get; set; } = Enumerable.Empty<MemberVM>();

    public IViewModelProvider ViewModelProvider { get; }

    public bool CanRead(MemberVM member)
        => member.MemberInfoVM.ReadRelevance.HasFlag(ReadRelevance);

    public bool CanWrite(MemberVM member)
        => member.MemberInfoVM.WriteRelevance.HasFlag(WriteRelevance);

    #endregion
}
