
using LionFire.IO;
using LionFire.Reactive;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LionFire.Blazor.Components.MudBlazor_.PropertyGrid;

public class InspectorCellVM : ReactiveObject
{
    [Reactive]
    public object? Value { get; set; }

    public Type ValueType { get; set; }


}
