using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.UI.Components;

public class ValueViewerVM<TValue> : ReactiveObject
{
    [Reactive]
    public TValue? SourceValue { get; set; }


}

public class ValueEditorVM<TValue> : ValueViewerVM<TValue>
{
    [Reactive]
    public TValue? Value { get; set; }

    public void Discard() { Value = SourceValue; }

    public Task Save()
    {
        throw new NotImplementedException();
    }


}

public class TextFieldVM<TValue> : ReactiveObject
{
    [Reactive]
    public ValueEditorVM<TValue> ValueVM { get; set; }


}
