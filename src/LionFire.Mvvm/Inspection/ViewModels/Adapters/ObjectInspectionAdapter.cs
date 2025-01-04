
using ReactiveUI.Fody.Helpers;

namespace LionFire.Inspection;

public class ObjectInspectionAdapter<TObject> : ReactiveObject
{
    public ObjectInspectionAdapter(TObject obj)
    {
        Object = obj;
    }

    [Reactive]
    public TObject? Object { get; set; }
}
