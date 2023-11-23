#if TODO
using LionFire.Metadata;
using LionFire.UI;

namespace LionFire.Inspection.ViewModels;

// TODO: Source Generator to get next non-null property value
public class InspectorOptions_OverlayProxy<TContainer> : IInspectorOptions
{
    public InspectorOptions_OverlayProxy(TContainer leaf, Func<TContainer, (IInspectorOptions?, TContainer?)> getParent)
    {
        Leaf = leaf;
        GetParent = getParent;
    }

    public EditApproach? EditApproach { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public HashSet<string>? GroupBlacklist { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public HashSet<string>? GroupWhitelist { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public HashSet<Type>? InspectorBlacklist { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public HashSet<Type>? InspectorWhitelist { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int? MaxDepth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool? ReadOnly { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public RelevanceFlags? ReadRelevance { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public RelevanceFlags? WriteRelevance { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public object? FlexData { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public TContainer Leaf { get; }
    public Func<TContainer, (IInspectorOptions?, TContainer?)> GetParent { get; }
}

#endif