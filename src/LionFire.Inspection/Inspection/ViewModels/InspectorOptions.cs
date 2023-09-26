using LionFire.Data.Async.Gets;
using LionFire.FlexObjects;
using LionFire.Metadata;
using LionFire.Ontology;
using LionFire.UI;
using ReactiveUI;

namespace LionFire.Inspection.ViewModels;

/// <summary>
/// Options and relationships useful for inspection.
/// </summary>
public class InspectorOptions : ReactiveObject, IInspectorOptions
{
    public static InspectorOptions Default { get; set; }
    public static readonly InspectorOptions DefaultDefault = new InspectorOptions
    {
        EditApproach = UI.EditApproach.InPlace,
        ReadRelevance = RelevanceFlags.DefaultForUser,
        WriteRelevance = RelevanceFlags.DefaultForUser,
        MaxDepth = 50,
        GetChildrenOnExpandRetryDelay = TimeSpan.FromSeconds(2),
        VisibleItemTypes = InspectorNodeKind.Data,
        ShowChildrenForNodeKinds = InspectorNodeKind.Group,
        FlattenedNodeKinds = InspectorNodeKind.Group,
        ShowChildrenForDepthBelow = 1,
        ShowAll = false,
    };

    static InspectorOptions()
    {
        Default = DefaultDefault;
    }

    object? IFlex.FlexData { get; set; }

    //#region Relationships

    //public InspectorOptions? Parent { get; }

    //#endregion

    #region Lifecycle

    //public InspectorOptions(InspectorOptions? parent = null)
    //{
    //    Parent = parent;
    //}

    #endregion

    #region Options

    // ENH: Use Overlay objects to allow acquisition of parent properties

    public HashSet<Type>? InspectorBlacklist { get; set; }
    public HashSet<Type>? InspectorWhitelist { get; set; }

    public HashSet<string>? GroupBlacklist { get; set; }
    public HashSet<string>? GroupWhitelist { get; set; }

    // ENH: Wildcard matching
    //public List<string>? GroupPatternBlacklist { get; set; }
    //public List<string>? GroupPatternWhitelist { get; set; }

    // ENH:
    //public Dictionary<Type, GetterOptions>? GetterOptions { get; set; }

    public bool? ReadOnly { get; set; }

    public int? MaxDepth { get; set; }

    public EditApproach? EditApproach { get; set; }

    [Reactive]
    public RelevanceFlags? ReadRelevance { get; set; }

    [Reactive]
    public RelevanceFlags? WriteRelevance { get; set; }


    #endregion

    public TimeSpan GetChildrenOnExpandRetryDelay { get; set; }

    [Reactive]
    public InspectorNodeKind VisibleItemTypes { get; set; }

    public InspectorNodeKind ShowChildrenForNodeKinds { get; set; }
    public InspectorNodeKind FlattenedNodeKinds { get; set; }
    public int ShowChildrenForDepthBelow { get; set; }

    public bool ShowAll { get; set; }
    public bool DiagnosticsMode { get; set; }
    //public HashSet<string>? FlattenedGroups { get; }


}
