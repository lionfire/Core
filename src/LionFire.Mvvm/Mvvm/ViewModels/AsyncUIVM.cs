using LionFire.Metadata;
using LionFire.Mvvm;
using ReactiveUI.SourceGenerators;

namespace LionFire.UI.ViewModels;

/// <summary>
/// Common UI-related functionality for AsyncVM
/// </summary>
/// <typeparam name="T"></typeparam>
public partial class AsyncUIVM<T> : AsyncVM<T>
    where T : class
{
    public AsyncUIVM(T value) : base(value) { }

    // MOVE to some sort of multi-purpose Flag Collection?
    [Relevance(RelevanceFlags.Internal)]
    [Reactive]
    private bool _showDetail;
}

