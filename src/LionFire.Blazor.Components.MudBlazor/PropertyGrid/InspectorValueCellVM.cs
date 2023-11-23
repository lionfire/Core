
using LionFire.Data.Async;
//using LionFire.Inspection.ViewModels;

namespace LionFire.Inspection.ViewModels;
//namespace LionFire.Blazor.Components.MudBlazor_.PropertyGrid;

public class InspectorValueCellVM : ReactiveObject
{
    public NodeVM? NodeVM { get; set; }
}

public class InspectorValueCellVM<TValue> : ReactiveObject // MOVE to Inspector DLL
{
    private NodeVM? nodeVM;

    public NodeVM? NodeVM { get => nodeVM; set { nodeVM = value; OnParametersSet(); } }

    public bool CanWrite => ValueState != null && ValueState.CanWrite;

    public IValueState<TValue>? ValueState { get; private set; }

    //private TValue Coerce(object o) => o switch
    //{
    //    TValue s => s,
    //    _ => "",
    //};

    // public string TValue { get; set; }
    
    public void OnParametersSet()
    {
        ArgumentNullException.ThrowIfNull(NodeVM);

        // TODO: see if TValue is supported

        // if (NodeVM.AsyncValue != null)
        // {
        //     bindingDisposable = NodeVM.AsyncValue.WhenAnyValue(x => x.Value).Subscribe(v => Value = Coerce(v));
        //     SupportsWriting = true;
        // }
        // else if (NodeVM.Getter != null)
        // {
        //     bindingDisposable = NodeVM.Getter.WhenAnyValue(x => x.ReadCacheValue).Subscribe(v => Value = Coerce(v));
        //     SupportsWriting = false;
        // }
        // else if (NodeVM.Setter != null)
        // {
        //     bindingDisposable = NodeVM.Getter.WhenAnyValue(x => x.Value).Subscribe(v => Value = Coerce(v));
        //     SupportsWriting = true;
        // }
        // else
        // {
        //     Debug.WriteLine("BoolEditor: No binding");
        // }
        if (NodeVM.ValueState != null)
        {
            ValueState = NodeVM.ValueState!.Cast<object, TValue>();
        }
        else
        {
            Debug.WriteLine($"{this.GetType().FullName}: No binding");
        }
    }

    //IDisposable? bindingDisposable;


    //public void Dispose()
    //{
    //    bindingDisposable?.Dispose();
    //    bindingDisposable = null;
    //}
}
