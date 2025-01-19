using LionFire.Metadata;
using LionFire.Mvvm;
using ReactiveUI.SourceGenerators;

namespace LionFire.UI.ViewModels;

/// <summary>
/// Common UI-related functionality for AsyncVM
/// </summary>
/// <typeparam name="T"></typeparam>
public partial class AsyncItemVM<T> : AsyncViewModelBase<T>
    where T : class
{
    public AsyncItemVM(T value) : base(value) { }

    // MOVE to some sort of multi-purpose Flag Collection?  Or factor out so it is introduced on demand.  Or better: invert so parent knows which items are expanded.
    [Relevance(RelevanceFlags.Internal)]
    [Reactive]
    private bool _showDetail;
}

