
using LionFire.IO;
using LionFire.Reactive;

namespace LionFire.Blazor.Components.MudBlazor_.PropertyGrid;

public partial class InspectorCellVM : ReactiveObject
{
    [ReactiveUI.SourceGenerators.Reactive]
    private object? _value;

    public Type ValueType { get; set; }


}
