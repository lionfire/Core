
using LionFire.IO;
using LionFire.Reactive;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LionFire.Blazor.Components.MudBlazor_.PropertyGrid;

public partial class InspectorCellVM : ReactiveObject
{
    [ReactiveUI.SourceGenerators.Reactive]
    private object? _value;

    public Type ValueType { get; set; }


}
