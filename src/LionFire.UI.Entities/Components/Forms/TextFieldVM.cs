using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.UI.Components;

public partial class ValueViewerVM<TValue> : ReactiveObject
{
    [ReactiveUI.SourceGenerators.Reactive]
    private TValue? _sourceValue;
}

public partial class ValueEditorVM<TValue> : ValueViewerVM<TValue>
{
    [ReactiveUI.SourceGenerators.Reactive]
    private TValue? _value;

    public void Discard() { Value = SourceValue; }

    public Task Save()
    {
        throw new NotImplementedException();
    }


}

public partial class TextFieldVM<TValue> : ReactiveObject
{
    [ReactiveUI.SourceGenerators.Reactive]
    private ValueEditorVM<TValue> _valueVM;
}
