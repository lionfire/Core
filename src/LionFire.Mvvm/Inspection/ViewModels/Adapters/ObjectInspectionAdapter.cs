

namespace LionFire.Inspection;

public partial class ObjectInspectionAdapter<TObject> : ReactiveObject
{
    public ObjectInspectionAdapter(TObject obj)
    {
        Object = obj;
    }

    [ReactiveUI.SourceGenerators.Reactive]
    private TObject? _object;
}
