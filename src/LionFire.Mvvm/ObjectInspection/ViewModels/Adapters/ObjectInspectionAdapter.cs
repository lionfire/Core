
namespace LionFire.Mvvm.ObjectInspection;

public class ObjectInspectionAdapter<TObject> : ReactiveObject
{
    public ObjectInspectionAdapter(TObject obj)
    {
        Object = obj;
    }

    [Reactive]
    public TObject? Object { get; set; }
}
