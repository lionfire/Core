using LionFire.FlexObjects;
using LionFire.Metadata;
using LionFire.UI;

namespace LionFire.Inspection.ViewModels;

[Overlayable] // TODO: Source generator overlay for IInspectorContext
public interface IInspectorOptions : IFlex//, IParented<InspectorOptions>
{
    EditApproach? EditApproach { get; }
    bool InPlaceEditing { get; }

    HashSet<string>? GroupBlacklist { get; }
    HashSet<string>? GroupWhitelist { get; }
    HashSet<Type>? InspectorBlacklist { get; }
    HashSet<Type>? InspectorWhitelist { get; }
    int? MaxDepth { get; }
    bool? ReadOnly { get; }
    RelevanceFlags? ReadRelevance { get; }
    RelevanceFlags? WriteRelevance { get; }

    /// <summary>
    /// MaxValue: disabled
    /// </summary>
    TimeSpan GetChildrenOnExpandRetryDelay { get; }


    InspectorNodeKind VisibleItemTypes { get; }

    //HashSet<string>? FlattenedGroups { get; }
    public InspectorNodeKind FlattenedNodeKinds { get; set; }
    InspectorNodeKind ShowChildrenForNodeKinds { get; set; }
    int ShowChildrenForDepthBelow { get; set; }
    bool ShowAll { get; set; }
    bool DiagnosticsMode { get; set; }
    
}
